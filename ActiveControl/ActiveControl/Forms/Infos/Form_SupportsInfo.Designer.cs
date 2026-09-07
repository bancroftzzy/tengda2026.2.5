namespace ActiveControl.Forms
{
    partial class Form_SupportsInfo
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
            this.cbSupportsAdjAble = new System.Windows.Forms.ComboBox();
            this.label26 = new System.Windows.Forms.Label();
            this.cbSupportsMat = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.tbSupportsMaxFC = new System.Windows.Forms.TextBox();
            this.btnWriteSupportInfo = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btnReadSupportInfo = new System.Windows.Forms.Button();
            this.btnInsertSupportInfo = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbSupportsHrzDist = new System.Windows.Forms.TextBox();
            this.tbSupportsSize1 = new System.Windows.Forms.TextBox();
            this.btnAppendSopportInfo = new System.Windows.Forms.Button();
            this.tbSupportsSize2 = new System.Windows.Forms.TextBox();
            this.tbSupportsDistToGround = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnDeleSupportInfo = new System.Windows.Forms.Button();
            this.lvSupports = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderJackStrokeMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearSupportInfo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSupportsMaxFT = new System.Windows.Forms.TextBox();
            this.labelJackStrokeMax = new System.Windows.Forms.Label();
            this.tbSupportsJackStrokeMax = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbSupportsAdjAble
            // 
            this.cbSupportsAdjAble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSupportsAdjAble.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSupportsAdjAble.FormattingEnabled = true;
            this.cbSupportsAdjAble.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cbSupportsAdjAble.Location = new System.Drawing.Point(885, 131);
            this.cbSupportsAdjAble.Name = "cbSupportsAdjAble";
            this.cbSupportsAdjAble.Size = new System.Drawing.Size(84, 28);
            this.cbSupportsAdjAble.TabIndex = 8;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(873, 97);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(99, 20);
            this.label26.TabIndex = 64;
            this.label26.Text = "轴力可调整";
            // 
            // cbSupportsMat
            // 
            this.cbSupportsMat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSupportsMat.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSupportsMat.FormattingEnabled = true;
            this.cbSupportsMat.Items.AddRange(new object[] {
            "钢",
            "混凝土"});
            this.cbSupportsMat.Location = new System.Drawing.Point(26, 134);
            this.cbSupportsMat.Name = "cbSupportsMat";
            this.cbSupportsMat.Size = new System.Drawing.Size(84, 28);
            this.cbSupportsMat.TabIndex = 1;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(657, 86);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(99, 40);
            this.label25.TabIndex = 62;
            this.label25.Text = "轴压承载力\r\n     (kN)";
            // 
            // tbSupportsMaxFC
            // 
            this.tbSupportsMaxFC.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsMaxFC.Location = new System.Drawing.Point(666, 134);
            this.tbSupportsMaxFC.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsMaxFC.Name = "tbSupportsMaxFC";
            this.tbSupportsMaxFC.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsMaxFC.TabIndex = 6;
            // 
            // btnWriteSupportInfo
            // 
            this.btnWriteSupportInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWriteSupportInfo.Location = new System.Drawing.Point(761, 39);
            this.btnWriteSupportInfo.Name = "btnWriteSupportInfo";
            this.btnWriteSupportInfo.Size = new System.Drawing.Size(60, 28);
            this.btnWriteSupportInfo.TabIndex = 11;
            this.btnWriteSupportInfo.Text = "写入";
            this.btnWriteSupportInfo.UseVisualStyleBackColor = true;
            this.btnWriteSupportInfo.Click += new System.EventHandler(this.btnWriteSupportInfo_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label10.Location = new System.Drawing.Point(22, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(225, 20);
            this.label10.TabIndex = 61;
            this.label10.Text = "从上到下依次输入支撑信息";
            // 
            // btnReadSupportInfo
            // 
            this.btnReadSupportInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadSupportInfo.Location = new System.Drawing.Point(689, 39);
            this.btnReadSupportInfo.Name = "btnReadSupportInfo";
            this.btnReadSupportInfo.Size = new System.Drawing.Size(60, 28);
            this.btnReadSupportInfo.TabIndex = 12;
            this.btnReadSupportInfo.Text = "读取";
            this.btnReadSupportInfo.UseVisualStyleBackColor = true;
            this.btnReadSupportInfo.Click += new System.EventHandler(this.btnReadSupportInfo_Click);
            // 
            // btnInsertSupportInfo
            // 
            this.btnInsertSupportInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertSupportInfo.Location = new System.Drawing.Point(833, 39);
            this.btnInsertSupportInfo.Name = "btnInsertSupportInfo";
            this.btnInsertSupportInfo.Size = new System.Drawing.Size(60, 28);
            this.btnInsertSupportInfo.TabIndex = 10;
            this.btnInsertSupportInfo.Text = "插入";
            this.btnInsertSupportInfo.UseVisualStyleBackColor = true;
            this.btnInsertSupportInfo.Click += new System.EventHandler(this.btnInsertSupportInfo_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(538, 97);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(80, 20);
            this.label14.TabIndex = 60;
            this.label14.Text = "尺寸2(m)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(410, 97);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 20);
            this.label15.TabIndex = 59;
            this.label15.Text = "尺寸1(m)";
            // 
            // tbSupportsHrzDist
            // 
            this.tbSupportsHrzDist.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsHrzDist.Location = new System.Drawing.Point(282, 134);
            this.tbSupportsHrzDist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsHrzDist.Name = "tbSupportsHrzDist";
            this.tbSupportsHrzDist.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsHrzDist.TabIndex = 3;
            // 
            // tbSupportsSize1
            // 
            this.tbSupportsSize1.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsSize1.Location = new System.Drawing.Point(410, 134);
            this.tbSupportsSize1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsSize1.Name = "tbSupportsSize1";
            this.tbSupportsSize1.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsSize1.TabIndex = 4;
            // 
            // btnAppendSopportInfo
            // 
            this.btnAppendSopportInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppendSopportInfo.Location = new System.Drawing.Point(905, 39);
            this.btnAppendSopportInfo.Name = "btnAppendSopportInfo";
            this.btnAppendSopportInfo.Size = new System.Drawing.Size(60, 28);
            this.btnAppendSopportInfo.TabIndex = 9;
            this.btnAppendSopportInfo.Text = "追加";
            this.btnAppendSopportInfo.UseVisualStyleBackColor = true;
            this.btnAppendSopportInfo.Click += new System.EventHandler(this.btnAppendSopportInfo_Click);
            // 
            // tbSupportsSize2
            // 
            this.tbSupportsSize2.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsSize2.Location = new System.Drawing.Point(538, 135);
            this.tbSupportsSize2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsSize2.Name = "tbSupportsSize2";
            this.tbSupportsSize2.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsSize2.TabIndex = 5;
            // 
            // tbSupportsDistToGround
            // 
            this.tbSupportsDistToGround.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsDistToGround.Location = new System.Drawing.Point(154, 134);
            this.tbSupportsDistToGround.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsDistToGround.Name = "tbSupportsDistToGround";
            this.tbSupportsDistToGround.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsDistToGround.TabIndex = 2;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(270, 97);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(107, 20);
            this.label16.TabIndex = 58;
            this.label16.Text = "水平间距(m)";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(142, 97);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(107, 20);
            this.label17.TabIndex = 57;
            this.label17.Text = "离地距离(m)";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(42, 97);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 20);
            this.label18.TabIndex = 56;
            this.label18.Text = "材料";
            // 
            // btnDeleSupportInfo
            // 
            this.btnDeleSupportInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleSupportInfo.Location = new System.Drawing.Point(617, 39);
            this.btnDeleSupportInfo.Name = "btnDeleSupportInfo";
            this.btnDeleSupportInfo.Size = new System.Drawing.Size(60, 28);
            this.btnDeleSupportInfo.TabIndex = 13;
            this.btnDeleSupportInfo.Text = "删除";
            this.btnDeleSupportInfo.UseVisualStyleBackColor = true;
            this.btnDeleSupportInfo.Click += new System.EventHandler(this.btnDeleSupportInfo_Click);
            // 
            // lvSupports
            // 
            this.lvSupports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader10,
            this.columnHeader7,
            this.columnHeader11,
            this.columnHeaderJackStrokeMax});
            this.lvSupports.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lvSupports.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvSupports.FullRowSelect = true;
            this.lvSupports.GridLines = true;
            this.lvSupports.HideSelection = false;
            this.lvSupports.Location = new System.Drawing.Point(26, 177);
            this.lvSupports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvSupports.MultiSelect = false;
            this.lvSupports.Name = "lvSupports";
            this.lvSupports.Size = new System.Drawing.Size(1092, 260);
            this.lvSupports.TabIndex = 55;
            this.lvSupports.UseCompatibleStateImageBehavior = false;
            this.lvSupports.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "支撑编号";
            this.columnHeader1.Width = 88;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "材料";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "离地距离";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 90;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "水平间距";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 90;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "特征尺寸1";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 95;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "特征尺寸2";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader6.Width = 95;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "轴压承载力";
            this.columnHeader10.Width = 120;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "轴拉承载力";
            this.columnHeader7.Width = 120;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "轴力可调整";
            this.columnHeader11.Width = 120;
            //
            // columnHeaderJackStrokeMax
            //
            this.columnHeaderJackStrokeMax.Text = "千斤顶行程上限(mm)";
            this.columnHeaderJackStrokeMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderJackStrokeMax.Width = 155;
            // 
            // btnClearSupportInfo
            // 
            this.btnClearSupportInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearSupportInfo.Location = new System.Drawing.Point(545, 39);
            this.btnClearSupportInfo.Name = "btnClearSupportInfo";
            this.btnClearSupportInfo.Size = new System.Drawing.Size(60, 28);
            this.btnClearSupportInfo.TabIndex = 14;
            this.btnClearSupportInfo.Text = "清空";
            this.btnClearSupportInfo.UseVisualStyleBackColor = true;
            this.btnClearSupportInfo.Click += new System.EventHandler(this.btnClearSupportInfo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(762, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 40);
            this.label1.TabIndex = 66;
            this.label1.Text = "轴拉承载力\r\n     (kN)";
            // 
            // tbSupportsMaxFT
            // 
            this.tbSupportsMaxFT.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsMaxFT.Location = new System.Drawing.Point(771, 134);
            this.tbSupportsMaxFT.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsMaxFT.Name = "tbSupportsMaxFT";
            this.tbSupportsMaxFT.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsMaxFT.TabIndex = 7;
            //
            // labelJackStrokeMax
            //
            this.labelJackStrokeMax.AutoSize = true;
            this.labelJackStrokeMax.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelJackStrokeMax.Location = new System.Drawing.Point(987, 86);
            this.labelJackStrokeMax.Name = "labelJackStrokeMax";
            this.labelJackStrokeMax.Size = new System.Drawing.Size(141, 40);
            this.labelJackStrokeMax.TabIndex = 67;
            this.labelJackStrokeMax.Text = "千斤顶行程上限\r\n        (mm)";
            //
            // tbSupportsJackStrokeMax
            //
            this.tbSupportsJackStrokeMax.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSupportsJackStrokeMax.Location = new System.Drawing.Point(1014, 134);
            this.tbSupportsJackStrokeMax.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSupportsJackStrokeMax.Name = "tbSupportsJackStrokeMax";
            this.tbSupportsJackStrokeMax.Size = new System.Drawing.Size(84, 28);
            this.tbSupportsJackStrokeMax.TabIndex = 9;
            this.tbSupportsJackStrokeMax.Text = "200";
            // 
            // Form_SupportsInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1144, 461);
            this.Controls.Add(this.labelJackStrokeMax);
            this.Controls.Add(this.tbSupportsJackStrokeMax);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSupportsMaxFT);
            this.Controls.Add(this.cbSupportsAdjAble);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.cbSupportsMat);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.tbSupportsMaxFC);
            this.Controls.Add(this.btnWriteSupportInfo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnReadSupportInfo);
            this.Controls.Add(this.btnInsertSupportInfo);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.tbSupportsHrzDist);
            this.Controls.Add(this.tbSupportsSize1);
            this.Controls.Add(this.btnAppendSopportInfo);
            this.Controls.Add(this.tbSupportsSize2);
            this.Controls.Add(this.tbSupportsDistToGround);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.btnDeleSupportInfo);
            this.Controls.Add(this.lvSupports);
            this.Controls.Add(this.btnClearSupportInfo);
            this.Name = "Form_SupportsInfo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "支撑信息录入";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSupportsAdjAble;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox cbSupportsMat;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox tbSupportsMaxFC;
        private System.Windows.Forms.Button btnWriteSupportInfo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnReadSupportInfo;
        private System.Windows.Forms.Button btnInsertSupportInfo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbSupportsHrzDist;
        private System.Windows.Forms.TextBox tbSupportsSize1;
        private System.Windows.Forms.Button btnAppendSopportInfo;
        private System.Windows.Forms.TextBox tbSupportsSize2;
        private System.Windows.Forms.TextBox tbSupportsDistToGround;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnDeleSupportInfo;
        private System.Windows.Forms.ListView lvSupports;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Button btnClearSupportInfo;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSupportsMaxFT;
        private System.Windows.Forms.ColumnHeader columnHeaderJackStrokeMax;
        private System.Windows.Forms.Label labelJackStrokeMax;
        private System.Windows.Forms.TextBox tbSupportsJackStrokeMax;
    }
}
