namespace nhom4_Real_time_booking_and_ticketing_system
{
    partial class FrmAI
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlChatHistory = new System.Windows.Forms.Panel();
            this.flpChatMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlAIMessage = new System.Windows.Forms.Panel();
            this.lblAIMessage = new System.Windows.Forms.Label();
            this.lblAIName = new System.Windows.Forms.Label();
            this.pnlUserMessage = new System.Windows.Forms.Panel();
            this.lblUserMessage = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblQuickQuestions = new System.Windows.Forms.Label();
            this.flpQuickQuestions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnHotMovies = new System.Windows.Forms.Button();
            this.btnFamilyMovies = new System.Windows.Forms.Button();
            this.btnTonightShowtimes = new System.Windows.Forms.Button();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslAIStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlHeader.SuspendLayout();
            this.pnlChatHistory.SuspendLayout();
            this.flpChatMessages.SuspendLayout();
            this.pnlAIMessage.SuspendLayout();
            this.pnlUserMessage.SuspendLayout();
            this.flpQuickQuestions.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.pnlHeader.Controls.Add(this.btnBack);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1184, 85);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(70)))), ((int)(((byte)(106)))));
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(984, 13);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(140, 42);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Quay lại";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSubtitle.Location = new System.Drawing.Point(22, 40);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(440, 20);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Movie Assistant - Hỗ trợ đặt vé xem phim";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(240, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TRỢ LÝ AI";
            // 
            // pnlChatHistory
            // 
            this.pnlChatHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChatHistory.Controls.Add(this.flpChatMessages);
            this.pnlChatHistory.Controls.Add(this.lblWelcome);
            this.pnlChatHistory.Location = new System.Drawing.Point(32, 96);
            this.pnlChatHistory.Name = "pnlChatHistory";
            this.pnlChatHistory.Size = new System.Drawing.Size(1120, 350);
            this.pnlChatHistory.TabIndex = 1;
            // 
            // flpChatMessages
            // 
            this.flpChatMessages.AutoScroll = true;
            this.flpChatMessages.Controls.Add(this.pnlAIMessage);
            this.flpChatMessages.Controls.Add(this.pnlUserMessage);
            this.flpChatMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpChatMessages.Location = new System.Drawing.Point(16, 54);
            this.flpChatMessages.Name = "flpChatMessages";
            this.flpChatMessages.Size = new System.Drawing.Size(1088, 280);
            this.flpChatMessages.TabIndex = 1;
            this.flpChatMessages.WrapContents = false;
            // 
            // pnlAIMessage
            // 
            this.pnlAIMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.pnlAIMessage.Controls.Add(this.lblAIMessage);
            this.pnlAIMessage.Controls.Add(this.lblAIName);
            this.pnlAIMessage.Location = new System.Drawing.Point(8, 8);
            this.pnlAIMessage.Margin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.pnlAIMessage.Name = "pnlAIMessage";
            this.pnlAIMessage.Size = new System.Drawing.Size(780, 90);
            this.pnlAIMessage.TabIndex = 0;
            // 
            // lblAIMessage
            // 
            this.lblAIMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblAIMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAIMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblAIMessage.Location = new System.Drawing.Point(14, 32);
            this.lblAIMessage.Name = "lblAIMessage";
            this.lblAIMessage.Size = new System.Drawing.Size(748, 48);
            this.lblAIMessage.TabIndex = 2;
            this.lblAIMessage.Text = "Xin chào! Bạn muốn tìm phim nào để xem hôm nay? Mình có thể gợi ý phim phù hợp vớ" +
    "i bạn.";
            // 
            // lblAIName
            // 
            this.lblAIName.BackColor = System.Drawing.Color.Transparent;
            this.lblAIName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAIName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(197)))), ((int)(((byte)(253)))));
            this.lblAIName.Location = new System.Drawing.Point(14, 8);
            this.lblAIName.Name = "lblAIName";
            this.lblAIName.Size = new System.Drawing.Size(740, 20);
            this.lblAIName.TabIndex = 1;
            this.lblAIName.Text = "TRỢ LÝ AI";
            // 
            // pnlUserMessage
            // 
            this.pnlUserMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(70)))), ((int)(((byte)(106)))));
            this.pnlUserMessage.Controls.Add(this.lblUserMessage);
            this.pnlUserMessage.Controls.Add(this.lblUserName);
            this.pnlUserMessage.Location = new System.Drawing.Point(380, 110);
            this.pnlUserMessage.Margin = new System.Windows.Forms.Padding(380, 12, 8, 0);
            this.pnlUserMessage.Name = "pnlUserMessage";
            this.pnlUserMessage.Size = new System.Drawing.Size(700, 68);
            this.pnlUserMessage.TabIndex = 1;
            // 
            // lblUserMessage
            // 
            this.lblUserMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblUserMessage.ForeColor = System.Drawing.Color.White;
            this.lblUserMessage.Location = new System.Drawing.Point(14, 30);
            this.lblUserMessage.Name = "lblUserMessage";
            this.lblUserMessage.Size = new System.Drawing.Size(672, 28);
            this.lblUserMessage.TabIndex = 1;
            this.lblUserMessage.Text = "Mình muốn tìm phim hay để xem tối nay.";
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.lblUserName.Location = new System.Drawing.Point(14, 6);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(660, 20);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "BẠN";
            // 
            // lblWelcome
            // 
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Location = new System.Drawing.Point(20, 12);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(1080, 32);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Xin chào! Tôi có thể giúp gì cho bạn?";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblQuickQuestions
            // 
            this.lblQuickQuestions.BackColor = System.Drawing.Color.Transparent;
            this.lblQuickQuestions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuickQuestions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.lblQuickQuestions.Location = new System.Drawing.Point(32, 458);
            this.lblQuickQuestions.Name = "lblQuickQuestions";
            this.lblQuickQuestions.Size = new System.Drawing.Size(1120, 22);
            this.lblQuickQuestions.TabIndex = 2;
            this.lblQuickQuestions.Text = "GỢI Ý CÂU HỎI";
            // 
            // flpQuickQuestions
            // 
            this.flpQuickQuestions.BackColor = System.Drawing.Color.Transparent;
            this.flpQuickQuestions.Controls.Add(this.btnHotMovies);
            this.flpQuickQuestions.Controls.Add(this.btnFamilyMovies);
            this.flpQuickQuestions.Controls.Add(this.btnTonightShowtimes);
            this.flpQuickQuestions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flpQuickQuestions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.flpQuickQuestions.Location = new System.Drawing.Point(32, 482);
            this.flpQuickQuestions.Name = "flpQuickQuestions";
            this.flpQuickQuestions.Size = new System.Drawing.Size(1120, 42);
            this.flpQuickQuestions.TabIndex = 3;
            this.flpQuickQuestions.WrapContents = false;
            // 
            // btnHotMovies
            // 
            this.btnHotMovies.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnHotMovies.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHotMovies.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHotMovies.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnHotMovies.ForeColor = System.Drawing.Color.White;
            this.btnHotMovies.Location = new System.Drawing.Point(0, 0);
            this.btnHotMovies.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnHotMovies.Name = "btnHotMovies";
            this.btnHotMovies.Size = new System.Drawing.Size(210, 36);
            this.btnHotMovies.TabIndex = 0;
            this.btnHotMovies.Text = "Phim nào đang hot?";
            this.btnHotMovies.UseVisualStyleBackColor = false;
            // 
            // btnFamilyMovies
            // 
            this.btnFamilyMovies.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnFamilyMovies.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFamilyMovies.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFamilyMovies.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFamilyMovies.ForeColor = System.Drawing.Color.White;
            this.btnFamilyMovies.Location = new System.Drawing.Point(222, 0);
            this.btnFamilyMovies.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnFamilyMovies.Name = "btnFamilyMovies";
            this.btnFamilyMovies.Size = new System.Drawing.Size(210, 36);
            this.btnFamilyMovies.TabIndex = 1;
            this.btnFamilyMovies.Text = "Phim hài/gia đình";
            this.btnFamilyMovies.UseVisualStyleBackColor = false;
            // 
            // btnTonightShowtimes
            // 
            this.btnTonightShowtimes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.btnTonightShowtimes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTonightShowtimes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTonightShowtimes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTonightShowtimes.ForeColor = System.Drawing.Color.White;
            this.btnTonightShowtimes.Location = new System.Drawing.Point(444, 0);
            this.btnTonightShowtimes.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnTonightShowtimes.Name = "btnTonightShowtimes";
            this.btnTonightShowtimes.Size = new System.Drawing.Size(210, 36);
            this.btnTonightShowtimes.TabIndex = 2;
            this.btnTonightShowtimes.Text = "Suất chiếu tối nay";
            this.btnTonightShowtimes.UseVisualStyleBackColor = false;
            // 
            // pnlInput
            // 
            this.pnlInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.pnlInput.Controls.Add(this.btnSend);
            this.pnlInput.Controls.Add(this.txtMessage);
            this.pnlInput.Location = new System.Drawing.Point(32, 530);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Size = new System.Drawing.Size(1120, 58);
            this.pnlInput.TabIndex = 4;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(966, 8);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(140, 42);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = false;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.ForeColor = System.Drawing.Color.White;
            this.txtMessage.Location = new System.Drawing.Point(12, 10);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(940, 25);
            this.txtMessage.TabIndex = 0;
            // 
            // stsMain
            // 
            this.stsMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(36)))), ((int)(((byte)(56)))));
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tsslAIStatus});
            this.stsMain.Location = new System.Drawing.Point(0, 639);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1184, 22);
            this.stsMain.SizingGrip = false;
            this.stsMain.TabIndex = 5;
            this.stsMain.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.ForeColor = System.Drawing.Color.White;
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(54, 17);
            this.tsslStatus.Text = "Sẵn sàng";
            // 
            // tsslAIStatus
            // 
            this.tsslAIStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.tsslAIStatus.Name = "tsslAIStatus";
            this.tsslAIStatus.Size = new System.Drawing.Size(1115, 17);
            this.tsslAIStatus.Spring = true;
            this.tsslAIStatus.Text = "Trợ lý AI chưa kết nối";
            this.tsslAIStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.pnlInput);
            this.Controls.Add(this.flpQuickQuestions);
            this.Controls.Add(this.lblQuickQuestions);
            this.Controls.Add(this.pnlChatHistory);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmAI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trợ lý AI - Movie Assistant";
            this.pnlHeader.ResumeLayout(false);
            this.pnlChatHistory.ResumeLayout(false);
            this.flpChatMessages.ResumeLayout(false);
            this.pnlAIMessage.ResumeLayout(false);
            this.pnlUserMessage.ResumeLayout(false);
            this.flpQuickQuestions.ResumeLayout(false);
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel pnlChatHistory;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.FlowLayoutPanel flpChatMessages;
        private System.Windows.Forms.Panel pnlAIMessage;
        private System.Windows.Forms.Label lblAIMessage;
        private System.Windows.Forms.Label lblAIName;
        private System.Windows.Forms.Panel pnlUserMessage;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserMessage;
        private System.Windows.Forms.Label lblQuickQuestions;
        private System.Windows.Forms.FlowLayoutPanel flpQuickQuestions;
        private System.Windows.Forms.Button btnHotMovies;
        private System.Windows.Forms.Button btnFamilyMovies;
        private System.Windows.Forms.Button btnTonightShowtimes;
        private System.Windows.Forms.Panel pnlInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslAIStatus;
    }
}