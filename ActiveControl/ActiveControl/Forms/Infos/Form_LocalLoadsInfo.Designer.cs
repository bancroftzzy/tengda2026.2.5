namespace ActiveControl.Forms
{
    partial class Form_LocalLoadsInfo
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
            this.btnWriteLocalLoadsInfo = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnReadLocalLoadsInfo = new System.Windows.Forms.Button();
            this.btnInsertLocalLoadsInfo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDistToECS = new System.Windows.Forms.TextBox();
            this.btnAppendLocalLoadsInfo = new System.Windows.Forms.Button();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.tbLocalGroundLoad = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDeleLocalLoadsInfo = new System.Windows.Forms.Button();
            this.lvLocalLoads = new System.Windows.Forms.ListView();
            this.chLocalLoadNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDistToECS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chWidth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLocalGroundLoad = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearLocalLoadsInfo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnWriteLocalLoadsInfo
            // 
            this.btnWriteLocalLoadsInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWriteLocalLoadsInfo.Location = new System.Drawing.Point(640, 37);
            this.btnWriteLocalLoadsInfo.Name = "btnWriteLocalLoadsInfo";
            this.btnWriteLocalLoadsInfo.Size = new System.Drawing.Size(85, 28);
            this.btnWriteLocalLoadsInfo.TabIndex = 11;
            this.btnWriteLocalLoadsInfo.Text = "写入";
            this.btnWriteLocalLoadsInfo.UseVisualStyleBackColor = true;
            this.btnWriteLocalLoadsInfo.Click += new System.EventHandler(this.btnWriteLocalLoadsInfo_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label9.Location = new System.Drawing.Point(20, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(225, 20);
            this.label9.TabIndex = 63;
            this.label9.Text = "依次输入局部荷载信息";
            // 
            // btnReadLocalLoadsInfo
            // 
            this.btnReadLocalLoadsInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadLocalLoadsInfo.Location = new System.Drawing.Point(540, 37);
            this.btnReadLocalLoadsInfo.Name = "btnReadLocalLoadsInfo";
            this.btnReadLocalLoadsInfo.Size = new System.Drawing.Size(85, 28);
            this.btnReadLocalLoadsInfo.TabIndex = 12;
            this.btnReadLocalLoadsInfo.Text = "读取";
            this.btnReadLocalLoadsInfo.UseVisualStyleBackColor = true;
            this.btnReadLocalLoadsInfo.Click += new System.EventHandler(this.btnReadLocalLoadsInfo_Click);
            // 
            // btnInsertLocalLoadsInfo
            // 
            this.btnInsertLocalLoadsInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertLocalLoadsInfo.Location = new System.Drawing.Point(740, 37);
            this.btnInsertLocalLoadsInfo.Name = "btnInsertLocalLoadsInfo";
            this.btnInsertLocalLoadsInfo.Size = new System.Drawing.Size(85, 28);
            this.btnInsertLocalLoadsInfo.TabIndex = 10;
            this.btnInsertLocalLoadsInfo.Text = "插入";
            this.btnInsertLocalLoadsInfo.UseVisualStyleBackColor = true;
            this.btnInsertLocalLoadsInfo.Click += new System.EventHandler(this.btnInsertLocalLoadsInfo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(422, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 20);
            this.label3.TabIndex = 58;
            this.label3.Text = "局部地面荷载(kPa)";
            // 
            // tbDistToECS
            // 
            this.tbDistToECS.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDistToECS.Location = new System.Drawing.Point(24, 118);
            this.tbDistToECS.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbDistToECS.Name = "tbDistToECS";
            this.tbDistToECS.Size = new System.Drawing.Size(160, 28);
            this.tbDistToECS.TabIndex = 1;
            // 
            // btnAppendLocalLoadsInfo
            // 
            this.btnAppendLocalLoadsInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppendLocalLoadsInfo.Location = new System.Drawing.Point(840, 37);
            this.btnAppendLocalLoadsInfo.Name = "btnAppendLocalLoadsInfo";
            this.btnAppendLocalLoadsInfo.Size = new System.Drawing.Size(85, 28);
            this.btnAppendLocalLoadsInfo.TabIndex = 9;
            this.btnAppendLocalLoadsInfo.Text = "追加";
            this.btnAppendLocalLoadsInfo.UseVisualStyleBackColor = true;
            this.btnAppendLocalLoadsInfo.Click += new System.EventHandler(this.btnAppendLocalLoadsInfo_Click);
            // 
            // tbWidth
            // 
            this.tbWidth.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWidth.Location = new System.Drawing.Point(224, 118);
            this.tbWidth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(160, 28);
            this.tbWidth.TabIndex = 2;
            // 
            // tbLocalGroundLoad
            // 
            this.tbLocalGroundLoad.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLocalGroundLoad.Location = new System.Drawing.Point(426, 118);
            this.tbLocalGroundLoad.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbLocalGroundLoad.Name = "tbLocalGroundLoad";
            this.tbLocalGroundLoad.Size = new System.Drawing.Size(160, 28);
            this.tbLocalGroundLoad.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(220, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 20);
            this.label2.TabIndex = 57;
            this.label2.Text = "荷载宽度(m)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 20);
            this.label1.TabIndex = 55;
            this.label1.Text = "距围护结构距离(m)";
            // 
            // btnDeleLocalLoadsInfo
            // 
            this.btnDeleLocalLoadsInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleLocalLoadsInfo.Location = new System.Drawing.Point(440, 37);
            this.btnDeleLocalLoadsInfo.Name = "btnDeleLocalLoadsInfo";
            this.btnDeleLocalLoadsInfo.Size = new System.Drawing.Size(85, 28);
            this.btnDeleLocalLoadsInfo.TabIndex = 13;
            this.btnDeleLocalLoadsInfo.Text = "删除";
            this.btnDeleLocalLoadsInfo.UseVisualStyleBackColor = true;
            this.btnDeleLocalLoadsInfo.Click += new System.EventHandler(this.btnDeleLocalLoadsInfo_Click);
            // 
            // lvLocalLoads
            // 
            this.lvLocalLoads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chLocalLoadNo,
            this.chDistToECS,
            this.chWidth,
            this.chLocalGroundLoad});
            this.lvLocalLoads.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lvLocalLoads.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvLocalLoads.FullRowSelect = true;
            this.lvLocalLoads.GridLines = true;
            this.lvLocalLoads.HideSelection = false;
            this.lvLocalLoads.Location = new System.Drawing.Point(24, 169);
            this.lvLocalLoads.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvLocalLoads.MultiSelect = false;
            this.lvLocalLoads.Name = "lvLocalLoads";
            this.lvLocalLoads.Size = new System.Drawing.Size(900, 338);
            this.lvLocalLoads.TabIndex = 54;
            this.lvLocalLoads.UseCompatibleStateImageBehavior = false;
            this.lvLocalLoads.View = System.Windows.Forms.View.Details;
            // 
            // chLocalLoadNo
            // 
            this.chLocalLoadNo.Text = "荷载编号";
            this.chLocalLoadNo.Width = 120;
            // 
            // chDistToECS
            // 
            this.chDistToECS.Text = "距围护结构距离(m)";
            this.chDistToECS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chDistToECS.Width = 180;
            // 
            // chWidth
            // 
            this.chWidth.Text = "荷载宽度(m)";
            this.chWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chWidth.Width = 150;
            // 
            // chLocalGroundLoad
            // 
            this.chLocalGroundLoad.Text = "局部地面荷载(kPa)";
            this.chLocalGroundLoad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chLocalGroundLoad.Width = 180;
            // 
            // btnClearLocalLoadsInfo
            // 
            this.btnClearLocalLoadsInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearLocalLoadsInfo.Location = new System.Drawing.Point(340, 37);
            this.btnClearLocalLoadsInfo.Name = "btnClearLocalLoadsInfo";
            this.btnClearLocalLoadsInfo.Size = new System.Drawing.Size(85, 28);
            this.btnClearLocalLoadsInfo.TabIndex = 14;
            this.btnClearLocalLoadsInfo.Text = "清空";
            this.btnClearLocalLoadsInfo.UseVisualStyleBackColor = true;
            this.btnClearLocalLoadsInfo.Click += new System.EventHandler(this.btnClearLocalLoadsInfo_Click);
            // 
            // Form_LocalLoadsInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(950, 532);
            this.Controls.Add(this.btnWriteLocalLoadsInfo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnReadLocalLoadsInfo);
            this.Controls.Add(this.btnInsertLocalLoadsInfo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbDistToECS);
            this.Controls.Add(this.btnAppendLocalLoadsInfo);
            this.Controls.Add(this.tbWidth);
            this.Controls.Add(this.tbLocalGroundLoad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDeleLocalLoadsInfo);
            this.Controls.Add(this.lvLocalLoads);
            this.Controls.Add(this.btnClearLocalLoadsInfo);
            this.Name = "Form_LocalLoadsInfo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "局部荷载信息录入";
            this.Load += new System.EventHandler(this.Form_LocalLoadsInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWriteLocalLoadsInfo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnReadLocalLoadsInfo;
        private System.Windows.Forms.Button btnInsertLocalLoadsInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDistToECS;
        private System.Windows.Forms.Button btnAppendLocalLoadsInfo;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.TextBox tbLocalGroundLoad;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDeleLocalLoadsInfo;
        private System.Windows.Forms.ListView lvLocalLoads;
        private System.Windows.Forms.ColumnHeader chLocalLoadNo;
        private System.Windows.Forms.ColumnHeader chDistToECS;
        private System.Windows.Forms.ColumnHeader chWidth;
        private System.Windows.Forms.ColumnHeader chLocalGroundLoad;
        private System.Windows.Forms.Button btnClearLocalLoadsInfo;
    }
}
