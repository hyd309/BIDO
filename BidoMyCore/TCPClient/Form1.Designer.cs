namespace TCPClient
{
    partial class frmClient
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.txtSendMsg = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSelectFile = new System.Windows.Forms.TextBox();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtMsg
            // 
            this.txtMsg.BackColor = System.Drawing.Color.White;
            this.txtMsg.Location = new System.Drawing.Point(3, 12);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMsg.Size = new System.Drawing.Size(504, 225);
            this.txtMsg.TabIndex = 9;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(515, 255);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(136, 42);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "连接服务端";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(559, 180);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(91, 21);
            this.txtPort.TabIndex = 7;
            this.txtPort.Text = "8011";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(519, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "PORT:";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(557, 147);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(94, 21);
            this.txtIp.TabIndex = 6;
            this.txtIp.Text = "192.168.1.76";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(519, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP  :";
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Location = new System.Drawing.Point(515, 303);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(136, 42);
            this.btnSendMsg.TabIndex = 10;
            this.btnSendMsg.Text = "发送消息";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // txtSendMsg
            // 
            this.txtSendMsg.Location = new System.Drawing.Point(8, 255);
            this.txtSendMsg.Multiline = true;
            this.txtSendMsg.Name = "txtSendMsg";
            this.txtSendMsg.Size = new System.Drawing.Size(499, 89);
            this.txtSendMsg.TabIndex = 11;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(560, 216);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(91, 21);
            this.txtName.TabIndex = 13;
            this.txtName.Text = "炽  雪";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(521, 221);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "昵称：";
            // 
            // txtSelectFile
            // 
            this.txtSelectFile.BackColor = System.Drawing.Color.White;
            this.txtSelectFile.Location = new System.Drawing.Point(7, 372);
            this.txtSelectFile.Name = "txtSelectFile";
            this.txtSelectFile.ReadOnly = true;
            this.txtSelectFile.Size = new System.Drawing.Size(411, 21);
            this.txtSelectFile.TabIndex = 16;
            // 
            // btnSendFile
            // 
            this.btnSendFile.Location = new System.Drawing.Point(515, 349);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(136, 42);
            this.btnSendFile.TabIndex = 15;
            this.btnSendFile.Text = "发送文件";
            this.btnSendFile.UseVisualStyleBackColor = true;
            this.btnSendFile.Click += new System.EventHandler(this.btnSendFile_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(424, 365);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(81, 27);
            this.btnSelectFile.TabIndex = 14;
            this.btnSelectFile.Text = "选择文件";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(512, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(147, 141);
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // frmClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 400);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtSelectFile);
            this.Controls.Add(this.btnSendFile);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSendMsg);
            this.Controls.Add(this.btnSendMsg);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(676, 439);
            this.MinimumSize = new System.Drawing.Size(676, 439);
            this.Name = "frmClient";
            this.Text = "聊天程序客户端";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSendMsg;
        private System.Windows.Forms.TextBox txtSendMsg;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSelectFile;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

