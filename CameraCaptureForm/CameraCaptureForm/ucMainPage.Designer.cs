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
            this.tBar_threshold = new System.Windows.Forms.TrackBar();
            this.btn_setThreshold = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CameraFeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBar_threshold)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_CameraFeed
            // 
            this.pb_CameraFeed.Location = new System.Drawing.Point(16, 49);
            this.pb_CameraFeed.Name = "pb_CameraFeed";
            this.pb_CameraFeed.Size = new System.Drawing.Size(876, 473);
            this.pb_CameraFeed.TabIndex = 0;
            this.pb_CameraFeed.TabStop = false;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(16, 547);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(111, 35);
            this.btn_Start.TabIndex = 4;
            this.btn_Start.Text = "Start Capture";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Enabled = false;
            this.btn_Stop.Location = new System.Drawing.Point(215, 547);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(111, 35);
            this.btn_Stop.TabIndex = 5;
            this.btn_Stop.Text = "Stop Capture";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // lbl_MainTitle
            // 
            this.lbl_MainTitle.AutoSize = true;
            this.lbl_MainTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_MainTitle.Location = new System.Drawing.Point(319, 10);
            this.lbl_MainTitle.Name = "lbl_MainTitle";
            this.lbl_MainTitle.Size = new System.Drawing.Size(194, 26);
            this.lbl_MainTitle.TabIndex = 7;
            this.lbl_MainTitle.Text = "Face Authenticator";
            // 
            // tBar_threshold
            // 
            this.tBar_threshold.Location = new System.Drawing.Point(374, 547);
            this.tBar_threshold.Maximum = 500000000;
            this.tBar_threshold.Minimum = 50000000;
            this.tBar_threshold.Name = "tBar_threshold";
            this.tBar_threshold.Size = new System.Drawing.Size(372, 45);
            this.tBar_threshold.TabIndex = 8;
            this.tBar_threshold.Value = 146250000;
            // 
            // btn_setThreshold
            // 
            this.btn_setThreshold.Enabled = false;
            this.btn_setThreshold.Location = new System.Drawing.Point(781, 547);
            this.btn_setThreshold.Name = "btn_setThreshold";
            this.btn_setThreshold.Size = new System.Drawing.Size(111, 35);
            this.btn_setThreshold.TabIndex = 9;
            this.btn_setThreshold.Text = "Set Threshold";
            this.btn_setThreshold.UseVisualStyleBackColor = true;
            this.btn_setThreshold.Click += new System.EventHandler(this.btn_setThreshold_Click);
            // 
            // ucMainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_setThreshold);
            this.Controls.Add(this.tBar_threshold);
            this.Controls.Add(this.lbl_MainTitle);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.pb_CameraFeed);
            this.Name = "ucMainPage";
            this.Size = new System.Drawing.Size(910, 627);
            ((System.ComponentModel.ISupportInitialize)(this.pb_CameraFeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBar_threshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_CameraFeed;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Label lbl_MainTitle;
        private System.Windows.Forms.TrackBar tBar_threshold;
        private System.Windows.Forms.Button btn_setThreshold;
    }
}
