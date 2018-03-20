namespace CameraCaptureForm
{
    partial class ucEnrolPage
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
            this.lbl_EnrolTitle = new System.Windows.Forms.Label();
            this.pb_CameraFeed = new System.Windows.Forms.PictureBox();
            this.pb_FaceOutput = new System.Windows.Forms.PictureBox();
            this.btn_NextFace = new System.Windows.Forms.Button();
            this.btn_PrevFace = new System.Windows.Forms.Button();
            this.btn_EnrolUser = new System.Windows.Forms.Button();
            this.tBox_UserName = new System.Windows.Forms.TextBox();
            this.lbl_UserName = new System.Windows.Forms.Label();
            this.btn_capture = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb_CameraFeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_FaceOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_EnrolTitle
            // 
            this.lbl_EnrolTitle.AutoSize = true;
            this.lbl_EnrolTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_EnrolTitle.Location = new System.Drawing.Point(488, 17);
            this.lbl_EnrolTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_EnrolTitle.Name = "lbl_EnrolTitle";
            this.lbl_EnrolTitle.Size = new System.Drawing.Size(142, 31);
            this.lbl_EnrolTitle.TabIndex = 0;
            this.lbl_EnrolTitle.Text = "Enrol User";
            this.lbl_EnrolTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_CameraFeed
            // 
            this.pb_CameraFeed.Location = new System.Drawing.Point(29, 68);
            this.pb_CameraFeed.Margin = new System.Windows.Forms.Padding(4);
            this.pb_CameraFeed.Name = "pb_CameraFeed";
            this.pb_CameraFeed.Size = new System.Drawing.Size(863, 455);
            this.pb_CameraFeed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pb_CameraFeed.TabIndex = 1;
            this.pb_CameraFeed.TabStop = false;
            // 
            // pb_FaceOutput
            // 
            this.pb_FaceOutput.Location = new System.Drawing.Point(900, 68);
            this.pb_FaceOutput.Margin = new System.Windows.Forms.Padding(4);
            this.pb_FaceOutput.Name = "pb_FaceOutput";
            this.pb_FaceOutput.Size = new System.Drawing.Size(309, 455);
            this.pb_FaceOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pb_FaceOutput.TabIndex = 2;
            this.pb_FaceOutput.TabStop = false;
            // 
            // btn_NextFace
            // 
            this.btn_NextFace.Enabled = false;
            this.btn_NextFace.Location = new System.Drawing.Point(230, 614);
            this.btn_NextFace.Margin = new System.Windows.Forms.Padding(4);
            this.btn_NextFace.Name = "btn_NextFace";
            this.btn_NextFace.Size = new System.Drawing.Size(148, 43);
            this.btn_NextFace.TabIndex = 3;
            this.btn_NextFace.Text = "Next Face";
            this.btn_NextFace.UseVisualStyleBackColor = true;
            this.btn_NextFace.Click += new System.EventHandler(this.btn_NextFace_Click);
            // 
            // btn_PrevFace
            // 
            this.btn_PrevFace.Enabled = false;
            this.btn_PrevFace.Location = new System.Drawing.Point(494, 614);
            this.btn_PrevFace.Margin = new System.Windows.Forms.Padding(4);
            this.btn_PrevFace.Name = "btn_PrevFace";
            this.btn_PrevFace.Size = new System.Drawing.Size(148, 43);
            this.btn_PrevFace.TabIndex = 4;
            this.btn_PrevFace.Text = "Previous Face";
            this.btn_PrevFace.UseVisualStyleBackColor = true;
            this.btn_PrevFace.Click += new System.EventHandler(this.btn_PrevFace_Click);
            // 
            // btn_EnrolUser
            // 
            this.btn_EnrolUser.Enabled = false;
            this.btn_EnrolUser.Location = new System.Drawing.Point(1029, 567);
            this.btn_EnrolUser.Margin = new System.Windows.Forms.Padding(4);
            this.btn_EnrolUser.Name = "btn_EnrolUser";
            this.btn_EnrolUser.Size = new System.Drawing.Size(148, 43);
            this.btn_EnrolUser.TabIndex = 5;
            this.btn_EnrolUser.Text = "Enrol User";
            this.btn_EnrolUser.UseVisualStyleBackColor = true;
            this.btn_EnrolUser.Click += new System.EventHandler(this.btn_EnrolUser_Click);
            // 
            // tBox_UserName
            // 
            this.tBox_UserName.Enabled = false;
            this.tBox_UserName.Location = new System.Drawing.Point(840, 661);
            this.tBox_UserName.Margin = new System.Windows.Forms.Padding(4);
            this.tBox_UserName.Name = "tBox_UserName";
            this.tBox_UserName.Size = new System.Drawing.Size(299, 22);
            this.tBox_UserName.TabIndex = 7;
            // 
            // lbl_UserName
            // 
            this.lbl_UserName.AutoSize = true;
            this.lbl_UserName.Location = new System.Drawing.Point(845, 640);
            this.lbl_UserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_UserName.Name = "lbl_UserName";
            this.lbl_UserName.Size = new System.Drawing.Size(79, 17);
            this.lbl_UserName.TabIndex = 8;
            this.lbl_UserName.Text = "User Name";
            // 
            // btn_capture
            // 
            this.btn_capture.Location = new System.Drawing.Point(840, 567);
            this.btn_capture.Margin = new System.Windows.Forms.Padding(4);
            this.btn_capture.Name = "btn_capture";
            this.btn_capture.Size = new System.Drawing.Size(148, 43);
            this.btn_capture.TabIndex = 9;
            this.btn_capture.Text = "Capture Faces";
            this.btn_capture.UseVisualStyleBackColor = true;
            this.btn_capture.Click += new System.EventHandler(this.btn_capture_Click);
            // 
            // ucEnrolPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_capture);
            this.Controls.Add(this.lbl_UserName);
            this.Controls.Add(this.tBox_UserName);
            this.Controls.Add(this.btn_EnrolUser);
            this.Controls.Add(this.btn_PrevFace);
            this.Controls.Add(this.btn_NextFace);
            this.Controls.Add(this.pb_FaceOutput);
            this.Controls.Add(this.pb_CameraFeed);
            this.Controls.Add(this.lbl_EnrolTitle);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ucEnrolPage";
            this.Size = new System.Drawing.Size(1213, 772);
            ((System.ComponentModel.ISupportInitialize)(this.pb_CameraFeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_FaceOutput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_EnrolTitle;
        private System.Windows.Forms.PictureBox pb_CameraFeed;
        private System.Windows.Forms.PictureBox pb_FaceOutput;
        private System.Windows.Forms.Button btn_NextFace;
        private System.Windows.Forms.Button btn_PrevFace;
        private System.Windows.Forms.Button btn_EnrolUser;
        private System.Windows.Forms.TextBox tBox_UserName;
        private System.Windows.Forms.Label lbl_UserName;
        private System.Windows.Forms.Button btn_capture;
    }
}
