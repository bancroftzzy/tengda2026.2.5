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
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearLoadcaseInfo = new System.Windows.Forms.Button();
            this.cbIsRemoveSupport = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbRemoveSupportIndex = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cbIsAddSlab = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tbSlabElevation = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbSlabThickness = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbBackfillDepth = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
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
            this.btnWriteLoadcaseInfo.Location = new System.Drawing.Point(850, 346);
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
            this.btnReadLoadcaseInfo.Location = new System.Drawing.Point(850, 394);
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
            this.btnInsertLoadcaseInfo.Location = new System.Drawing.Point(850, 442);
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
            this.btnAppendLoadcaseInfo.Location = new System.Drawing.Point(850, 490);
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
            this.btnDeleLoadcaseInfo.Location = new System.Drawing.Point(850, 298);
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
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15});
            this.lvLoadcases.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lvLoadcases.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvLoadcases.FullRowSelect = true;
            this.lvLoadcases.GridLines = true;
            this.lvLoadcases.HideSelection = false;
            this.lvLoadcases.Location = new System.Drawing.Point(24, 250);
            this.lvLoadcases.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvLoadcases.MultiSelect = false;
            this.lvLoadcases.Name = "lvLoadcases";
            this.lvLoadcases.Size = new System.Drawing.Size(800, 270);
            this.lvLoadcases.TabIndex = 41;
            this.lvLoadcases.UseCompatibleStateImageBehavior = false;
            this.lvLoadcases.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "编号";
            this.columnHeader7.Width = 60;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "开挖深度";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 100;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "激活支撑";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "拆除支撑";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader10.Width = 100;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "支撑编号";
            this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader11.Width = 100;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "浇筑顶板";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader12.Width = 100;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "顶板标高";
            this.columnHeader13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader13.Width = 100;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "顶板厚度";
            this.columnHeader14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader14.Width = 100;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "回筑深度";
            this.columnHeader15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader15.Width = 100;
            // 
            // btnClearLoadcaseInfo
            // 
            this.btnClearLoadcaseInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearLoadcaseInfo.Location = new System.Drawing.Point(850, 250);
            this.btnClearLoadcaseInfo.Name = "btnClearLoadcaseInfo";
            this.btnClearLoadcaseInfo.Size = new System.Drawing.Size(80, 28);
            this.btnClearLoadcaseInfo.TabIndex = 3;
            this.btnClearLoadcaseInfo.Text = "清空";
            this.btnClearLoadcaseInfo.UseVisualStyleBackColor = true;
            this.btnClearLoadcaseInfo.Click += new System.EventHandler(this.btnClearLoadcaseInfo_Click);
            // 
            // cbIsRemoveSupport
            // 
            this.cbIsRemoveSupport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIsRemoveSupport.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbIsRemoveSupport.FormattingEnabled = true;
            this.cbIsRemoveSupport.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cbIsRemoveSupport.Location = new System.Drawing.Point(24, 140);
            this.cbIsRemoveSupport.Name = "cbIsRemoveSupport";
            this.cbIsRemoveSupport.Size = new System.Drawing.Size(85, 28);
            this.cbIsRemoveSupport.TabIndex = 44;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(20, 107);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(81, 20);
            this.label14.TabIndex = 45;
            this.label14.Text = "拆除支撑";
            // 
            // tbRemoveSupportIndex
            // 
            this.tbRemoveSupportIndex.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRemoveSupportIndex.Location = new System.Drawing.Point(195, 140);
            this.tbRemoveSupportIndex.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbRemoveSupportIndex.Name = "tbRemoveSupportIndex";
            this.tbRemoveSupportIndex.Size = new System.Drawing.Size(85, 28);
            this.tbRemoveSupportIndex.TabIndex = 46;
            this.tbRemoveSupportIndex.Text = "-1";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(191, 107);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 20);
            this.label15.TabIndex = 47;
            this.label15.Text = "支撑编号";
            // 
            // cbIsAddSlab
            // 
            this.cbIsAddSlab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIsAddSlab.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbIsAddSlab.FormattingEnabled = true;
            this.cbIsAddSlab.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cbIsAddSlab.Location = new System.Drawing.Point(366, 140);
            this.cbIsAddSlab.Name = "cbIsAddSlab";
            this.cbIsAddSlab.Size = new System.Drawing.Size(85, 28);
            this.cbIsAddSlab.TabIndex = 48;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(362, 107);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(81, 20);
            this.label16.TabIndex = 49;
            this.label16.Text = "浇筑顶板";
            // 
            // tbSlabElevation
            // 
            this.tbSlabElevation.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSlabElevation.Location = new System.Drawing.Point(537, 140);
            this.tbSlabElevation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSlabElevation.Name = "tbSlabElevation";
            this.tbSlabElevation.Size = new System.Drawing.Size(85, 28);
            this.tbSlabElevation.TabIndex = 50;
            this.tbSlabElevation.Text = "0";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(533, 107);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(107, 20);
            this.label17.TabIndex = 51;
            this.label17.Text = "顶板标高(m)";
            // 
            // tbSlabThickness
            // 
            this.tbSlabThickness.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSlabThickness.Location = new System.Drawing.Point(24, 214);
            this.tbSlabThickness.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSlabThickness.Name = "tbSlabThickness";
            this.tbSlabThickness.Size = new System.Drawing.Size(85, 28);
            this.tbSlabThickness.TabIndex = 52;
            this.tbSlabThickness.Text = "0";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(20, 181);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(107, 20);
            this.label18.TabIndex = 53;
            this.label18.Text = "顶板厚度(m)";
            // 
            // tbBackfillDepth
            // 
            this.tbBackfillDepth.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbBackfillDepth.Location = new System.Drawing.Point(195, 214);
            this.tbBackfillDepth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbBackfillDepth.Name = "tbBackfillDepth";
            this.tbBackfillDepth.Size = new System.Drawing.Size(85, 28);
            this.tbBackfillDepth.TabIndex = 54;
            this.tbBackfillDepth.Text = "0";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(191, 181);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(107, 20);
            this.label19.TabIndex = 55;
            this.label19.Text = "回筑深度(m)";
            // 
            // Form_LoadcasesInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.tbBackfillDepth);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.tbSlabThickness);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.tbSlabElevation);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.cbIsAddSlab);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.tbRemoveSupportIndex);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.cbIsRemoveSupport);
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
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.Button btnClearLoadcaseInfo;
        private System.Windows.Forms.ComboBox cbIsRemoveSupport;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbRemoveSupportIndex;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cbIsAddSlab;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbSlabElevation;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbSlabThickness;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbBackfillDepth;
        private System.Windows.Forms.Label label19;
    }
}