namespace TPCServer
{
    partial class frm_server
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnBeginListen = new System.Windows.Forms.Button();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMsgSend = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbOnline = new System.Windows.Forms.ListBox();
            this.btnSendToAll = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.txtSelectFile = new System.Windows.Forms.TextBox();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(469, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP  :";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(510, 12);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(91, 21);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "192.168.1.76";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(469, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "PORT:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(510, 45);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(91, 21);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "8011";
            // 
            // btnBeginListen
            // 
            this.btnBeginListen.Location = new System.Drawing.Point(471, 268);
            this.btnBeginListen.Name = "btnBeginListen";
            this.btnBeginListen.Size = new System.Drawing.Size(130, 42);
            this.btnBeginListen.TabIndex = 2;
            this.btnBeginListen.Text = "启动服务";
            this.btnBeginListen.UseVisualStyleBackColor = true;
            this.btnBeginListen.Click += new System.EventHandler(this.btnBeginListen_Click);
            // 
            // txtMsg
            // 
            this.txtMsg.BackColor = System.Drawing.Color.White;
            this.txtMsg.Location = new System.Drawing.Point(7, 12);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMsg.Size = new System.Drawing.Size(456, 239);
            this.txtMsg.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(471, 356);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(130, 42);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "发送消息";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtMsgSend
            // 
            this.txtMsgSend.Location = new System.Drawing.Point(7, 270);
            this.txtMsgSend.Multiline = true;
            this.txtMsgSend.Name = "txtMsgSend";
            this.txtMsgSend.Size = new System.Drawing.Size(455, 132);
            this.txtMsgSend.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(467, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "在线列表";
            // 
            // lbOnline
            // 
            this.lbOnline.FormattingEnabled = true;
            this.lbOnline.ItemHeight = 12;
            this.lbOnline.Location = new System.Drawing.Point(471, 84);
            this.lbOnline.Name = "lbOnline";
            this.lbOnline.Size = new System.Drawing.Size(130, 172);
            this.lbOnline.TabIndex = 7;
            // 
            // btnSendToAll
            // 
            this.btnSendToAll.Location = new System.Drawing.Point(471, 312);
            this.btnSendToAll.Name = "btnSendToAll";
            this.btnSendToAll.Size = new System.Drawing.Size(130, 42);
            this.btnSendToAll.TabIndex = 8;
            this.btnSendToAll.Text = "群发消息";
            this.btnSendToAll.UseVisualStyleBackColor = true;
            this.btnSendToAll.Click += new System.EventHandler(this.btnSendToAll_Click);
            // 
            // txtSelectFile
            // 
            this.txtSelectFile.BackColor = System.Drawing.Color.White;
            this.txtSelectFile.Location = new System.Drawing.Point(7, 408);
            this.txtSelectFile.Name = "txtSelectFile";
            this.txtSelectFile.ReadOnly = true;
            this.txtSelectFile.Size = new System.Drawing.Size(368, 21);
            this.txtSelectFile.TabIndex = 19;
            // 
            // btnSendFile
            // 
            this.btnSendFile.Location = new System.Drawing.Point(471, 400);
            this.btnSendFile.Name = "btnSendFile";
            this.btnSendFile.Size = new System.Drawing.Size(130, 42);
            this.btnSendFile.TabIndex = 18;
            this.btnSendFile.Text = "发送文件";
            this.btnSendFile.UseVisualStyleBackColor = true;
            this.btnSendFile.Click += new System.EventHandler(this.btnSendFile_Click_1);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(381, 408);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(81, 27);
            this.btnSelectFile.TabIndex = 17;
            this.btnSelectFile.Text = "选择文件";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click_1);
            // 
            // frm_server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 442);
            this.Controls.Add(this.txtSelectFile);
            this.Controls.Add(this.btnSendFile);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.btnSendToAll);
            this.Controls.Add(this.lbOnline);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMsgSend);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.btnBeginListen);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(626, 481);
            this.MinimumSize = new System.Drawing.Size(626, 481);
            this.Name = "frm_server";
            this.Text = "聊天程序服务端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnBeginListen;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMsgSend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbOnline;
        private System.Windows.Forms.Button btnSendToAll;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox txtSelectFile;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.Button btnSelectFile;
    }
}

