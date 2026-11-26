namespace nhom4_Real_time_booking_and_ticketing_system
{
    partial class FrmMovieList
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.flpMovielist = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlMoviecard = new System.Windows.Forms.Panel();
            this.btnBookTicket = new System.Windows.Forms.Button();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblMovieName = new System.Windows.Forms.Label();
            this.picMovie = new System.Windows.Forms.PictureBox();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEndpoint = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnHistory = new System.Windows.Forms.Button();
            this.flpMovielist.SuspendLayout();
            this.pnlMoviecard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMovie)).BeginInit();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(32, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(440, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Phim đang chiếu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnLogout.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Location = new System.Drawing.Point(1032, 16);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(120, 40);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = false;
            // 
            // flpMovielist
            // 
            this.flpMovielist.Controls.Add(this.pnlMoviecard);
            this.flpMovielist.Location = new System.Drawing.Point(32, 76);
            this.flpMovielist.Name = "flpMovielist";
            this.flpMovielist.Size = new System.Drawing.Size(1120, 500);
            this.flpMovielist.TabIndex = 2;
            // 
            // pnlMoviecard
            // 
            this.pnlMoviecard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.pnlMoviecard.Controls.Add(this.btnBookTicket);
            this.pnlMoviecard.Controls.Add(this.lblDuration);
            this.pnlMoviecard.Controls.Add(this.lblMovieName);
            this.pnlMoviecard.Controls.Add(this.picMovie);
            this.pnlMoviecard.Location = new System.Drawing.Point(8, 8);
            this.pnlMoviecard.Margin = new System.Windows.Forms.Padding(8);
            this.pnlMoviecard.Name = "pnlMoviecard";
            this.pnlMoviecard.Size = new System.Drawing.Size(340, 440);
            this.pnlMoviecard.TabIndex = 0;
            // 
            // btnBookTicket
            // 
            this.btnBookTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnBookTicket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBookTicket.FlatAppearance.BorderSize = 0;
            this.btnBookTicket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBookTicket.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBookTicket.ForeColor = System.Drawing.Color.White;
            this.btnBookTicket.Location = new System.Drawing.Point(25, 376);
            this.btnBookTicket.Name = "btnBookTicket";
            this.btnBookTicket.Size = new System.Drawing.Size(290, 42);
            this.btnBookTicket.TabIndex = 3;
            this.btnBookTicket.Text = "Đặt vé";
            this.btnBookTicket.UseVisualStyleBackColor = false;
            // 
            // lblDuration
            // 
            this.lblDuration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblDuration.Location = new System.Drawing.Point(25, 338);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(290, 24);
            this.lblDuration.TabIndex = 2;
            this.lblDuration.Text = "Thời lượng: 120 phút";
            // 
            // lblMovieName
            // 
            this.lblMovieName.AutoEllipsis = true;
            this.lblMovieName.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMovieName.Location = new System.Drawing.Point(25, 302);
            this.lblMovieName.Name = "lblMovieName";
            this.lblMovieName.Size = new System.Drawing.Size(290, 32);
            this.lblMovieName.TabIndex = 1;
            this.lblMovieName.Text = "Tên phim";
            // 
            // picMovie
            // 
            this.picMovie.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.picMovie.Location = new System.Drawing.Point(25, 20);
            this.picMovie.Name = "picMovie";
            this.picMovie.Size = new System.Drawing.Size(290, 270);
            this.picMovie.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMovie.TabIndex = 0;
            this.picMovie.TabStop = false;
            // 
            // stsMain
            // 
            this.stsMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslServer,
            this.tsslEndpoint});
            this.stsMain.Location = new System.Drawing.Point(0, 599);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1184, 22);
            this.stsMain.SizingGrip = false;
            this.stsMain.TabIndex = 3;
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
            this.tsslEndpoint.Size = new System.Drawing.Size(1095, 17);
            this.tsslEndpoint.Spring = true;
            this.tsslEndpoint.Text = "127.0.0.1:9000";
            this.tsslEndpoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.Location = new System.Drawing.Point(510, 20);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(310, 32);
            this.lblWelcome.TabIndex = 4;
            this.lblWelcome.Text = "Xin chào, Người dùng";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnHistory
            // 
            this.btnHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnHistory.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistory.Location = new System.Drawing.Point(864, 16);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(156, 40);
            this.btnHistory.TabIndex = 5;
            this.btnHistory.Text = "Lịch sử mua vé";
            this.btnHistory.UseVisualStyleBackColor = false;
            // 
            // FrmMovieList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1184, 621);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.flpMovielist);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmMovieList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách phim";
            this.flpMovielist.ResumeLayout(false);
            this.pnlMoviecard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMovie)).EndInit();
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.FlowLayoutPanel flpMovielist;
        private System.Windows.Forms.Panel pnlMoviecard;
        private System.Windows.Forms.Label lblMovieName;
        private System.Windows.Forms.PictureBox picMovie;
        private System.Windows.Forms.Button btnBookTicket;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslServer;
        private System.Windows.Forms.ToolStripStatusLabel tsslEndpoint;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnHistory;
    }
}