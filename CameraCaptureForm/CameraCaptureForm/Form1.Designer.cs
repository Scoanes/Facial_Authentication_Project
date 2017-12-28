namespace CameraCaptureForm
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_StartCapture = new System.Windows.Forms.Button();
            this.btn_StopCapture = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(61, 71);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1012, 466);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btn_StartCapture
            // 
            this.btn_StartCapture.Location = new System.Drawing.Point(61, 12);
            this.btn_StartCapture.Name = "btn_StartCapture";
            this.btn_StartCapture.Size = new System.Drawing.Size(119, 35);
            this.btn_StartCapture.TabIndex = 1;
            this.btn_StartCapture.Text = "Start Capture";
            this.btn_StartCapture.UseVisualStyleBackColor = true;
            this.btn_StartCapture.Click += new System.EventHandler(this.btn_StartCapture_Click);
            // 
            // btn_StopCapture
            // 
            this.btn_StopCapture.Location = new System.Drawing.Point(214, 12);
            this.btn_StopCapture.Name = "btn_StopCapture";
            this.btn_StopCapture.Size = new System.Drawing.Size(133, 35);
            this.btn_StopCapture.TabIndex = 2;
            this.btn_StopCapture.Text = "Stop Capture";
            this.btn_StopCapture.UseVisualStyleBackColor = true;
            this.btn_StopCapture.Click += new System.EventHandler(this.btn_StopCapture_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1144, 578);
            this.Controls.Add(this.btn_StopCapture);
            this.Controls.Add(this.btn_StartCapture);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_StartCapture;
        private System.Windows.Forms.Button btn_StopCapture;
    }
}

