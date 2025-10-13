namespace Judoka.VideoStreamView
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonPlayStop = new System.Windows.Forms.Button();
            this.playerTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(644, 427);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // buttonPlayStop
            // 
            this.buttonPlayStop.Location = new System.Drawing.Point(275, 445);
            this.buttonPlayStop.Name = "buttonPlayStop";
            this.buttonPlayStop.Size = new System.Drawing.Size(75, 23);
            this.buttonPlayStop.TabIndex = 1;
            this.buttonPlayStop.Text = "&Play";
            this.buttonPlayStop.UseVisualStyleBackColor = true;
            this.buttonPlayStop.Click += new System.EventHandler(this.buttonPlayStop_Click);
            // 
            // playerTimer
            // 
            this.playerTimer.Interval = 40;
            this.playerTimer.Tick += new System.EventHandler(this.playerTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 668);
            this.Controls.Add(this.buttonPlayStop);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonPlayStop;
        private System.Windows.Forms.Timer playerTimer;
    }
}

