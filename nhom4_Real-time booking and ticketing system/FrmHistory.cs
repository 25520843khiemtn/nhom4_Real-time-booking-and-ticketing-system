using Newtonsoft.Json.Linq;
using nhom4_Real_time_booking_and_ticketing_system.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicketBooking.Shared;


namespace nhom4_Real_time_booking_and_ticketing_system
{
    public partial class FrmHistory : Form
    {
        public FrmHistory()
        {
            InitializeComponent();
            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);

            Load += async (s, e) => await LoadHistoryAsync();
            btnRefresh.Click += async (s, e) => await LoadHistoryAsync();
            btnBack.Click += (s, e) => Close();
        }

        private static async Task<ProtocolMessage> SendAsync(string type, object payload = null)
        {
            ProtocolMessage req = new ProtocolMessage { MessageType = type };
            if (payload != null) req.Data = JObject.FromObject(payload);

            ProtocolMessage resp = await AppSession.Connection.SendRequestAsync(req);
            if (resp.Status != ResponseStatus.SUCCESS)
            {
                JObject obj = resp.Data as JObject;
                string msg = obj != null ? obj["Message"]?.ToString() : null;
                throw new Exception(msg ?? "Server trả về lỗi.");
            }
            return resp;
        }

        private async Task LoadHistoryAsync()
        {
            btnRefresh.Enabled = false;
            lblMessage.Text = "Đang tải lịch sử...";
            lblUserInfo.Text = "Tài khoản: " + AppSession.UserId;
            dgvHistory.Rows.Clear();

            try
            {
                if (AppSession.Connection == null || !AppSession.Connection.IsConnected)
                    throw new Exception("Kết nối Server không còn hoạt động.");

                ProtocolMessage h = await SendAsync(MessageTypes.GET_HISTORY);
                List<TransactionModel> list = h.Data != null
                    ? h.Data.ToObject<List<TransactionModel>>()
                    : new List<TransactionModel>();

                Dictionary<int, string> movieName = new Dictionary<int, string>();
                Dictionary<int, ShowtimeModel> shows = new Dictionary<int, ShowtimeModel>();

                ProtocolMessage mv = await SendAsync(MessageTypes.GET_MOVIES);
                foreach (MovieModel movie in mv.Data.ToObject<List<MovieModel>>())
                {
                    ProtocolMessage st = await SendAsync(
                        MessageTypes.GET_SHOWTIMES,
                        new GetShowtimesPayload { MovieId = movie.MovieId });

                    foreach (ShowtimeModel show in st.Data.ToObject<List<ShowtimeModel>>())
                    {
                        shows[show.ShowtimeId] = show;
                        movieName[show.ShowtimeId] = movie.Title;
                    }
                }

                decimal total = 0;
                foreach (TransactionModel t in list.OrderByDescending(x => x.BuyTime))
                {
                    string title = movieName.ContainsKey(t.ShowtimeId) ? movieName[t.ShowtimeId] : "Suất #" + t.ShowtimeId;
                    string start = shows.ContainsKey(t.ShowtimeId) ? shows[t.ShowtimeId].StartTime.ToString("dd/MM/yyyy HH:mm") : "-";

                    foreach (TransactionDetailModel d in t.Details ?? new List<TransactionDetailModel>())
                    {
                        dgvHistory.Rows.Add(
                            t.BuyTime.ToString("dd/MM/yyyy HH:mm"), title, start, d.SeatId, d.UnitPrice.ToString("N0") + " đ");
                        total += d.UnitPrice;
                    }
                }

                lblMessage.Text = dgvHistory.Rows.Count == 0
                    ? "Bạn chưa mua vé nào."
                    : dgvHistory.Rows.Count + " vé, tổng cộng " + total.ToString("N0") + " đ";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Lỗi: " + ex.Message;
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }
    }
}