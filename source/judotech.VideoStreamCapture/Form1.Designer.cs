namespace Judoka.VideoStreamCapture
{
    partial class FormMain
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
            this.panelControls = new System.Windows.Forms.Panel();
            this.panelControlsRight = new System.Windows.Forms.Panel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.panelControlsMain = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboCamera = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelDebug = new System.Windows.Forms.Panel();
            this.pointerPosition = new System.Windows.Forms.Label();
            this.framesInBuffer = new System.Windows.Forms.Label();
            this.labelDebug1 = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.picture = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panelControls.SuspendLayout();
            this.panelControlsRight.SuspendLayout();
            this.panelControlsMain.SuspendLayout();
            this.panelDebug.SuspendLayout();
            this.panelContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.panelControlsRight);
            this.panelControls.Controls.Add(this.panelControlsMain);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControls.Location = new System.Drawing.Point(0, 448);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(814, 71);
            this.panelControls.TabIndex = 0;
            // 
            // panelControlsRight
            // 
            this.panelControlsRight.Controls.Add(this.buttonStart);
            this.panelControlsRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControlsRight.Location = new System.Drawing.Point(629, 0);
            this.panelControlsRight.Name = "panelControlsRight";
            this.panelControlsRight.Size = new System.Drawing.Size(185, 71);
            this.panelControlsRight.TabIndex = 10;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(9, 6);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(170, 27);
            this.buttonStart.TabIndex = 5;
            this.buttonStart.Text = "&Start Capture";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // panelControlsMain
            // 
            this.panelControlsMain.Controls.Add(this.comboBox1);
            this.panelControlsMain.Controls.Add(this.label2);
            this.panelControlsMain.Controls.Add(this.comboCamera);
            this.panelControlsMain.Controls.Add(this.label1);
            this.panelControlsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlsMain.Location = new System.Drawing.Point(0, 0);
            this.panelControlsMain.Name = "panelControlsMain";
            this.panelControlsMain.Size = new System.Drawing.Size(814, 71);
            this.panelControlsMain.TabIndex = 9;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Video Stream Storage on this machine"});
            this.comboBox1.Location = new System.Drawing.Point(64, 34);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(267, 21);
            this.comboBox1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Storage:";
            // 
            // comboCamera
            // 
            this.comboCamera.FormattingEnabled = true;
            this.comboCamera.Location = new System.Drawing.Point(64, 7);
            this.comboCamera.Name = "comboCamera";
            this.comboCamera.Size = new System.Drawing.Size(267, 21);
            this.comboCamera.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Camera:";
            // 
            // panelDebug
            // 
            this.panelDebug.Controls.Add(this.textBox1);
            this.panelDebug.Controls.Add(this.pointerPosition);
            this.panelDebug.Controls.Add(this.framesInBuffer);
            this.panelDebug.Controls.Add(this.labelDebug1);
            this.panelDebug.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelDebug.Location = new System.Drawing.Point(814, 0);
            this.panelDebug.Name = "panelDebug";
            this.panelDebug.Size = new System.Drawing.Size(222, 519);
            this.panelDebug.TabIndex = 1;
            // 
            // pointerPosition
            // 
            this.pointerPosition.AutoSize = true;
            this.pointerPosition.Location = new System.Drawing.Point(6, 39);
            this.pointerPosition.Name = "pointerPosition";
            this.pointerPosition.Size = new System.Drawing.Size(83, 13);
            this.pointerPosition.TabIndex = 9;
            this.pointerPosition.Text = "Pointer Position:";
            // 
            // framesInBuffer
            // 
            this.framesInBuffer.AutoSize = true;
            this.framesInBuffer.Location = new System.Drawing.Point(6, 9);
            this.framesInBuffer.Name = "framesInBuffer";
            this.framesInBuffer.Size = new System.Drawing.Size(89, 13);
            this.framesInBuffer.TabIndex = 8;
            this.framesInBuffer.Text = "Frames in Buffer: ";
            // 
            // labelDebug1
            // 
            this.labelDebug1.AutoSize = true;
            this.labelDebug1.Location = new System.Drawing.Point(6, 66);
            this.labelDebug1.Name = "labelDebug1";
            this.labelDebug1.Size = new System.Drawing.Size(45, 13);
            this.labelDebug1.TabIndex = 7;
            this.labelDebug1.Text = "Debug: ";
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.picture);
            this.panelContent.Controls.Add(this.panelControls);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(814, 519);
            this.panelContent.TabIndex = 2;
            // 
            // picture
            // 
            this.picture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picture.Location = new System.Drawing.Point(0, 0);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(814, 448);
            this.picture.TabIndex = 5;
            this.picture.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 82);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(201, 99);
            this.textBox1.TabIndex = 10;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 519);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelDebug);
            this.Name = "FormMain";
            this.Text = "Judoka - Video Stream Capture";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panelControls.ResumeLayout(false);
            this.panelControlsRight.ResumeLayout(false);
            this.panelControlsMain.ResumeLayout(false);
            this.panelControlsMain.PerformLayout();
            this.panelDebug.ResumeLayout(false);
            this.panelDebug.PerformLayout();
            this.panelContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Panel panelDebug;
        private System.Windows.Forms.Label labelDebug1;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelControlsRight;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Panel panelControlsMain;
        private System.Windows.Forms.ComboBox comboCamera;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label pointerPosition;
        private System.Windows.Forms.Label framesInBuffer;
        private System.Windows.Forms.TextBox textBox1;
    }
}

