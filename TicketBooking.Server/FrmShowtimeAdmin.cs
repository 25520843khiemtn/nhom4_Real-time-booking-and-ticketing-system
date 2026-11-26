using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketBooking.Server
{
    public partial class FrmShowtimeAdmin : Form
    {
        public FrmShowtimeAdmin()
        {
            InitializeComponent();
        }

        private void FrmShowtimeAdmin_Load(object sender, EventArgs e)
        {
            string[] movies = { "Phim A", "Phim B", "Phim C" };
            string[] rooms = { "Phòng 1", "Phòng 2" };
            cboMovie.Items.AddRange(movies);
            cboFilterMovie.Items.AddRange(movies);
            cboRoom.Items.AddRange(rooms);
            cboFilterRoom.Items.AddRange(rooms);

            dgvShowtimes.Rows.Add(1, "Phim A", "Phòng 1", "19:00 25/11/2026", "90.000 đ", 48, "Đang mở bán");
            dgvShowtimes.Rows.Add(2, "Phim B", "Phòng 2", "20:30 25/11/2026", "90.000 đ", 40, "Đang mở bán");
            dgvShowtimes.Rows.Add(3, "Phim C", "Phòng 1", "21:00 26/11/2026", "100.000 đ", 48, "Sắp mở bán");
            dgvShowtimes.ClearSelection();

            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            lblMessage.Text = "";
        }

        private void dgvShowtimes_SelectionChanged(object sender, EventArgs e)
        {
            bool hasRow = dgvShowtimes.SelectedRows.Count > 0;
            btnUpdate.Enabled = hasRow;
            btnDelete.Enabled = hasRow;
        }
        // Trả về thông báo lỗi, hoặc null nếu hợp lệ.
        // Kiểm tra trùng giờ trong cùng phòng cần dữ liệu thật, để người viết logic thêm sau với thông báo:
        // "Phòng này đã có suất chiếu trùng giờ."
        private string ValidateInput()
        {
            if (cboMovie.SelectedIndex < 0 || cboRoom.SelectedIndex < 0)
                return "Vui lòng chọn phim và phòng chiếu.";

            DateTime start = dtpStartDate.Value.Date + dtpStartTime.Value.TimeOfDay;
            if (start <= DateTime.Now)
                return "Giờ chiếu phải sau thời điểm hiện tại.";

            if (nudPrice.Value <= 0)
                return "Giá vé phải lớn hơn 0.";

            return null;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string error = ValidateInput();
            if (error != null)
            {
                lblMessage.ForeColor = Color.Firebrick;
                lblMessage.Text = error;
                return;
            }

            DateTime start = dtpStartDate.Value.Date + dtpStartTime.Value.TimeOfDay;
            var vi = new System.Globalization.CultureInfo("vi-VN");
            dgvShowtimes.Rows.Add(dgvShowtimes.Rows.Count + 1, cboMovie.Text, cboRoom.Text,
                start.ToString("HH:mm dd/MM/yyyy"), nudPrice.Value.ToString("N0", vi) + " đ", 48, "Đang mở bán");

            lblMessage.ForeColor = Color.SeaGreen;
            lblMessage.Text = "Đã thêm suất chiếu (dữ liệu mẫu, chưa lưu vào Server).";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            cboMovie.SelectedIndex = -1;
            cboRoom.SelectedIndex = -1;
            dtpStartDate.Value = DateTime.Today;
            dtpStartTime.Value = DateTime.Now;
            nudPrice.Value = 90000;
            lblMessage.Text = "";
            dgvShowtimes.ClearSelection();
        }
    }
}
