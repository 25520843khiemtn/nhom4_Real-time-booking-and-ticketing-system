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
using nhom4_Real_time_booking_and_ticketing_system.Network;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public partial class FrmShowtime : Form
    {
        private readonly MovieModel _selectedMovie;

        private List<ShowtimeModel> _allShowtimes =
            new List<ShowtimeModel>();

        public FrmShowtime(MovieModel selectedMovie)
        {
            InitializeComponent();

            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);

            if (selectedMovie == null)
                throw new ArgumentNullException("selectedMovie");

            _selectedMovie = selectedMovie;

            // Thiết lập thông tin phim được chọn.
            lblMovieName.Text = _selectedMovie.Title;

            // Chưa có dữ liệu poster trong MovieModel.
            picMovie.Image = null;

            // Không cho chọn ngày trong quá khứ.
            dtpShowDate.MinDate = DateTime.Today;
            dtpShowDate.Value = DateTime.Today;

            // Gắn các sự kiện.
            Load += FrmShowtime_Load;
            dtpShowDate.ValueChanged += dtpShowDate_ValueChanged;
            btnBack.Click += btnBack_Click;

            // Cho phép cuộn danh sách suất chiếu.
            flpShowtimes.AutoScroll = true;
            flpShowtimes.WrapContents = true;
        }

        // Tải suất chiếu khi Form được mở.
        private async void FrmShowtime_Load(object sender, EventArgs e)
        {
            await LoadShowtimesAsync();
        }

        // Gửi yêu cầu GET_SHOWTIMES đến Server.
        private async Task LoadShowtimesAsync()
        {
            try
            {
                if (AppSession.Connection == null ||
                    !AppSession.Connection.IsConnected)
                {
                    throw new Exception(
                        "Kết nối Server không còn hoạt động."
                    );
                }

                flpShowtimes.Controls.Clear();

                lblRoom.Text = "Đang tải danh sách suất chiếu...";

                ProtocolMessage request = new ProtocolMessage
                {
                    MessageType = MessageTypes.GET_SHOWTIMES,
                    Data = Newtonsoft.Json.Linq.JObject.FromObject(
                        new GetShowtimesPayload
                        {
                            MovieId = _selectedMovie.MovieId
                        }
                    )
                };

                ProtocolMessage response =
                    await AppSession.Connection.SendRequestAsync(request);

                if (response.Status != ResponseStatus.SUCCESS)
                {
                    string errorMessage =
                        "Không thể lấy danh sách suất chiếu.";

                    if (response.Data != null)
                    {
                        errorMessage =
                            response.Data["Message"]?.ToString()
                            ?? errorMessage;
                    }

                    throw new Exception(errorMessage);
                }

                _allShowtimes =
                    response.GetData<List<ShowtimeModel>>()
                    ?? new List<ShowtimeModel>();

                DisplayShowtimesByDate();
            }
            catch (Exception ex)
            {
                flpShowtimes.Controls.Clear();

                lblRoom.Text = "Không thể tải suất chiếu.";

                MessageBox.Show(
                    ex.Message,
                    "Lỗi tải suất chiếu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Hiển thị các suất chiếu thuộc ngày đang chọn.
        private void DisplayShowtimesByDate()
        {
            flpShowtimes.Controls.Clear();

            DateTime selectedDate = dtpShowDate.Value.Date;

            List<ShowtimeModel> filteredShowtimes =
                _allShowtimes
                    .Where(s => s.StartTime.Date == selectedDate)
                    .OrderBy(s => s.StartTime)
                    .ToList();

            if (filteredShowtimes.Count == 0)
            {
                lblRoom.Text = "Không có suất chiếu trong ngày này.";

                Label lblEmpty = new Label();
                lblEmpty.Text = "Không tìm thấy suất chiếu.";
                lblEmpty.ForeColor = Color.White;
                lblEmpty.AutoSize = true;
                lblEmpty.Margin = new Padding(8);

                flpShowtimes.Controls.Add(lblEmpty);
                return;
            }

            lblRoom.Text =
                "Có " + filteredShowtimes.Count +
                " suất chiếu trong ngày " +
                selectedDate.ToString("dd/MM/yyyy");

            foreach (ShowtimeModel showtime in filteredShowtimes)
            {
                Button button = CreateShowtimeButton(showtime);

                flpShowtimes.Controls.Add(button);
            }
        }

        // Tạo nút đại diện cho một suất chiếu.
        private Button CreateShowtimeButton(ShowtimeModel showtime)
        {
            Button button = new Button();

            button.BackColor = Color.FromArgb(37, 99, 235);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            button.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold
            );

            button.Size = new Size(210, 85);
            button.Margin = new Padding(8);
            button.Cursor = Cursors.Hand;

            button.Text =
                showtime.StartTime.ToString("HH:mm") +
                Environment.NewLine +
                showtime.RoomName +
                Environment.NewLine +
                showtime.TicketPrice.ToString("N0") + " đ";

            // Lưu dữ liệu suất chiếu vào Tag.
            button.Tag = showtime;

            button.Click += btnShowtime_Click;

            return button;
        }

        // Khi người dùng chọn một ngày khác.
        private void dtpShowDate_ValueChanged(object sender, EventArgs e)
        {
            DisplayShowtimesByDate();
        }

        // Khi người dùng chọn một suất chiếu.
        private void btnShowtime_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
                return;

            ShowtimeModel selectedShowtime =
                button.Tag as ShowtimeModel;

            if (selectedShowtime == null)
                return;

            FrmSeatBooking seatForm =
                new FrmSeatBooking(selectedShowtime);

            Hide();

            seatForm.FormClosed += (s, args) => Show();

            seatForm.Show();
        }

        // Quay lại danh sách phim.
        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}