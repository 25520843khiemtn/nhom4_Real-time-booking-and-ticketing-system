namespace nhom4_Real_time_booking_and_ticketing_system
{
    partial class FrmShowtime
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picMovie = new System.Windows.Forms.PictureBox();
            this.lblMovieName = new System.Windows.Forms.Label();
            this.lblSelectedDate = new System.Windows.Forms.Label();
            this.lblRoom = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.dtpShowDate = new System.Windows.Forms.DateTimePicker();
            this.flpShowtimes = new System.Windows.Forms.FlowLayoutPanel();
            this.btnShowtime = new System.Windows.Forms.Button();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEndpoint = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picMovie)).BeginInit();
            this.flpShowtimes.SuspendLayout();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // picMovie
            // 
            this.picMovie.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.picMovie.Location = new System.Drawing.Point(40, 32);
            this.picMovie.Name = "picMovie";
            this.picMovie.Size = new System.Drawing.Size(220, 300);
            this.picMovie.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMovie.TabIndex = 0;
            this.picMovie.TabStop = false;
            // 
            // lblMovieName
            // 
            this.lblMovieName.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMovieName.Location = new System.Drawing.Point(300, 32);
            this.lblMovieName.Name = "lblMovieName";
            this.lblMovieName.Size = new System.Drawing.Size(744, 48);
            this.lblMovieName.TabIndex = 1;
            this.lblMovieName.Text = "Tên phim";
            // 
            // lblSelectedDate
            // 
            this.lblSelectedDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSelectedDate.Location = new System.Drawing.Point(300, 92);
            this.lblSelectedDate.Name = "lblSelectedDate";
            this.lblSelectedDate.Size = new System.Drawing.Size(300, 24);
            this.lblSelectedDate.TabIndex = 2;
            this.lblSelectedDate.Text = "Chọn ngày chiếu";
            // 
            // lblRoom
            // 
            this.lblRoom.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoom.Location = new System.Drawing.Point(300, 464);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(744, 28);
            this.lblRoom.TabIndex = 3;
            this.lblRoom.Text = "Phòng 1 - Còn 48 ghế trống";
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Location = new System.Drawing.Point(40, 520);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(140, 40);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "Quay lại";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // dtpShowDate
            // 
            this.dtpShowDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpShowDate.Location = new System.Drawing.Point(300, 120);
            this.dtpShowDate.Name = "dtpShowDate";
            this.dtpShowDate.Size = new System.Drawing.Size(220, 25);
            this.dtpShowDate.TabIndex = 5;
            // 
            // flpShowtimes
            // 
            this.flpShowtimes.AutoScroll = true;
            this.flpShowtimes.Controls.Add(this.btnShowtime);
            this.flpShowtimes.Location = new System.Drawing.Point(300, 172);
            this.flpShowtimes.Name = "flpShowtimes";
            this.flpShowtimes.Size = new System.Drawing.Size(744, 280);
            this.flpShowtimes.TabIndex = 6;
            // 
            // btnShowtime
            // 
            this.btnShowtime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnShowtime.FlatAppearance.BorderSize = 0;
            this.btnShowtime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowtime.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowtime.ForeColor = System.Drawing.Color.White;
            this.btnShowtime.Location = new System.Drawing.Point(8, 8);
            this.btnShowtime.Margin = new System.Windows.Forms.Padding(8);
            this.btnShowtime.Name = "btnShowtime";
            this.btnShowtime.Size = new System.Drawing.Size(120, 48);
            this.btnShowtime.TabIndex = 0;
            this.btnShowtime.Text = "19:00";
            this.btnShowtime.UseVisualStyleBackColor = false;
            // 
            // stsMain
            // 
            this.stsMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslServer,
            this.tsslEndpoint});
            this.stsMain.Location = new System.Drawing.Point(0, 599);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1084, 22);
            this.stsMain.SizingGrip = false;
            this.stsMain.TabIndex = 7;
            this.stsMain.Text = "statusStrip1";
            // 
            // tsslServer
            // 
            this.tsslServer.Name = "tsslServer";
            this.tsslServer.Size = new System.Drawing.Size(74, 17);
            this.tsslServer.Text = "Chưa kết nối";
            // 
            // tsslEndpoint
            // 
            this.tsslEndpoint.Name = "tsslEndpoint";
            this.tsslEndpoint.Size = new System.Drawing.Size(958, 17);
            this.tsslEndpoint.Spring = true;
            this.tsslEndpoint.Text = "127.0.0.1:9000";
            this.tsslEndpoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmShowtime
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1084, 621);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.flpShowtimes);
            this.Controls.Add(this.dtpShowDate);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblRoom);
            this.Controls.Add(this.lblSelectedDate);
            this.Controls.Add(this.lblMovieName);
            this.Controls.Add(this.picMovie);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmShowtime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn suất chiếu";
            ((System.ComponentModel.ISupportInitialize)(this.picMovie)).EndInit();
            this.flpShowtimes.ResumeLayout(false);
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picMovie;
        private System.Windows.Forms.Label lblMovieName;
        private System.Windows.Forms.Label lblSelectedDate;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.DateTimePicker dtpShowDate;
        private System.Windows.Forms.FlowLayoutPanel flpShowtimes;
        private System.Windows.Forms.Button btnShowtime;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslServer;
        private System.Windows.Forms.ToolStripStatusLabel tsslEndpoint;
    }
}