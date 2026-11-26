namespace nhom4_Real_time_booking_and_ticketing_system
{
    partial class FrmSeatBooking
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
            this.components = new System.ComponentModel.Container();
            this.lblShowInfo = new System.Windows.Forms.Label();
            this.lblScreen = new System.Windows.Forms.Label();
            this.lblLegendAvailable = new System.Windows.Forms.Label();
            this.lblLegendMine = new System.Windows.Forms.Label();
            this.lblLegendOther = new System.Windows.Forms.Label();
            this.lblLegendSold = new System.Windows.Forms.Label();
            this.tlpSeatMap = new System.Windows.Forms.TableLayoutPanel();
            this.btnSeat_A1 = new System.Windows.Forms.Button();
            this.btnSeat_A2 = new System.Windows.Forms.Button();
            this.btnSeat_A3 = new System.Windows.Forms.Button();
            this.btnSeat_A4 = new System.Windows.Forms.Button();
            this.pnlLegendAvailable = new System.Windows.Forms.Panel();
            this.pnlLegendMine = new System.Windows.Forms.Panel();
            this.pnlLegendOther = new System.Windows.Forms.Panel();
            this.pnlLegendSold = new System.Windows.Forms.Panel();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblSummaryTitle = new System.Windows.Forms.Label();
            this.lblHeldSeats = new System.Windows.Forms.Label();
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.lblCountdown = new System.Windows.Forms.Label();
            this.btnPayment = new System.Windows.Forms.Button();
            this.btnCancelHold = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.timerHold = new System.Windows.Forms.Timer(this.components);
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEndpoint = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnBack = new System.Windows.Forms.Button();
            this.tlpSeatMap.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblShowInfo
            // 
            this.lblShowInfo.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowInfo.Location = new System.Drawing.Point(32, 16);
            this.lblShowInfo.Name = "lblShowInfo";
            this.lblShowInfo.Size = new System.Drawing.Size(620, 32);
            this.lblShowInfo.TabIndex = 0;
            this.lblShowInfo.Text = "Tên phim - 19:00 - Phòng 1";
            // 
            // lblScreen
            // 
            this.lblScreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblScreen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblScreen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblScreen.Location = new System.Drawing.Point(96, 60);
            this.lblScreen.Name = "lblScreen";
            this.lblScreen.Size = new System.Drawing.Size(640, 28);
            this.lblScreen.TabIndex = 1;
            this.lblScreen.Text = "MÀN HÌNH";
            this.lblScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLegendAvailable
            // 
            this.lblLegendAvailable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLegendAvailable.Location = new System.Drawing.Point(60, 578);
            this.lblLegendAvailable.Name = "lblLegendAvailable";
            this.lblLegendAvailable.Size = new System.Drawing.Size(160, 24);
            this.lblLegendAvailable.TabIndex = 2;
            this.lblLegendAvailable.Text = "Ghế trống";
            this.lblLegendAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLegendMine
            // 
            this.lblLegendMine.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLegendMine.Location = new System.Drawing.Point(252, 578);
            this.lblLegendMine.Name = "lblLegendMine";
            this.lblLegendMine.Size = new System.Drawing.Size(160, 24);
            this.lblLegendMine.TabIndex = 3;
            this.lblLegendMine.Text = "Ghế bạn đang chọn";
            this.lblLegendMine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLegendOther
            // 
            this.lblLegendOther.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLegendOther.Location = new System.Drawing.Point(444, 578);
            this.lblLegendOther.Name = "lblLegendOther";
            this.lblLegendOther.Size = new System.Drawing.Size(160, 24);
            this.lblLegendOther.TabIndex = 4;
            this.lblLegendOther.Text = "Ghế người khác đang giữ";
            this.lblLegendOther.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLegendSold
            // 
            this.lblLegendSold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLegendSold.Location = new System.Drawing.Point(636, 578);
            this.lblLegendSold.Name = "lblLegendSold";
            this.lblLegendSold.Size = new System.Drawing.Size(160, 24);
            this.lblLegendSold.TabIndex = 5;
            this.lblLegendSold.Text = "Ghế đã bán";
            this.lblLegendSold.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpSeatMap
            // 
            this.tlpSeatMap.ColumnCount = 8;
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlpSeatMap.Controls.Add(this.btnSeat_A1, 0, 0);
            this.tlpSeatMap.Controls.Add(this.btnSeat_A2, 1, 0);
            this.tlpSeatMap.Controls.Add(this.btnSeat_A3, 2, 0);
            this.tlpSeatMap.Controls.Add(this.btnSeat_A4, 3, 0);
            this.tlpSeatMap.Location = new System.Drawing.Point(96, 100);
            this.tlpSeatMap.Margin = new System.Windows.Forms.Padding(0);
            this.tlpSeatMap.Name = "tlpSeatMap";
            this.tlpSeatMap.RowCount = 6;
            this.tlpSeatMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpSeatMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpSeatMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpSeatMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpSeatMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpSeatMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpSeatMap.Size = new System.Drawing.Size(640, 384);
            this.tlpSeatMap.TabIndex = 6;
            // 
            // btnSeat_A1
            // 
            this.btnSeat_A1.BackColor = System.Drawing.Color.White;
            this.btnSeat_A1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSeat_A1.FlatAppearance.BorderSize = 0;
            this.btnSeat_A1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeat_A1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeat_A1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.btnSeat_A1.Location = new System.Drawing.Point(4, 4);
            this.btnSeat_A1.Margin = new System.Windows.Forms.Padding(4);
            this.btnSeat_A1.Name = "btnSeat_A1";
            this.btnSeat_A1.Size = new System.Drawing.Size(72, 56);
            this.btnSeat_A1.TabIndex = 0;
            this.btnSeat_A1.Text = "A1";
            this.btnSeat_A1.UseVisualStyleBackColor = false;
            // 
            // btnSeat_A2
            // 
            this.btnSeat_A2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSeat_A2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSeat_A2.FlatAppearance.BorderSize = 0;
            this.btnSeat_A2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeat_A2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeat_A2.ForeColor = System.Drawing.Color.White;
            this.btnSeat_A2.Location = new System.Drawing.Point(84, 4);
            this.btnSeat_A2.Margin = new System.Windows.Forms.Padding(4);
            this.btnSeat_A2.Name = "btnSeat_A2";
            this.btnSeat_A2.Size = new System.Drawing.Size(72, 56);
            this.btnSeat_A2.TabIndex = 1;
            this.btnSeat_A2.Text = "A2";
            this.btnSeat_A2.UseVisualStyleBackColor = false;
            // 
            // btnSeat_A3
            // 
            this.btnSeat_A3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(179)))), ((int)(((byte)(8)))));
            this.btnSeat_A3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSeat_A3.FlatAppearance.BorderSize = 0;
            this.btnSeat_A3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeat_A3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeat_A3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.btnSeat_A3.Location = new System.Drawing.Point(164, 4);
            this.btnSeat_A3.Margin = new System.Windows.Forms.Padding(4);
            this.btnSeat_A3.Name = "btnSeat_A3";
            this.btnSeat_A3.Size = new System.Drawing.Size(72, 56);
            this.btnSeat_A3.TabIndex = 2;
            this.btnSeat_A3.Text = "A3";
            this.btnSeat_A3.UseVisualStyleBackColor = false;
            // 
            // btnSeat_A4
            // 
            this.btnSeat_A4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.btnSeat_A4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSeat_A4.FlatAppearance.BorderSize = 0;
            this.btnSeat_A4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSeat_A4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeat_A4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.btnSeat_A4.Location = new System.Drawing.Point(244, 4);
            this.btnSeat_A4.Margin = new System.Windows.Forms.Padding(4);
            this.btnSeat_A4.Name = "btnSeat_A4";
            this.btnSeat_A4.Size = new System.Drawing.Size(72, 56);
            this.btnSeat_A4.TabIndex = 3;
            this.btnSeat_A4.Text = "A4";
            this.btnSeat_A4.UseVisualStyleBackColor = false;
            // 
            // pnlLegendAvailable
            // 
            this.pnlLegendAvailable.BackColor = System.Drawing.Color.White;
            this.pnlLegendAvailable.Location = new System.Drawing.Point(32, 580);
            this.pnlLegendAvailable.Name = "pnlLegendAvailable";
            this.pnlLegendAvailable.Size = new System.Drawing.Size(20, 20);
            this.pnlLegendAvailable.TabIndex = 7;
            // 
            // pnlLegendMine
            // 
            this.pnlLegendMine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.pnlLegendMine.Location = new System.Drawing.Point(224, 580);
            this.pnlLegendMine.Name = "pnlLegendMine";
            this.pnlLegendMine.Size = new System.Drawing.Size(20, 20);
            this.pnlLegendMine.TabIndex = 8;
            // 
            // pnlLegendOther
            // 
            this.pnlLegendOther.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(179)))), ((int)(((byte)(8)))));
            this.pnlLegendOther.Location = new System.Drawing.Point(416, 580);
            this.pnlLegendOther.Name = "pnlLegendOther";
            this.pnlLegendOther.Size = new System.Drawing.Size(20, 20);
            this.pnlLegendOther.TabIndex = 9;
            // 
            // pnlLegendSold
            // 
            this.pnlLegendSold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.pnlLegendSold.Location = new System.Drawing.Point(608, 580);
            this.pnlLegendSold.Name = "pnlLegendSold";
            this.pnlLegendSold.Size = new System.Drawing.Size(20, 20);
            this.pnlLegendSold.TabIndex = 10;
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.pnlSummary.Controls.Add(this.lblMessage);
            this.pnlSummary.Controls.Add(this.btnCancelHold);
            this.pnlSummary.Controls.Add(this.btnPayment);
            this.pnlSummary.Controls.Add(this.lblCountdown);
            this.pnlSummary.Controls.Add(this.lblTotalPrice);
            this.pnlSummary.Controls.Add(this.lblHeldSeats);
            this.pnlSummary.Controls.Add(this.lblSummaryTitle);
            this.pnlSummary.Location = new System.Drawing.Point(832, 24);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(320, 580);
            this.pnlSummary.TabIndex = 11;
            // 
            // lblSummaryTitle
            // 
            this.lblSummaryTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummaryTitle.Location = new System.Drawing.Point(24, 24);
            this.lblSummaryTitle.Name = "lblSummaryTitle";
            this.lblSummaryTitle.Size = new System.Drawing.Size(272, 32);
            this.lblSummaryTitle.TabIndex = 0;
            this.lblSummaryTitle.Text = "Thông tin đặt vé";
            // 
            // lblHeldSeats
            // 
            this.lblHeldSeats.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeldSeats.Location = new System.Drawing.Point(24, 76);
            this.lblHeldSeats.Name = "lblHeldSeats";
            this.lblHeldSeats.Size = new System.Drawing.Size(272, 100);
            this.lblHeldSeats.TabIndex = 1;
            this.lblHeldSeats.Text = "Ghế đang chọn: A2";
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPrice.Location = new System.Drawing.Point(24, 188);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(272, 32);
            this.lblTotalPrice.TabIndex = 2;
            this.lblTotalPrice.Text = "Tổng tiền: 90.000 đ";
            // 
            // lblCountdown
            // 
            this.lblCountdown.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCountdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(165)))), ((int)(((byte)(250)))));
            this.lblCountdown.Location = new System.Drawing.Point(24, 236);
            this.lblCountdown.Name = "lblCountdown";
            this.lblCountdown.Size = new System.Drawing.Size(272, 48);
            this.lblCountdown.TabIndex = 3;
            this.lblCountdown.Text = "Ghế giữ còn: 05:00";
            this.lblCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPayment
            // 
            this.btnPayment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPayment.FlatAppearance.BorderSize = 0;
            this.btnPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPayment.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPayment.ForeColor = System.Drawing.Color.White;
            this.btnPayment.Location = new System.Drawing.Point(24, 330);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(272, 46);
            this.btnPayment.TabIndex = 4;
            this.btnPayment.Text = "Thanh toán";
            this.btnPayment.UseVisualStyleBackColor = false;
            // 
            // btnCancelHold
            // 
            this.btnCancelHold.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnCancelHold.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelHold.Location = new System.Drawing.Point(24, 388);
            this.btnCancelHold.Name = "btnCancelHold";
            this.btnCancelHold.Size = new System.Drawing.Size(272, 42);
            this.btnCancelHold.TabIndex = 5;
            this.btnCancelHold.Text = "Hủy giữ ghế";
            this.btnCancelHold.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblMessage.Location = new System.Drawing.Point(24, 452);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(272, 100);
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "Thông báo từ Server";
            // 
            // timerHold
            // 
            this.timerHold.Interval = 1000;
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
            this.stsMain.TabIndex = 12;
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
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Location = new System.Drawing.Point(672, 14);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(128, 36);
            this.btnBack.TabIndex = 13;
            this.btnBack.Text = "Quay lại";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // FrmSeatBooking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1184, 621);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlLegendSold);
            this.Controls.Add(this.pnlLegendOther);
            this.Controls.Add(this.pnlLegendMine);
            this.Controls.Add(this.pnlLegendAvailable);
            this.Controls.Add(this.tlpSeatMap);
            this.Controls.Add(this.lblLegendSold);
            this.Controls.Add(this.lblLegendOther);
            this.Controls.Add(this.lblLegendMine);
            this.Controls.Add(this.lblLegendAvailable);
            this.Controls.Add(this.lblScreen);
            this.Controls.Add(this.lblShowInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmSeatBooking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn ghế và thanh toán";
            this.tlpSeatMap.ResumeLayout(false);
            this.pnlSummary.ResumeLayout(false);
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblShowInfo;
        private System.Windows.Forms.Label lblScreen;
        private System.Windows.Forms.Label lblLegendAvailable;
        private System.Windows.Forms.Label lblLegendMine;
        private System.Windows.Forms.Label lblLegendOther;
        private System.Windows.Forms.Label lblLegendSold;
        private System.Windows.Forms.TableLayoutPanel tlpSeatMap;
        private System.Windows.Forms.Button btnSeat_A1;
        private System.Windows.Forms.Button btnSeat_A2;
        private System.Windows.Forms.Button btnSeat_A3;
        private System.Windows.Forms.Button btnSeat_A4;
        private System.Windows.Forms.Panel pnlLegendAvailable;
        private System.Windows.Forms.Panel pnlLegendMine;
        private System.Windows.Forms.Panel pnlLegendOther;
        private System.Windows.Forms.Panel pnlLegendSold;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblTotalPrice;
        private System.Windows.Forms.Label lblHeldSeats;
        private System.Windows.Forms.Label lblSummaryTitle;
        private System.Windows.Forms.Button btnPayment;
        private System.Windows.Forms.Label lblCountdown;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnCancelHold;
        private System.Windows.Forms.Timer timerHold;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslServer;
        private System.Windows.Forms.ToolStripStatusLabel tsslEndpoint;
        private System.Windows.Forms.Button btnBack;
    }
}