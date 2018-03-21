namespace CameraCaptureForm
{
    partial class ucMainPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pb_CameraFeed = new System.Windows.Forms.PictureBox();
            this.btn_Start = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.lbl_MainTitle = new System.Windows.Forms.Label();
            this.cbox_AuthSelector = new System.Windows.Forms.ComboBox();
            this.btn_ChooseAuth = new System.Windows.Forms.Button();
            this.lbl_TimeTaken = new System.Windows.Forms.Label();
            this.lbl_AuthTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CameraFeed)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_CameraFeed
            // 
            this.pb_CameraFeed.Location = new System.Drawing.Point(21, 60);
            this.pb_CameraFeed.Margin = new System.Windows.Forms.Padding(4);
            this.pb_CameraFeed.Name = "pb_CameraFeed";
            this.pb_CameraFeed.Size = new System.Drawing.Size(1168, 582);
            this.pb_CameraFeed.TabIndex = 0;
            this.pb_CameraFeed.TabStop = false;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(203, 688);
            this.btn_Start.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(148, 43);
            this.btn_Start.TabIndex = 4;
            this.btn_Start.Text = "Start Capture";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Enabled = false;
            this.btn_Stop.Location = new System.Drawing.Point(468, 688);
            this.btn_Stop.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(148, 43);
            this.btn_Stop.TabIndex = 5;
            this.btn_Stop.Text = "Stop Capture";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // lbl_MainTitle
            // 
            this.lbl_MainTitle.AutoSize = true;
            this.lbl_MainTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_MainTitle.Location = new System.Drawing.Point(425, 12);
            this.lbl_MainTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_MainTitle.Name = "lbl_MainTitle";
            this.lbl_MainTitle.Size = new System.Drawing.Size(243, 31);
            this.lbl_MainTitle.TabIndex = 7;
            this.lbl_MainTitle.Text = "Face Authenticator";
            // 
            // cbox_AuthSelector
            // 
            this.cbox_AuthSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_AuthSelector.FormattingEnabled = true;
            this.cbox_AuthSelector.Location = new System.Drawing.Point(695, 698);
            this.cbox_AuthSelector.Margin = new System.Windows.Forms.Padding(4);
            this.cbox_AuthSelector.Name = "cbox_AuthSelector";
            this.cbox_AuthSelector.Size = new System.Drawing.Size(279, 24);
            this.cbox_AuthSelector.TabIndex = 8;
            this.cbox_AuthSelector.Tag = "";
            // 
            // btn_ChooseAuth
            // 
            this.btn_ChooseAuth.Location = new System.Drawing.Point(1025, 688);
            this.btn_ChooseAuth.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ChooseAuth.Name = "btn_ChooseAuth";
            this.btn_ChooseAuth.Size = new System.Drawing.Size(164, 43);
            this.btn_ChooseAuth.TabIndex = 9;
            this.btn_ChooseAuth.Text = "Choose Authenticator";
            this.btn_ChooseAuth.UseVisualStyleBackColor = true;
            this.btn_ChooseAuth.Click += new System.EventHandler(this.btn_ChooseAuth_Click);
            // 
            // lbl_TimeTaken
            // 
            this.lbl_TimeTaken.AutoSize = true;
            this.lbl_TimeTaken.Location = new System.Drawing.Point(431, 650);
            this.lbl_TimeTaken.Name = "lbl_TimeTaken";
            this.lbl_TimeTaken.Size = new System.Drawing.Size(191, 17);
            this.lbl_TimeTaken.TabIndex = 10;
            this.lbl_TimeTaken.Text = "Time Taken To Authenticate:";
            // 
            // lbl_AuthTime
            // 
            this.lbl_AuthTime.AutoSize = true;
            this.lbl_AuthTime.Location = new System.Drawing.Point(628, 650);
            this.lbl_AuthTime.Name = "lbl_AuthTime";
            this.lbl_AuthTime.Size = new System.Drawing.Size(24, 17);
            this.lbl_AuthTime.TabIndex = 11;
            this.lbl_AuthTime.Text = "00";
            // 
            // ucMainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_AuthTime);
            this.Controls.Add(this.lbl_TimeTaken);
            this.Controls.Add(this.btn_ChooseAuth);
            this.Controls.Add(this.cbox_AuthSelector);
            this.Controls.Add(this.lbl_MainTitle);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.pb_CameraFeed);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ucMainPage";
            this.Size = new System.Drawing.Size(1213, 772);
            ((System.ComponentModel.ISupportInitialize)(this.pb_CameraFeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_CameraFeed;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Label lbl_MainTitle;
        private System.Windows.Forms.ComboBox cbox_AuthSelector;
        private System.Windows.Forms.Button btn_ChooseAuth;
        private System.Windows.Forms.Label lbl_TimeTaken;
        private System.Windows.Forms.Label lbl_AuthTime;
    }
}
