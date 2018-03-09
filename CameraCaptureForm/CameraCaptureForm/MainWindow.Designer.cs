namespace CameraCaptureForm
{
    partial class MainWindow
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
            this.pnl_Main = new System.Windows.Forms.Panel();
            this.btn_userControlSwitch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pnl_Main
            // 
            this.pnl_Main.Location = new System.Drawing.Point(0, 1);
            this.pnl_Main.Name = "pnl_Main";
            this.pnl_Main.Size = new System.Drawing.Size(910, 627);
            this.pnl_Main.TabIndex = 0;
            // 
            // btn_userControlSwitch
            // 
            this.btn_userControlSwitch.Location = new System.Drawing.Point(12, 647);
            this.btn_userControlSwitch.Name = "btn_userControlSwitch";
            this.btn_userControlSwitch.Size = new System.Drawing.Size(111, 35);
            this.btn_userControlSwitch.TabIndex = 4;
            this.btn_userControlSwitch.UseVisualStyleBackColor = true;
            this.btn_userControlSwitch.Click += new System.EventHandler(this.btn_userControlSwitch_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 694);
            this.Controls.Add(this.btn_userControlSwitch);
            this.Controls.Add(this.pnl_Main);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindow";
            this.Text = "Facial Authentication Project";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_Main;
        private System.Windows.Forms.Button btn_userControlSwitch;
    }
}

