using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TicketBooking.Shared;
using nhom4_Real_time_booking_and_ticketing_system.Network;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public partial class FrmSeatBooking : Form
    {
        private enum SeatState
        {
            Available,
            Mine,
            HeldByOther,
            Sold
        }

        private readonly Dictionary<string, SeatState> _seatStates =
            new Dictionary<string, SeatState>();

        private readonly Dictionary<string, DateTime> _heldSeatExpiry =
            new Dictionary<string, DateTime>();

        private readonly ShowtimeModel _selectedShowtime;

        private bool _loadingSeats = false;

        public FrmSeatBooking()
        {
            InitializeComponent();
            _selectedShowtime = null;
        }
        public FrmSeatBooking(ShowtimeModel selectedShowtime)
        {
            InitializeComponent();

            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);

            if (selectedShowtime == null)
                throw new ArgumentNullException(nameof(selectedShowtime));

            _selectedShowtime = selectedShowtime;

            BuildSeatMap();

            Load -= FrmSeatBooking_Load;
            Load += FrmSeatBooking_Load;

            FormClosed -= FrmSeatBooking_FormClosed;
            FormClosed += FrmSeatBooking_FormClosed;

            btnPayment.Click -= btnPayment_Click;
            btnPayment.Click += btnPayment_Click;

            btnCancelHold.Click -= btnCancelHold_Click;
            btnCancelHold.Click += btnCancelHold_Click;

            btnBack.Click -= btnBack_Click;
            btnBack.Click += btnBack_Click;

            timerHold.Stop();
            timerHold.Interval = 1000;
            timerHold.Tick -= timerHold_Tick;
            timerHold.Tick += timerHold_Tick;

            if (AppSession.Connection != null)
            {
                AppSession.Connection.MessageReceived -= Connection_MessageReceived;

                AppSession.Connection.MessageReceived += Connection_MessageReceived;
            }
            AppSession.Reconnected -= AppSession_Reconnected;
            AppSession.Reconnected += AppSession_Reconnected;

            lblShowInfo.Text =
                _selectedShowtime.RoomName + " - " +
                _selectedShowtime.StartTime.ToString("dd/MM/yyyy HH:mm");

            UpdateSummary();
        }

        private void BuildSeatMap()
        {
            tlpSeatMap.Controls.Clear();
            _seatStates.Clear();

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string id =
                        ((char)('A' + row)).ToString() + (col + 1);

                    Button btn = new Button
                    {
                        Name = "btnSeat_" + id,
                        Text = id,
                        Tag = id,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(3),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };

                    btn.Click += SeatButton_Click;

                    tlpSeatMap.Controls.Add(btn, col, row);

                    SetSeatState(id, SeatState.Available);
                }
            }
        }

        private Button FindSeat(string id)
        {
            return tlpSeatMap.Controls
                .OfType<Button>()
                .FirstOrDefault(b => (string)b.Tag == id);
        }

        private void SetSeatState(string id, SeatState state)
        {
            Button btn = FindSeat(id);

            if (btn == null)
                return;

            _seatStates[id] = state;

            switch (state)
            {
                case SeatState.Mine:
                    btn.BackColor = Color.FromArgb(37, 99, 235);
                    btn.ForeColor = Color.White;
                    btn.Enabled = true;
                    break;

                case SeatState.HeldByOther:
                    btn.BackColor = Color.FromArgb(234, 179, 8);
                    btn.ForeColor = Color.Black;
                    btn.Enabled = false;
                    break;

                case SeatState.Sold:
                    btn.BackColor = Color.FromArgb(249, 115, 22);
                    btn.ForeColor = Color.White;
                    btn.Enabled = false;
                    break;

                default:
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.Black;
                    btn.Enabled = true;
                    break;
            }
        }

        // Chuyển SeatModel từ Server thành trạng thái giao diện.
        private void ApplySeatModel(SeatModel seat)
        {
            if (seat == null)
                return;

            if (seat.ShowtimeId != _selectedShowtime.ShowtimeId)
                return;

            string seatId = seat.SeatId.ToUpperInvariant();

            if (!_seatStates.ContainsKey(seatId))
                return;

            if (seat.Status == SeatStatus.Available)
            {
                _heldSeatExpiry.Remove(seatId);
                SetSeatState(seatId, SeatState.Available);
            }
            else if (seat.Status == SeatStatus.Sold)
            {
                _heldSeatExpiry.Remove(seatId);
                SetSeatState(seatId, SeatState.Sold);
            }
            else if (seat.Status == SeatStatus.Held)
            {
                if (seat.LockedBy == AppSession.UserId)
                {
                    SetSeatState(seatId, SeatState.Mine);

                    if (seat.UnlockTime.HasValue)
                    {
                        _heldSeatExpiry[seatId] =
                            seat.UnlockTime.Value;
                    }
                }
                else
                {
                    _heldSeatExpiry.Remove(seatId);
                    SetSeatState(seatId, SeatState.HeldByOther);
                }
            }

            UpdateSummary();
        }

        private async void FrmSeatBooking_Load(object sender, EventArgs e)
        {
            await LoadSeatsAsync();
        }

        private async Task LoadSeatsAsync()
        {
            if (_loadingSeats)
                return;

            if (AppSession.Connection == null ||
                !AppSession.Connection.IsConnected)
            {
                SetMessage("Chưa kết nối đến Server.");
                return;
            }

            _loadingSeats = true;

            try
            {
                ProtocolMessage request = new ProtocolMessage
                {
                    MessageType = MessageTypes.GET_SEATS,
                    UserId = AppSession.UserId,
                    Data = JObject.FromObject(
                        new GetSeatsPayload
                        {
                            ShowtimeId = _selectedShowtime.ShowtimeId
                        })
                };

                ProtocolMessage response =
                    await AppSession.Connection.SendRequestAsync(request);

                if (response.Status != ResponseStatus.SUCCESS)
                {
                    SetMessage("Không thể tải danh sách ghế.");
                    return;
                }

                List<SeatModel> seats =
                    response.GetData<List<SeatModel>>();

                if (seats == null)
                {
                    SetMessage("Dữ liệu ghế từ Server không hợp lệ.");
                    return;
                }

                _heldSeatExpiry.Clear();

                foreach (SeatModel seat in seats)
                {
                    ApplySeatModel(seat);
                }

                UpdateSummary();
                SetMessage("Đã tải trạng thái ghế.");
            }
            catch (Exception ex)
            {
                SetMessage("Lỗi tải ghế: " + ex.Message);
            }
            finally
            {
                _loadingSeats = false;
            }
        }

        private async void SeatButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            string seatId = btn.Tag.ToString();

            if (!_seatStates.ContainsKey(seatId))
                return;

            SeatState state = _seatStates[seatId];

            if (state == SeatState.Sold)
            {
                SetMessage("Ghế " + seatId + " đã được bán, vui lòng chọn ghế khác.");
                return;
            }

            if (state == SeatState.HeldByOther)
            {
                SetMessage("Ghế " + seatId + " đang được người khác giữ. Hãy thử lại sau hoặc chọn ghế khác.");
                return;
            }

            if (state == SeatState.Mine)
            {
                await CancelSeatAsync(seatId);
                return;
            }

            await HoldSeatAsync(seatId);
        }

        private async Task HoldSeatAsync(string seatId)
        {
            if (AppSession.Connection == null ||
                !AppSession.Connection.IsConnected)
            {
                SetMessage("Mất kết nối đến Server.");
                return;
            }

            Button btn = FindSeat(seatId);

            if (btn != null)
                btn.Enabled = false;

            try
            {
                ProtocolMessage request = new ProtocolMessage
                {
                    MessageType = MessageTypes.HOLD_SEAT,
                    UserId = AppSession.UserId,
                    Data = JObject.FromObject(
                        new SeatActionPayload
                        {
                            ShowtimeId = _selectedShowtime.ShowtimeId,
                            SeatId = seatId
                        })
                };

                ProtocolMessage response = await AppSession.Connection.SendRequestAsync(request);

                string message =
                    response.Data?["Message"]?.ToString();

                if (response.Status != ResponseStatus.SUCCESS)
                {
                    SetMessage(message ?? "Không thể giữ ghế.");

                    await LoadSeatsAsync();
                    return;
                }

                SeatModel seat =
                    response.Data?["Seat"]?.ToObject<SeatModel>();

                if (seat != null)
                    ApplySeatModel(seat);

                SetMessage(message ?? "Giữ ghế thành công.");
            }
            catch (Exception ex)
            {
                SetMessage("Lỗi giữ ghế: " + ex.Message);
                await LoadSeatsAsync();
            }
            finally
            {
                UpdateSummary();
            }
        }

        private async Task CancelSeatAsync(string seatId)
        {
            if (AppSession.Connection == null ||
                !AppSession.Connection.IsConnected)
            {
                SetMessage("Mất kết nối đến Server.");
                return;
            }

            Button btn = FindSeat(seatId);

            if (btn != null)
                btn.Enabled = false;

            try
            {
                ProtocolMessage request = new ProtocolMessage
                {
                    MessageType = MessageTypes.CANCEL_HOLD,
                    UserId = AppSession.UserId,
                    Data = JObject.FromObject(
                        new SeatActionPayload
                        {
                            ShowtimeId = _selectedShowtime.ShowtimeId,
                            SeatId = seatId
                        })
                };

                ProtocolMessage response =
                    await AppSession.Connection.SendRequestAsync(request);

                string message =
                    response.Data?["Message"]?.ToString();

                if (response.Status != ResponseStatus.SUCCESS)
                {
                    SetMessage(message ?? "Không thể hủy giữ ghế.");
                    await LoadSeatsAsync();
                    return;
                }

                SeatModel seat =
                    response.Data?["Seat"]?.ToObject<SeatModel>();

                if (seat != null)
                    ApplySeatModel(seat);
                else
                    SetSeatState(seatId, SeatState.Available);

                _heldSeatExpiry.Remove(seatId);

                SetMessage(message ?? "Đã hủy giữ ghế.");
            }
            catch (Exception ex)
            {
                SetMessage("Lỗi hủy giữ ghế: " + ex.Message);
                await LoadSeatsAsync();
            }
            finally
            {
                UpdateSummary();
            }
        }

        private async void btnCancelHold_Click(object sender, EventArgs e)
        {
            List<string> seatIds = _heldSeatExpiry.Keys.ToList();

            if (seatIds.Count == 0)
            {
                SetMessage("Bạn chưa giữ ghế nào.");
                return;
            }

            foreach (string seatId in seatIds)
            {
                await CancelSeatAsync(seatId);
            }

            SetMessage("Đã xử lý yêu cầu hủy giữ ghế.");
        }

        private void Connection_MessageReceived(ProtocolMessage message)
        {
            if (message == null ||
                message.MessageType != MessageTypes.SEAT_UPDATED)
                return;

            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<ProtocolMessage>(
                    HandleSeatUpdatedMessage), message);
            }
            else
            {
                HandleSeatUpdatedMessage(message);
            }
        }

        private void HandleSeatUpdatedMessage(ProtocolMessage message)
        {
            try
            {
                SeatUpdatedPayload payload =
                    message.Data?.ToObject<SeatUpdatedPayload>();

                if (payload == null ||
                    payload.Seat == null ||
                    payload.ShowtimeId != _selectedShowtime.ShowtimeId)
                    return;

                ApplySeatModel(payload.Seat);

                SetMessage(
                    "Ghế " + payload.Seat.SeatId +
                    " vừa được cập nhật: " + payload.Reason);
            }
            catch (Exception ex)
            {
                SetMessage("Lỗi cập nhật trạng thái ghế: " + ex.Message);
            }
        }

        private async void timerHold_Tick(object sender, EventArgs e)
        {
            if (_heldSeatExpiry.Count == 0)
            {
                lblCountdown.Text = "Thời gian giữ: --:--";
                return;
            }

            DateTime earliestExpiry = _heldSeatExpiry.Values.Min();
            TimeSpan remaining = earliestExpiry - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                lblCountdown.Text = "Đang xác nhận trạng thái ghế...";

                await LoadSeatsAsync();
                return;
            }

            lblCountdown.Text =
                "Thời gian giữ: " +
                remaining.Minutes.ToString("00") + ":" +
                remaining.Seconds.ToString("00");
        }

        private async void btnPayment_Click(object sender, EventArgs e)
        {
            List<string> seatIds = _heldSeatExpiry.Keys.OrderBy(s => s).ToList();

            if (seatIds.Count == 0)
            {
                SetMessage("Bạn chưa giữ ghế để thanh toán.");
                return;
            }

            if (AppSession.Connection == null || !AppSession.Connection.IsConnected)
            {
                SetMessage("Mất kết nối đến Server.");
                return;
            }

            btnPayment.Enabled = false;

            try
            {
                ProtocolMessage request = new ProtocolMessage
                {
                    MessageType = MessageTypes.BUY_TICKET,
                    UserId = AppSession.UserId,
                    Data = JObject.FromObject(
                        new BuyTicketPayload
                        {
                            ShowtimeId = _selectedShowtime.ShowtimeId,
                            SeatIds = seatIds
                        })
                };

                ProtocolMessage response = await AppSession.Connection.SendRequestAsync(request);

                string message = response.Data?["Message"]?.ToString();

                if (response.Status != ResponseStatus.SUCCESS)
                {
                    SetMessage(message ?? "Thanh toán thất bại.");
                    MessageBox.Show(message ?? "Thanh toán thất bại.", "Mua vé thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await LoadSeatsAsync();
                    return;
                }

                List<SeatModel> soldSeats =
                    response.Data?["Seats"]?.ToObject<List<SeatModel>>();

                if (soldSeats != null)
                {
                    foreach (SeatModel seat in soldSeats)
                    {
                        ApplySeatModel(seat);
                    }
                }

                _heldSeatExpiry.Clear();

                UpdateSummary();

                SetMessage(message ?? "Thanh toán thành công.");
                MessageBox.Show("Mua vé thành công!\n" + "Suất chiếu: " + _selectedShowtime.RoomName + " - " + _selectedShowtime.StartTime.ToString("dd/MM/yyyy HH:mm") + "\n" + "Ghế: " + string.Join(", ", seatIds) + "\n" + "Tổng tiền: " + (seatIds.Count * _selectedShowtime.TicketPrice).ToString("N0") + " VNĐ\n" + "Bạn có thể xem lại trong mục Lịch sử.", "Mua vé thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SetMessage("Mất kết nối khi thanh toán. Hệ thống đang kết nối lại.");
                MessageBox.Show(
                    "Kết nối bị gián đoạn trong lúc thanh toán nên chưa biết vé đã được mua hay chưa.\n" +
                    "Sau khi kết nối lại, hãy mở mục Lịch sử để kiểm tra trước khi mua lại.",
                    "Cần kiểm tra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                UpdateSummary();
            }
        }

        private void UpdateSummary()
        {
            int count = _heldSeatExpiry.Count;

            lblHeldSeats.Text = count == 0 ? "Ghế đang giữ: chưa có" : "Ghế đang giữ (" + count + "): " + string.Join(", ", _heldSeatExpiry.Keys.OrderBy(s => s));

            lblTotalPrice.Text =
                "Tổng tiền: " +
                (count * _selectedShowtime.TicketPrice).ToString("N0") +
                " VNĐ";

            btnPayment.Enabled = count > 0;
            btnCancelHold.Enabled = count > 0;

            if (count == 0)
            {
                lblCountdown.Text = "Thời gian giữ: --:--";
                timerHold.Stop();
            }
            else if (!timerHold.Enabled)
            {
                timerHold.Start();
            }
        }

        private void SetMessage(string message)
        {
            if (lblMessage != null)
                lblMessage.Text = message;
        }
        private void AppSession_Reconnected()
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke(new Action(async () =>
            {
                try
                {
                    AppSession.Connection.MessageReceived -= Connection_MessageReceived;
                    AppSession.Connection.MessageReceived += Connection_MessageReceived;

                    SetMessage("Đã kết nối lại. Đang tải lại sơ đồ ghế...");
                    await LoadSeatsAsync();
                    SetMessage("Đã kết nối lại. Nếu bạn đang giữ ghế, hãy chọn lại.");
                }
                catch (Exception ex)
                {
                    SetMessage("Lỗi sau khi kết nối lại: " + ex.Message);
                }
            }));
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmSeatBooking_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppSession.Reconnected -= AppSession_Reconnected;
            timerHold.Stop();

            if (AppSession.Connection != null)
            {
                AppSession.Connection.MessageReceived -= Connection_MessageReceived;
            }
        }
    }
}