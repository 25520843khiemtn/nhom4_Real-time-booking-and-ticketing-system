namespace TicketBooking.Server
{
    partial class FrmShowtimeAdmin
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cboFilterMovie = new System.Windows.Forms.ComboBox();
            this.dtpFilterDate = new System.Windows.Forms.DateTimePicker();
            this.cboFilterRoom = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvShowtimes = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMovie = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRoom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFreeSeats = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlEdit = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.nudPrice = new System.Windows.Forms.NumericUpDown();
            this.lblPrice = new System.Windows.Forms.Label();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.cboRoom = new System.Windows.Forms.ComboBox();
            this.lblRoom = new System.Windows.Forms.Label();
            this.lblMovie = new System.Windows.Forms.Label();
            this.cboMovie = new System.Windows.Forms.ComboBox();
            this.lblEditTitle = new System.Windows.Forms.Label();
            this.lblFilterMovie = new System.Windows.Forms.Label();
            this.lblFilterDate = new System.Windows.Forms.Label();
            this.lblFilterRoom = new System.Windows.Forms.Label();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEndpoint = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShowtimes)).BeginInit();
            this.pnlEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(24, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý suất chiếu";
            // 
            // cboFilterMovie
            // 
            this.cboFilterMovie.BackColor = System.Drawing.Color.White;
            this.cboFilterMovie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterMovie.ForeColor = System.Drawing.Color.Black;
            this.cboFilterMovie.FormattingEnabled = true;
            this.cboFilterMovie.Location = new System.Drawing.Point(24, 80);
            this.cboFilterMovie.Name = "cboFilterMovie";
            this.cboFilterMovie.Size = new System.Drawing.Size(220, 25);
            this.cboFilterMovie.TabIndex = 1;
            // 
            // dtpFilterDate
            // 
            this.dtpFilterDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFilterDate.Location = new System.Drawing.Point(256, 80);
            this.dtpFilterDate.Name = "dtpFilterDate";
            this.dtpFilterDate.Size = new System.Drawing.Size(160, 25);
            this.dtpFilterDate.TabIndex = 2;
            // 
            // cboFilterRoom
            // 
            this.cboFilterRoom.BackColor = System.Drawing.Color.White;
            this.cboFilterRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterRoom.ForeColor = System.Drawing.Color.Black;
            this.cboFilterRoom.FormattingEnabled = true;
            this.cboFilterRoom.Location = new System.Drawing.Point(428, 80);
            this.cboFilterRoom.Name = "cboFilterRoom";
            this.cboFilterRoom.Size = new System.Drawing.Size(160, 25);
            this.cboFilterRoom.TabIndex = 3;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(600, 76);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 32);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Tải lại";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // dgvShowtimes
            // 
            this.dgvShowtimes.AllowUserToAddRows = false;
            this.dgvShowtimes.AllowUserToDeleteRows = false;
            this.dgvShowtimes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvShowtimes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvShowtimes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvShowtimes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvShowtimes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colMovie,
            this.colRoom,
            this.colStartTime,
            this.colPrice,
            this.colFreeSeats,
            this.colStatus});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvShowtimes.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvShowtimes.EnableHeadersVisualStyles = false;
            this.dgvShowtimes.Location = new System.Drawing.Point(24, 110);
            this.dgvShowtimes.MultiSelect = false;
            this.dgvShowtimes.Name = "dgvShowtimes";
            this.dgvShowtimes.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvShowtimes.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvShowtimes.RowHeadersVisible = false;
            this.dgvShowtimes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvShowtimes.Size = new System.Drawing.Size(740, 400);
            this.dgvShowtimes.TabIndex = 5;
            this.dgvShowtimes.SelectionChanged += new System.EventHandler(this.dgvShowtimes_SelectionChanged);
            // 
            // colId
            // 
            this.colId.HeaderText = "Mã";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            // 
            // colMovie
            // 
            this.colMovie.HeaderText = "Phim";
            this.colMovie.Name = "colMovie";
            this.colMovie.ReadOnly = true;
            // 
            // colRoom
            // 
            this.colRoom.HeaderText = "Phòng";
            this.colRoom.Name = "colRoom";
            this.colRoom.ReadOnly = true;
            // 
            // colStartTime
            // 
            this.colStartTime.HeaderText = "Giờ chiếu";
            this.colStartTime.Name = "colStartTime";
            this.colStartTime.ReadOnly = true;
            // 
            // colPrice
            // 
            this.colPrice.HeaderText = "Giá vé";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            // 
            // colFreeSeats
            // 
            this.colFreeSeats.HeaderText = "Ghế trống";
            this.colFreeSeats.Name = "colFreeSeats";
            this.colFreeSeats.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Trạng thái";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // pnlEdit
            // 
            this.pnlEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.pnlEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEdit.Controls.Add(this.lblMessage);
            this.pnlEdit.Controls.Add(this.btnClear);
            this.pnlEdit.Controls.Add(this.btnDelete);
            this.pnlEdit.Controls.Add(this.btnUpdate);
            this.pnlEdit.Controls.Add(this.btnAdd);
            this.pnlEdit.Controls.Add(this.nudPrice);
            this.pnlEdit.Controls.Add(this.lblPrice);
            this.pnlEdit.Controls.Add(this.dtpStartTime);
            this.pnlEdit.Controls.Add(this.dtpStartDate);
            this.pnlEdit.Controls.Add(this.lblStartDate);
            this.pnlEdit.Controls.Add(this.lblStartTime);
            this.pnlEdit.Controls.Add(this.cboRoom);
            this.pnlEdit.Controls.Add(this.lblRoom);
            this.pnlEdit.Controls.Add(this.lblMovie);
            this.pnlEdit.Controls.Add(this.cboMovie);
            this.pnlEdit.Controls.Add(this.lblEditTitle);
            this.pnlEdit.Location = new System.Drawing.Point(788, 64);
            this.pnlEdit.Name = "pnlEdit";
            this.pnlEdit.Size = new System.Drawing.Size(372, 446);
            this.pnlEdit.TabIndex = 6;
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblMessage.Location = new System.Drawing.Point(16, 402);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(340, 28);
            this.lblMessage.TabIndex = 15;
            this.lblMessage.Text = "Chọn một suất chiếu trong danh sách để xem / sửa";
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(280, 354);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(76, 36);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Làm mới";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(192, 354);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 36);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(104, 354);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(80, 36);
            this.btnUpdate.TabIndex = 12;
            this.btnUpdate.Text = "Sửa";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(16, 354);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(80, 36);
            this.btnAdd.TabIndex = 11;
            this.btnAdd.Text = "Thêm";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // nudPrice
            // 
            this.nudPrice.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudPrice.Location = new System.Drawing.Point(16, 306);
            this.nudPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudPrice.Name = "nudPrice";
            this.nudPrice.Size = new System.Drawing.Size(340, 25);
            this.nudPrice.TabIndex = 10;
            this.nudPrice.ThousandsSeparator = true;
            this.nudPrice.Value = new decimal(new int[] {
            90000,
            0,
            0,
            0});
            // 
            // lblPrice
            // 
            this.lblPrice.Location = new System.Drawing.Point(16, 284);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(340, 18);
            this.lblPrice.TabIndex = 9;
            this.lblPrice.Text = "Giá vé (đồng)";
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "HH:mm";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(16, 248);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(160, 25);
            this.dtpStartTime.TabIndex = 8;
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(16, 190);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(160, 25);
            this.dtpStartDate.TabIndex = 7;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(16, 168);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(77, 19);
            this.lblStartDate.TabIndex = 6;
            this.lblStartDate.Text = "Ngày chiếu";
            // 
            // lblStartTime
            // 
            this.lblStartTime.Location = new System.Drawing.Point(16, 226);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(340, 18);
            this.lblStartTime.TabIndex = 5;
            this.lblStartTime.Text = "Giờ chiếu";
            // 
            // cboRoom
            // 
            this.cboRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoom.FormattingEnabled = true;
            this.cboRoom.Location = new System.Drawing.Point(16, 132);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Size = new System.Drawing.Size(340, 25);
            this.cboRoom.TabIndex = 4;
            // 
            // lblRoom
            // 
            this.lblRoom.Location = new System.Drawing.Point(16, 110);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(340, 18);
            this.lblRoom.TabIndex = 3;
            this.lblRoom.Text = "Phòng chiếu";
            // 
            // lblMovie
            // 
            this.lblMovie.Location = new System.Drawing.Point(16, 52);
            this.lblMovie.Name = "lblMovie";
            this.lblMovie.Size = new System.Drawing.Size(340, 18);
            this.lblMovie.TabIndex = 2;
            this.lblMovie.Text = "Phim";
            // 
            // cboMovie
            // 
            this.cboMovie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMovie.FormattingEnabled = true;
            this.cboMovie.Location = new System.Drawing.Point(16, 74);
            this.cboMovie.Name = "cboMovie";
            this.cboMovie.Size = new System.Drawing.Size(340, 25);
            this.cboMovie.TabIndex = 1;
            // 
            // lblEditTitle
            // 
            this.lblEditTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditTitle.Location = new System.Drawing.Point(16, 22);
            this.lblEditTitle.Name = "lblEditTitle";
            this.lblEditTitle.Size = new System.Drawing.Size(340, 28);
            this.lblEditTitle.TabIndex = 0;
            this.lblEditTitle.Text = "THÔNG TIN SUẤT CHIẾU";
            // 
            // lblFilterMovie
            // 
            this.lblFilterMovie.Location = new System.Drawing.Point(24, 58);
            this.lblFilterMovie.Name = "lblFilterMovie";
            this.lblFilterMovie.Size = new System.Drawing.Size(220, 18);
            this.lblFilterMovie.TabIndex = 7;
            this.lblFilterMovie.Text = "Phim";
            // 
            // lblFilterDate
            // 
            this.lblFilterDate.Location = new System.Drawing.Point(256, 58);
            this.lblFilterDate.Name = "lblFilterDate";
            this.lblFilterDate.Size = new System.Drawing.Size(160, 18);
            this.lblFilterDate.TabIndex = 8;
            this.lblFilterDate.Text = "Ngày chiếu";
            // 
            // lblFilterRoom
            // 
            this.lblFilterRoom.Location = new System.Drawing.Point(428, 58);
            this.lblFilterRoom.Name = "lblFilterRoom";
            this.lblFilterRoom.Size = new System.Drawing.Size(160, 18);
            this.lblFilterRoom.TabIndex = 9;
            this.lblFilterRoom.Text = "Phòng chiếu";
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
            this.stsMain.TabIndex = 10;
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
            // FrmShowtimeAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1184, 621);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.lblFilterRoom);
            this.Controls.Add(this.lblFilterDate);
            this.Controls.Add(this.lblFilterMovie);
            this.Controls.Add(this.pnlEdit);
            this.Controls.Add(this.dgvShowtimes);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cboFilterRoom);
            this.Controls.Add(this.dtpFilterDate);
            this.Controls.Add(this.cboFilterMovie);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmShowtimeAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý suất chiếu";
            this.Load += new System.EventHandler(this.FrmShowtimeAdmin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvShowtimes)).EndInit();
            this.pnlEdit.ResumeLayout(false);
            this.pnlEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox cboFilterMovie;
        private System.Windows.Forms.DateTimePicker dtpFilterDate;
        private System.Windows.Forms.ComboBox cboFilterRoom;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgvShowtimes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMovie;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoom;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFreeSeats;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Panel pnlEdit;
        private System.Windows.Forms.ComboBox cboMovie;
        private System.Windows.Forms.Label lblEditTitle;
        private System.Windows.Forms.Label lblFilterMovie;
        private System.Windows.Forms.Label lblFilterDate;
        private System.Windows.Forms.Label lblFilterRoom;
        private System.Windows.Forms.Label lblMovie;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslServer;
        private System.Windows.Forms.ToolStripStatusLabel tsslEndpoint;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.ComboBox cboRoom;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.NumericUpDown nudPrice;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnAdd;
    }
}