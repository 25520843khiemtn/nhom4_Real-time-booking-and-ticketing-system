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
    public partial class FrmMovieList : Form
    {
        public FrmMovieList()
        {
            InitializeComponent();

            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);

            flpMovielist.AutoScroll = true;
            flpMovielist.WrapContents = true;

            Load += FrmMovieList_Load;
            btnLogout.Click += btnLogout_Click;
            btnHistory.Click += btnHistory_Click;
        }

        private async void FrmMovieList_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "Xin chào, " + AppSession.UserId;
            await LoadMoviesAsync();
        }

        private async Task LoadMoviesAsync()
        {
            try
            {
                if (AppSession.Connection == null || !AppSession.Connection.IsConnected)
                {
                    MessageBox.Show(
                        "Kết nối Server không còn hoạt động.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                flpMovielist.Controls.Clear();

                Label lblLoading = new Label();
                lblLoading.Text = "Đang tải danh sách phim...";
                lblLoading.ForeColor = Color.White;
                lblLoading.AutoSize = true;
                lblLoading.Margin = new Padding(10);

                flpMovielist.Controls.Add(lblLoading);

                ProtocolMessage request = new ProtocolMessage
                {
                    MessageType = MessageTypes.GET_MOVIES
                };

                ProtocolMessage response = await AppSession.Connection.SendRequestAsync(request);

                if (response.Status != ResponseStatus.SUCCESS)
                {
                    string errorMessage = "Không thể lấy danh sách phim.";

                    if (response.Data != null)
                    {
                        errorMessage = response.Data["Message"]?.ToString() ?? errorMessage;
                    }

                    throw new Exception(errorMessage);
                }

                List<MovieModel> movies = response.GetData<List<MovieModel>>();

                flpMovielist.Controls.Clear();

                if (movies == null || movies.Count == 0)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "Hiện chưa có phim nào.";
                    lblEmpty.ForeColor = Color.White;
                    lblEmpty.AutoSize = true;
                    lblEmpty.Margin = new Padding(10);

                    flpMovielist.Controls.Add(lblEmpty);
                    return;
                }

                foreach (MovieModel movie in movies)
                {
                    if (movie != null)
                    {
                        Panel movieCard = CreateMovieCard(movie);
                        flpMovielist.Controls.Add(movieCard);
                    }
                }
            }
            catch (Exception ex)
            {
                flpMovielist.Controls.Clear();

                Label lblError = new Label();
                lblError.Text = "Không thể tải danh sách phim.";
                lblError.ForeColor = Color.FromArgb(252, 165, 165);
                lblError.AutoSize = true;
                lblError.Margin = new Padding(10);

                flpMovielist.Controls.Add(lblError);

                MessageBox.Show(
                    ex.Message,
                    "Lỗi tải danh sách phim",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private Panel CreateMovieCard(MovieModel movie)
        {
            Panel card = new Panel();

            card.BackColor = Color.FromArgb(24, 36, 56);
            card.Size = new Size(340, 440);
            card.Margin = new Padding(8);
            card.Tag = movie;

            PictureBox picture = new PictureBox();

            picture.BackColor = Color.FromArgb(16, 24, 39);
            picture.Location = new Point(25, 20);
            picture.Size = new Size(290, 270);
            picture.SizeMode = PictureBoxSizeMode.Zoom;

            Label lblName = new Label();

            lblName.Text = movie.Title;
            lblName.Font = new Font("Segoe UI", 13F, FontStyle.Bold);

            lblName.ForeColor = Color.White;
            lblName.Location = new Point(25, 302);
            lblName.Size = new Size(290, 32);
            lblName.AutoEllipsis = true;

            Label lblDuration = new Label();

            lblDuration.Text = "Thời lượng: " + movie.DurationMinutes + " phút";

            lblDuration.ForeColor = Color.FromArgb(156, 163, 175);

            lblDuration.Location = new Point(25, 338);
            lblDuration.Size = new Size(290, 24);

            Label lblGenre = new Label();

            lblGenre.Text = "Thể loại: " +
                (string.IsNullOrWhiteSpace(movie.Genre)
                    ? "Chưa cập nhật"
                    : movie.Genre);

            lblGenre.ForeColor = Color.FromArgb(156, 163, 175);

            lblGenre.Location = new Point(25, 362);
            lblGenre.Size = new Size(290, 24);
            lblGenre.AutoEllipsis = true;

            Button btnBook = new Button();

            btnBook.Text = "Đặt vé";
            btnBook.BackColor = Color.FromArgb(37, 99, 235);

            btnBook.ForeColor = Color.White;
            btnBook.FlatStyle = FlatStyle.Flat;
            btnBook.FlatAppearance.BorderSize = 0;

            btnBook.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            btnBook.Cursor = Cursors.Hand;
            btnBook.Location = new Point(25, 390);
            btnBook.Size = new Size(290, 42);

            btnBook.Tag = movie;

            btnBook.Click += btnBookTicket_Click;

            card.Controls.Add(picture);
            card.Controls.Add(lblName);
            card.Controls.Add(lblDuration);
            card.Controls.Add(lblGenre);
            card.Controls.Add(btnBook);

            return card;
        }

        private void btnBookTicket_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
                return;

            MovieModel selectedMovie = button.Tag as MovieModel;

            if (selectedMovie == null)
                return;

            FrmShowtime showtimeForm = new FrmShowtime(selectedMovie);

            Hide();

            showtimeForm.FormClosed += (s, args) =>
            {
                if (!IsDisposed)
                    Show();
            };

            showtimeForm.Show();
        }
        public bool LoggingOut { get; private set; }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) 
                return;

            LoggingOut = true;
            Close();
        }
        private void btnHistory_Click(object sender, EventArgs e)
        {
            FrmHistory history = new FrmHistory();
            Hide();
            history.FormClosed += (s, args) => { if (!IsDisposed) Show(); };
            history.Show();
        }
    }
}