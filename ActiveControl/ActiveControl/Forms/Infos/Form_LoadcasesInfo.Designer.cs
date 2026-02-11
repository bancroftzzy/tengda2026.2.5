namespace ActiveControl.Forms
{
    partial class Form_LoadcasesInfo
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
            this.cbLoadcasesIsActive = new System.Windows.Forms.ComboBox();
            this.btnWriteLoadcaseInfo = new System.Windows.Forms.Button();
            this.btnReadLoadcaseInfo = new System.Windows.Forms.Button();
            this.btnInsertLoadcaseInfo = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbLoadcasesEcvDepth = new System.Windows.Forms.TextBox();
            this.btnAppendLoadcaseInfo = new System.Windows.Forms.Button();
            this.btnDeleLoadcaseInfo = new System.Windows.Forms.Button();
            this.lvLoadcases = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearLoadcaseInfo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbLoadcasesIsActive
            // 
            this.cbLoadcasesIsActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoadcasesIsActive.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLoadcasesIsActive.FormattingEnabled = true;
            this.cbLoadcasesIsActive.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cbLoadcasesIsActive.Location = new System.Drawing.Point(195, 66);
            this.cbLoadcasesIsActive.Name = "cbLoadcasesIsActive";
            this.cbLoadcasesIsActive.Size = new System.Drawing.Size(85, 28);
            this.cbLoadcasesIsActive.TabIndex = 2;
            // 
            // btnWriteLoadcaseInfo
            // 
            this.btnWriteLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWriteLoadcaseInfo.Location = new System.Drawing.Point(372, 206);
            this.btnWriteLoadcaseInfo.Name = "btnWriteLoadcaseInfo";
            this.btnWriteLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnWriteLoadcaseInfo.TabIndex = 5;
            this.btnWriteLoadcaseInfo.Text = "写入";
            this.btnWriteLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnWriteLoadcaseInfo.Click += new System.EventHandler(this.btnWriteLoadcaseInfo_Click);
            // 
            // btnReadLoadcaseInfo
            // 
            this.btnReadLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadLoadcaseInfo.Location = new System.Drawing.Point(372, 254);
            this.btnReadLoadcaseInfo.Name = "btnReadLoadcaseInfo";
            this.btnReadLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnReadLoadcaseInfo.TabIndex = 6;
            this.btnReadLoadcaseInfo.Text = "读取";
            this.btnReadLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnReadLoadcaseInfo.Click += new System.EventHandler(this.btnReadLoadcaseInfo_Click);
            // 
            // btnInsertLoadcaseInfo
            // 
            this.btnInsertLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertLoadcaseInfo.Location = new System.Drawing.Point(372, 302);
            this.btnInsertLoadcaseInfo.Name = "btnInsertLoadcaseInfo";
            this.btnInsertLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnInsertLoadcaseInfo.TabIndex = 7;
            this.btnInsertLoadcaseInfo.Text = "插入";
            this.btnInsertLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnInsertLoadcaseInfo.Click += new System.EventHandler(this.btnInsertLoadcaseInfo_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(191, 33);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 20);
            this.label12.TabIndex = 43;
            this.label12.Text = "激活支撑";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(20, 33);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(107, 20);
            this.label13.TabIndex = 42;
            this.label13.Text = "开挖深度(m)";
            // 
            // tbLoadcasesEcvDepth
            // 
            this.tbLoadcasesEcvDepth.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLoadcasesEcvDepth.Location = new System.Drawing.Point(24, 66);
            this.tbLoadcasesEcvDepth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbLoadcasesEcvDepth.Name = "tbLoadcasesEcvDepth";
            this.tbLoadcasesEcvDepth.Size = new System.Drawing.Size(85, 28);
            this.tbLoadcasesEcvDepth.TabIndex = 1;
            // 
            // btnAppendLoadcaseInfo
            // 
            this.btnAppendLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppendLoadcaseInfo.Location = new System.Drawing.Point(372, 350);
            this.btnAppendLoadcaseInfo.Name = "btnAppendLoadcaseInfo";
            this.btnAppendLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnAppendLoadcaseInfo.TabIndex = 8;
            this.btnAppendLoadcaseInfo.Text = "追加";
            this.btnAppendLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnAppendLoadcaseInfo.Click += new System.EventHandler(this.btnAppendLoadcaseInfo_Click);
            // 
            // btnDeleLoadcaseInfo
            // 
            this.btnDeleLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleLoadcaseInfo.Location = new System.Drawing.Point(372, 158);
            this.btnDeleLoadcaseInfo.Name = "btnDeleLoadcaseInfo";
            this.btnDeleLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnDeleLoadcaseInfo.TabIndex = 4;
            this.btnDeleLoadcaseInfo.Text = "删除";
            this.btnDeleLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnDeleLoadcaseInfo.Click += new System.EventHandler(this.btnDeleLoadcaseInfo_Click);
            // 
            // lvLoadcases
            // 
            this.lvLoadcases.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.lvLoadcases.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lvLoadcases.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvLoadcases.FullRowSelect = true;
            this.lvLoadcases.GridLines = true;
            this.lvLoadcases.HideSelection = false;
            this.lvLoadcases.Location = new System.Drawing.Point(24, 110);
            this.lvLoadcases.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvLoadcases.MultiSelect = false;
            this.lvLoadcases.Name = "lvLoadcases";
            this.lvLoadcases.Size = new System.Drawing.Size(305, 270);
            this.lvLoadcases.TabIndex = 41;
            this.lvLoadcases.UseCompatibleStateImageBehavior = false;
            this.lvLoadcases.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "工序编号";
            this.columnHeader7.Width = 84;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "开挖深度";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 84;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "激活支撑";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader9.Width = 84;
            // 
            // btnClearLoadcaseInfo
            // 
            this.btnClearLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearLoadcaseInfo.Location = new System.Drawing.Point(372, 110);
            this.btnClearLoadcaseInfo.Name = "btnClearLoadcaseInfo";
            this.btnClearLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnClearLoadcaseInfo.TabIndex = 3;
            this.btnClearLoadcaseInfo.Text = "清空";
            this.btnClearLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnClearLoadcaseInfo.Click += new System.EventHandler(this.btnClearLoadcaseInfo_Click);
            // 
            // Form_LoadcasesInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(483, 403);
            this.Controls.Add(this.cbLoadcasesIsActive);
            this.Controls.Add(this.btnWriteLoadcaseInfo);
            this.Controls.Add(this.btnReadLoadcaseInfo);
            this.Controls.Add(this.btnInsertLoadcaseInfo);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbLoadcasesEcvDepth);
            this.Controls.Add(this.btnAppendLoadcaseInfo);
            this.Controls.Add(this.btnDeleLoadcaseInfo);
            this.Controls.Add(this.lvLoadcases);
            this.Controls.Add(this.btnClearLoadcaseInfo);
            this.Name = "Form_LoadcasesInfo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "施工信息录入";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbLoadcasesIsActive;
        private System.Windows.Forms.Button btnWriteLoadcaseInfo;
        private System.Windows.Forms.Button btnReadLoadcaseInfo;
        private System.Windows.Forms.Button btnInsertLoadcaseInfo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbLoadcasesEcvDepth;
        private System.Windows.Forms.Button btnAppendLoadcaseInfo;
        private System.Windows.Forms.Button btnDeleLoadcaseInfo;
        private System.Windows.Forms.ListView lvLoadcases;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Button btnClearLoadcaseInfo;
    }
}