namespace PAN_Windows
{
    partial class frmGetPanInquiry
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPanStatus = new System.Windows.Forms.Button();
            this.TxtPanNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TxtUboFlag = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.TxtIpvFlag = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.TxtKycMode = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.TxtUpdtRmks = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.TxtDectRmk = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.TxtUpdtStatus = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.TxtStatusDtl = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TxtModDt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TxtAppEntryDt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TxtStatusDt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtAppStatus = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtAppName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtAppPanNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnPanStatus);
            this.panel1.Controls.Add(this.TxtPanNo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(445, 38);
            this.panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(335, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(106, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPanStatus
            // 
            this.btnPanStatus.Location = new System.Drawing.Point(223, 8);
            this.btnPanStatus.Name = "btnPanStatus";
            this.btnPanStatus.Size = new System.Drawing.Size(106, 23);
            this.btnPanStatus.TabIndex = 2;
            this.btnPanStatus.Text = "&Pan Status";
            this.btnPanStatus.UseVisualStyleBackColor = true;
            this.btnPanStatus.Click += new System.EventHandler(this.btnPanStatus_Click);
            // 
            // TxtPanNo
            // 
            this.TxtPanNo.Location = new System.Drawing.Point(83, 9);
            this.TxtPanNo.MaxLength = 10;
            this.TxtPanNo.Name = "TxtPanNo";
            this.TxtPanNo.Size = new System.Drawing.Size(133, 21);
            this.TxtPanNo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pan No";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.TxtUboFlag);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.TxtIpvFlag);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.TxtKycMode);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.TxtUpdtRmks);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.TxtDectRmk);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.TxtUpdtStatus);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.TxtStatusDtl);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.TxtModDt);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.TxtAppEntryDt);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.TxtStatusDt);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.TxtAppStatus);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.TxtAppName);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.TxtAppPanNo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(6, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(653, 188);
            this.panel2.TabIndex = 1;
            // 
            // TxtUboFlag
            // 
            this.TxtUboFlag.Location = new System.Drawing.Point(151, 133);
            this.TxtUboFlag.Name = "TxtUboFlag";
            this.TxtUboFlag.ReadOnly = true;
            this.TxtUboFlag.Size = new System.Drawing.Size(183, 21);
            this.TxtUboFlag.TabIndex = 25;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(89, 137);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "UBO Flag";
            // 
            // TxtIpvFlag
            // 
            this.TxtIpvFlag.Location = new System.Drawing.Point(449, 107);
            this.TxtIpvFlag.Name = "TxtIpvFlag";
            this.TxtIpvFlag.ReadOnly = true;
            this.TxtIpvFlag.Size = new System.Drawing.Size(183, 21);
            this.TxtIpvFlag.TabIndex = 23;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(392, 111);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "IPV Flag";
            // 
            // TxtKycMode
            // 
            this.TxtKycMode.Location = new System.Drawing.Point(151, 107);
            this.TxtKycMode.Name = "TxtKycMode";
            this.TxtKycMode.ReadOnly = true;
            this.TxtKycMode.Size = new System.Drawing.Size(183, 21);
            this.TxtKycMode.TabIndex = 21;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(83, 111);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(62, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "KYC Mode";
            // 
            // TxtUpdtRmks
            // 
            this.TxtUpdtRmks.Location = new System.Drawing.Point(449, 133);
            this.TxtUpdtRmks.Name = "TxtUpdtRmks";
            this.TxtUpdtRmks.ReadOnly = true;
            this.TxtUpdtRmks.Size = new System.Drawing.Size(183, 21);
            this.TxtUpdtRmks.TabIndex = 19;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(353, 137);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "UPDT Remarks";
            // 
            // TxtDectRmk
            // 
            this.TxtDectRmk.Location = new System.Drawing.Point(151, 159);
            this.TxtDectRmk.Name = "TxtDectRmk";
            this.TxtDectRmk.ReadOnly = true;
            this.TxtDectRmk.Size = new System.Drawing.Size(183, 21);
            this.TxtDectRmk.TabIndex = 17;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 163);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(139, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Hold Deactive Remarks";
            // 
            // TxtUpdtStatus
            // 
            this.TxtUpdtStatus.Location = new System.Drawing.Point(449, 82);
            this.TxtUpdtStatus.Name = "TxtUpdtStatus";
            this.TxtUpdtStatus.ReadOnly = true;
            this.TxtUpdtStatus.Size = new System.Drawing.Size(183, 21);
            this.TxtUpdtStatus.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(367, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "UPDT Status";
            // 
            // TxtStatusDtl
            // 
            this.TxtStatusDtl.Location = new System.Drawing.Point(151, 82);
            this.TxtStatusDtl.Name = "TxtStatusDtl";
            this.TxtStatusDtl.ReadOnly = true;
            this.TxtStatusDtl.Size = new System.Drawing.Size(183, 21);
            this.TxtStatusDtl.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(68, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Status Delta";
            // 
            // TxtModDt
            // 
            this.TxtModDt.Location = new System.Drawing.Point(449, 57);
            this.TxtModDt.Name = "TxtModDt";
            this.TxtModDt.ReadOnly = true;
            this.TxtModDt.Size = new System.Drawing.Size(183, 21);
            this.TxtModDt.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(381, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "MOD Date";
            // 
            // TxtAppEntryDt
            // 
            this.TxtAppEntryDt.Location = new System.Drawing.Point(151, 57);
            this.TxtAppEntryDt.Name = "TxtAppEntryDt";
            this.TxtAppEntryDt.ReadOnly = true;
            this.TxtAppEntryDt.Size = new System.Drawing.Size(183, 21);
            this.TxtAppEntryDt.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(78, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Entry Date";
            // 
            // TxtStatusDt
            // 
            this.TxtStatusDt.Location = new System.Drawing.Point(449, 31);
            this.TxtStatusDt.Name = "TxtStatusDt";
            this.TxtStatusDt.ReadOnly = true;
            this.TxtStatusDt.Size = new System.Drawing.Size(183, 21);
            this.TxtStatusDt.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(370, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Status Date";
            // 
            // TxtAppStatus
            // 
            this.TxtAppStatus.Location = new System.Drawing.Point(151, 31);
            this.TxtAppStatus.Name = "TxtAppStatus";
            this.TxtAppStatus.ReadOnly = true;
            this.TxtAppStatus.Size = new System.Drawing.Size(183, 21);
            this.TxtAppStatus.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(101, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Status";
            // 
            // TxtAppName
            // 
            this.TxtAppName.Location = new System.Drawing.Point(449, 6);
            this.TxtAppName.Name = "TxtAppName";
            this.TxtAppName.ReadOnly = true;
            this.TxtAppName.Size = new System.Drawing.Size(183, 21);
            this.TxtAppName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(405, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name";
            // 
            // TxtAppPanNo
            // 
            this.TxtAppPanNo.Location = new System.Drawing.Point(151, 6);
            this.TxtAppPanNo.Name = "TxtAppPanNo";
            this.TxtAppPanNo.ReadOnly = true;
            this.TxtAppPanNo.Size = new System.Drawing.Size(183, 21);
            this.TxtAppPanNo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(100, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pan No";
            // 
            // frmGetPanInquiry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 246);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmGetPanInquiry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Get Pan Inquiry";
            this.Load += new System.EventHandler(this.frmGetPanInquiry_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmGetPanInquiry_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox TxtPanNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPanStatus;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtAppPanNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtAppName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtAppStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtStatusDt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TxtAppEntryDt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TxtModDt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TxtStatusDtl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TxtUpdtStatus;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox TxtDectRmk;
        private System.Windows.Forms.TextBox TxtUpdtRmks;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox TxtKycMode;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox TxtIpvFlag;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox TxtUboFlag;
    }
}