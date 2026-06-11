namespace ActiveControl.Forms
{
    partial class Form_SoilLayersInfo
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
            this.btnWriteSoilLayerInfo = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnReadSoilLayerInfo = new System.Windows.Forms.Button();
            this.btnInsertSoilLayerInfo = new System.Windows.Forms.Button();
            this.cbSoilLayersType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSoilLayersPhi = new System.Windows.Forms.TextBox();
            this.tbSoilLayersK0 = new System.Windows.Forms.TextBox();
            this.btnAppendSoilLayerInfo = new System.Windows.Forms.Button();
            this.tbSoilLayersEs = new System.Windows.Forms.TextBox();
            this.tbSoilLayersM = new System.Windows.Forms.TextBox();
            this.tbSoilLayersGamma = new System.Windows.Forms.TextBox();
            this.tbSoilLayersC = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSoilLayersThick = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDeleSoilLayerInfo = new System.Windows.Forms.Button();
            this.lvSoilLayers = new System.Windows.Forms.ListView();
            this.chSoilLayersNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersThick = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersC = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersPhi = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersK0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersEs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersGamma = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSoilLayersWaterSoilMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearSoilLayerInfo = new System.Windows.Forms.Button();
            this.chkEnableWater = new System.Windows.Forms.CheckBox();
            this.lblWaterTableElev = new System.Windows.Forms.Label();
            this.tbWaterTableElev = new System.Windows.Forms.TextBox();
            this.lblWaterUnit = new System.Windows.Forms.Label();
            this.lblWaterSoilMode = new System.Windows.Forms.Label();
            this.cbWaterSoilMode = new System.Windows.Forms.ComboBox();
            this.chkEnableNonlinear = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnWriteSoilLayerInfo
            // 
            this.btnWriteSoilLayerInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWriteSoilLayerInfo.Location = new System.Drawing.Point(894, 37);
            this.btnWriteSoilLayerInfo.Name = "btnWriteSoilLayerInfo";
            this.btnWriteSoilLayerInfo.Size = new System.Drawing.Size(60, 28);
            this.btnWriteSoilLayerInfo.TabIndex = 11;
            this.btnWriteSoilLayerInfo.Text = "写入";
            this.btnWriteSoilLayerInfo.UseVisualStyleBackColor = true;
            this.btnWriteSoilLayerInfo.Click += new System.EventHandler(this.btnWriteSoilLayerInfo_Click);
            // 
            // label9
            //
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label9.Location = new System.Drawing.Point(20, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(225, 20);
            this.label9.TabIndex = 63;
            this.label9.Text = "从上到下依次输入土层信息";
//            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // btnReadSoilLayerInfo
            // 
            this.btnReadSoilLayerInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReadSoilLayerInfo.Location = new System.Drawing.Point(819, 37);
            this.btnReadSoilLayerInfo.Name = "btnReadSoilLayerInfo";
            this.btnReadSoilLayerInfo.Size = new System.Drawing.Size(60, 28);
            this.btnReadSoilLayerInfo.TabIndex = 12;
            this.btnReadSoilLayerInfo.Text = "读取";
            this.btnReadSoilLayerInfo.UseVisualStyleBackColor = true;
            this.btnReadSoilLayerInfo.Click += new System.EventHandler(this.btnReadSoilLayerInfo_Click);
            // 
            // btnInsertSoilLayerInfo
            // 
            this.btnInsertSoilLayerInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertSoilLayerInfo.Location = new System.Drawing.Point(969, 37);
            this.btnInsertSoilLayerInfo.Name = "btnInsertSoilLayerInfo";
            this.btnInsertSoilLayerInfo.Size = new System.Drawing.Size(60, 28);
            this.btnInsertSoilLayerInfo.TabIndex = 10;
            this.btnInsertSoilLayerInfo.Text = "插入";
            this.btnInsertSoilLayerInfo.UseVisualStyleBackColor = true;
            this.btnInsertSoilLayerInfo.Click += new System.EventHandler(this.btnInsertSoilLayerInfo_Click);
            // 
            // cbSoilLayersType
            // 
            this.cbSoilLayersType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSoilLayersType.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSoilLayersType.FormattingEnabled = true;
            this.cbSoilLayersType.Items.AddRange(new object[] {
            "杂填土",
            "粉质黏土",
            "砂质粉土",
            "粉砂",
            "淤泥质粘土"});
            this.cbSoilLayersType.Location = new System.Drawing.Point(962, 118);
            this.cbSoilLayersType.Name = "cbSoilLayersType";
            this.cbSoilLayersType.Size = new System.Drawing.Size(142, 28);
            this.cbSoilLayersType.TabIndex = 8;
            this.cbSoilLayersType.SelectedIndexChanged += new System.EventHandler(this.cbSoilLayersType_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(965, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 20);
            this.label8.TabIndex = 62;
            this.label8.Text = "土性";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(825, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 20);
            this.label7.TabIndex = 61;
            this.label7.Text = "γ(kN/m^3)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(690, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.TabIndex = 60;
            this.label6.Text = "m(MN/m^4)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(556, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 20);
            this.label5.TabIndex = 59;
            this.label5.Text = "Es(MPa)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(422, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 20);
            this.label3.TabIndex = 58;
            this.label3.Text = "K0";
            // 
            // tbSoilLayersPhi
            // 
            this.tbSoilLayersPhi.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersPhi.Location = new System.Drawing.Point(292, 118);
            this.tbSoilLayersPhi.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersPhi.Name = "tbSoilLayersPhi";
            this.tbSoilLayersPhi.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersPhi.TabIndex = 3;
            // 
            // tbSoilLayersK0
            // 
            this.tbSoilLayersK0.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersK0.Location = new System.Drawing.Point(426, 118);
            this.tbSoilLayersK0.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersK0.Name = "tbSoilLayersK0";
            this.tbSoilLayersK0.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersK0.TabIndex = 4;
            // 
            // btnAppendSoilLayerInfo
            // 
            this.btnAppendSoilLayerInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppendSoilLayerInfo.Location = new System.Drawing.Point(1044, 37);
            this.btnAppendSoilLayerInfo.Name = "btnAppendSoilLayerInfo";
            this.btnAppendSoilLayerInfo.Size = new System.Drawing.Size(60, 28);
            this.btnAppendSoilLayerInfo.TabIndex = 9;
            this.btnAppendSoilLayerInfo.Text = "追加";
            this.btnAppendSoilLayerInfo.UseVisualStyleBackColor = true;
            this.btnAppendSoilLayerInfo.Click += new System.EventHandler(this.btnAppendSoilLayerInfo_Click);
            // 
            // tbSoilLayersEs
            // 
            this.tbSoilLayersEs.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersEs.Location = new System.Drawing.Point(560, 118);
            this.tbSoilLayersEs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersEs.Name = "tbSoilLayersEs";
            this.tbSoilLayersEs.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersEs.TabIndex = 5;
            // 
            // tbSoilLayersM
            // 
            this.tbSoilLayersM.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersM.Location = new System.Drawing.Point(694, 118);
            this.tbSoilLayersM.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersM.Name = "tbSoilLayersM";
            this.tbSoilLayersM.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersM.TabIndex = 6;
            // 
            // tbSoilLayersGamma
            // 
            this.tbSoilLayersGamma.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersGamma.Location = new System.Drawing.Point(828, 118);
            this.tbSoilLayersGamma.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersGamma.Name = "tbSoilLayersGamma";
            this.tbSoilLayersGamma.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersGamma.TabIndex = 7;
            // 
            // tbSoilLayersC
            // 
            this.tbSoilLayersC.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersC.Location = new System.Drawing.Point(158, 118);
            this.tbSoilLayersC.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersC.Name = "tbSoilLayersC";
            this.tbSoilLayersC.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersC.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(288, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 57;
            this.label2.Text = "φ(°)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(154, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 20);
            this.label4.TabIndex = 56;
            this.label4.Text = "c(kPa)";
            // 
            // tbSoilLayersThick
            // 
            this.tbSoilLayersThick.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSoilLayersThick.Location = new System.Drawing.Point(24, 118);
            this.tbSoilLayersThick.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbSoilLayersThick.Name = "tbSoilLayersThick";
            this.tbSoilLayersThick.Size = new System.Drawing.Size(85, 28);
            this.tbSoilLayersThick.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 55;
            this.label1.Text = "厚度(m)";
            // 
            // btnDeleSoilLayerInfo
            // 
            this.btnDeleSoilLayerInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleSoilLayerInfo.Location = new System.Drawing.Point(744, 37);
            this.btnDeleSoilLayerInfo.Name = "btnDeleSoilLayerInfo";
            this.btnDeleSoilLayerInfo.Size = new System.Drawing.Size(60, 28);
            this.btnDeleSoilLayerInfo.TabIndex = 13;
            this.btnDeleSoilLayerInfo.Text = "删除";
            this.btnDeleSoilLayerInfo.UseVisualStyleBackColor = true;
            this.btnDeleSoilLayerInfo.Click += new System.EventHandler(this.btnDeleSoilLayerInfo_Click);
            // 
            // lvSoilLayers
            // 
            this.lvSoilLayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSoilLayersNo,
            this.chSoilLayersThick,
            this.chSoilLayersC,
            this.chSoilLayersPhi,
            this.chSoilLayersK0,
            this.chSoilLayersEs,
            this.chSoilLayersM,
            this.chSoilLayersGamma,
            this.chSoilLayersType,
            this.chSoilLayersWaterSoilMode});
            this.lvSoilLayers.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lvSoilLayers.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvSoilLayers.FullRowSelect = true;
            this.lvSoilLayers.GridLines = true;
            this.lvSoilLayers.HideSelection = false;
            this.lvSoilLayers.Location = new System.Drawing.Point(24, 169);
            this.lvSoilLayers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvSoilLayers.MultiSelect = false;
            this.lvSoilLayers.Name = "lvSoilLayers";
            this.lvSoilLayers.Size = new System.Drawing.Size(1087, 338);
            this.lvSoilLayers.TabIndex = 54;
            this.lvSoilLayers.UseCompatibleStateImageBehavior = false;
            this.lvSoilLayers.View = System.Windows.Forms.View.Details;
            // 
            // chSoilLayersNo
            // 
            this.chSoilLayersNo.Text = "土层编号";
            this.chSoilLayersNo.Width = 88;
            // 
            // chSoilLayersThick
            // 
            this.chSoilLayersThick.Text = "T(m)";
            this.chSoilLayersThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chSoilLayersC
            // 
            this.chSoilLayersC.Text = "c(kPa)";
            this.chSoilLayersC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chSoilLayersC.Width = 68;
            // 
            // chSoilLayersPhi
            // 
            this.chSoilLayersPhi.Text = "φ(°)";
            this.chSoilLayersPhi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chSoilLayersPhi.Width = 65;
            // 
            // chSoilLayersK0
            // 
            this.chSoilLayersK0.Text = "K0";
            this.chSoilLayersK0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chSoilLayersEs
            // 
            this.chSoilLayersEs.Text = "Es(MPa)";
            this.chSoilLayersEs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chSoilLayersEs.Width = 80;
            // 
            // chSoilLayersM
            // 
            this.chSoilLayersM.Text = "m(MN/m^4)";
            this.chSoilLayersM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chSoilLayersM.Width = 103;
            // 
            // chSoilLayersGamma
            // 
            this.chSoilLayersGamma.Text = "γ(kN/m^3)";
            this.chSoilLayersGamma.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.chSoilLayersGamma.Width = 90;
            // 
            // chSoilLayersType
            //
            this.chSoilLayersType.Text = "土性";
            this.chSoilLayersType.Width = 160;
            //
            // chSoilLayersWaterSoilMode
            //
            this.chSoilLayersWaterSoilMode.Text = "水土模式";
            this.chSoilLayersWaterSoilMode.Width = 140;
            //
            // btnClearSoilLayerInfo
            //
            this.btnClearSoilLayerInfo.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearSoilLayerInfo.Location = new System.Drawing.Point(669, 37);
            this.btnClearSoilLayerInfo.Name = "btnClearSoilLayerInfo";
            this.btnClearSoilLayerInfo.Size = new System.Drawing.Size(60, 28);
            this.btnClearSoilLayerInfo.TabIndex = 14;
            this.btnClearSoilLayerInfo.Text = "清空";
            this.btnClearSoilLayerInfo.UseVisualStyleBackColor = true;
            this.btnClearSoilLayerInfo.Click += new System.EventHandler(this.btnClearSoilLayerInfo_Click);
            //
            // chkEnableWater
            //
            this.chkEnableWater.AutoSize = true;
            this.chkEnableWater.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableWater.Location = new System.Drawing.Point(24, 12);
            this.chkEnableWater.Name = "chkEnableWater";
            this.chkEnableWater.Size = new System.Drawing.Size(135, 24);
            this.chkEnableWater.TabIndex = 64;
            this.chkEnableWater.Text = "考虑地下水影响";
            this.chkEnableWater.UseVisualStyleBackColor = true;
            this.chkEnableWater.CheckedChanged += new System.EventHandler(this.chkEnableWater_CheckedChanged);
            //
            // lblWaterTableElev
            //
            this.lblWaterTableElev.AutoSize = true;
            this.lblWaterTableElev.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWaterTableElev.Location = new System.Drawing.Point(40, 42);
            this.lblWaterTableElev.Name = "lblWaterTableElev";
            this.lblWaterTableElev.Size = new System.Drawing.Size(105, 20);
            this.lblWaterTableElev.TabIndex = 65;
            this.lblWaterTableElev.Text = "地下水位标高";
            //
            // tbWaterTableElev
            //
            this.tbWaterTableElev.Enabled = false;
            this.tbWaterTableElev.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWaterTableElev.Location = new System.Drawing.Point(165, 39);
            this.tbWaterTableElev.Name = "tbWaterTableElev";
            this.tbWaterTableElev.Size = new System.Drawing.Size(70, 28);
            this.tbWaterTableElev.TabIndex = 66;
            //
            // lblWaterUnit
            //
            this.lblWaterUnit.AutoSize = true;
            this.lblWaterUnit.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWaterUnit.Location = new System.Drawing.Point(240, 42);
            this.lblWaterUnit.Name = "lblWaterUnit";
            this.lblWaterUnit.Size = new System.Drawing.Size(30, 20);
            this.lblWaterUnit.TabIndex = 69;
            this.lblWaterUnit.Text = "(m)";
            //
            // lblWaterSoilMode
            //
            this.lblWaterSoilMode.AutoSize = true;
            this.lblWaterSoilMode.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWaterSoilMode.Location = new System.Drawing.Point(1120, 93);
            this.lblWaterSoilMode.Name = "lblWaterSoilMode";
            this.lblWaterSoilMode.Size = new System.Drawing.Size(75, 20);
            this.lblWaterSoilMode.TabIndex = 67;
            this.lblWaterSoilMode.Text = "水土模式";
            //
            // cbWaterSoilMode
            //
            this.cbWaterSoilMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWaterSoilMode.Enabled = false;
            this.cbWaterSoilMode.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbWaterSoilMode.FormattingEnabled = true;
            this.cbWaterSoilMode.Items.AddRange(new object[] {
            "自动（根据土性）",
            "水土分算",
            "水土合算"});
            this.cbWaterSoilMode.Location = new System.Drawing.Point(1120, 118);
            this.cbWaterSoilMode.Name = "cbWaterSoilMode";
            this.cbWaterSoilMode.Size = new System.Drawing.Size(140, 28);
            this.cbWaterSoilMode.TabIndex = 68;
            //
            // chkEnableNonlinear
            //
            this.chkEnableNonlinear.AutoSize = true;
            this.chkEnableNonlinear.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableNonlinear.Location = new System.Drawing.Point(280, 12);
            this.chkEnableNonlinear.Name = "chkEnableNonlinear";
            this.chkEnableNonlinear.Size = new System.Drawing.Size(195, 24);
            this.chkEnableNonlinear.TabIndex = 69;
            this.chkEnableNonlinear.Text = "启用非线性土弹簧计算";
            this.chkEnableNonlinear.UseVisualStyleBackColor = true;
            this.chkEnableNonlinear.CheckedChanged += new System.EventHandler(this.chkEnableNonlinear_CheckedChanged);
            //
            // Form_SoilLayersInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1280, 532);
            this.Controls.Add(this.cbWaterSoilMode);
            this.Controls.Add(this.lblWaterSoilMode);
            this.Controls.Add(this.lblWaterUnit);
            this.Controls.Add(this.tbWaterTableElev);
            this.Controls.Add(this.lblWaterTableElev);
            this.Controls.Add(this.chkEnableWater);
            this.Controls.Add(this.chkEnableNonlinear);
            this.Controls.Add(this.btnWriteSoilLayerInfo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnReadSoilLayerInfo);
            this.Controls.Add(this.btnInsertSoilLayerInfo);
            this.Controls.Add(this.cbSoilLayersType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbSoilLayersPhi);
            this.Controls.Add(this.tbSoilLayersK0);
            this.Controls.Add(this.btnAppendSoilLayerInfo);
            this.Controls.Add(this.tbSoilLayersEs);
            this.Controls.Add(this.tbSoilLayersM);
            this.Controls.Add(this.tbSoilLayersGamma);
            this.Controls.Add(this.tbSoilLayersC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbSoilLayersThick);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDeleSoilLayerInfo);
            this.Controls.Add(this.lvSoilLayers);
            this.Controls.Add(this.btnClearSoilLayerInfo);
            this.Name = "Form_SoilLayersInfo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "土层信息录入";
            this.Load += new System.EventHandler(this.Form_SoilLayersInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWriteSoilLayerInfo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnReadSoilLayerInfo;
        private System.Windows.Forms.Button btnInsertSoilLayerInfo;
        private System.Windows.Forms.ComboBox cbSoilLayersType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSoilLayersPhi;
        private System.Windows.Forms.TextBox tbSoilLayersK0;
        private System.Windows.Forms.Button btnAppendSoilLayerInfo;
        private System.Windows.Forms.TextBox tbSoilLayersEs;
        private System.Windows.Forms.TextBox tbSoilLayersM;
        private System.Windows.Forms.TextBox tbSoilLayersGamma;
        private System.Windows.Forms.TextBox tbSoilLayersC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSoilLayersThick;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDeleSoilLayerInfo;
        private System.Windows.Forms.ListView lvSoilLayers;
        private System.Windows.Forms.ColumnHeader chSoilLayersNo;
        private System.Windows.Forms.ColumnHeader chSoilLayersThick;
        private System.Windows.Forms.ColumnHeader chSoilLayersC;
        private System.Windows.Forms.ColumnHeader chSoilLayersPhi;
        private System.Windows.Forms.ColumnHeader chSoilLayersK0;
        private System.Windows.Forms.ColumnHeader chSoilLayersEs;
        private System.Windows.Forms.ColumnHeader chSoilLayersM;
        private System.Windows.Forms.ColumnHeader chSoilLayersGamma;
        private System.Windows.Forms.ColumnHeader chSoilLayersType;
        private System.Windows.Forms.ColumnHeader chSoilLayersWaterSoilMode;
        private System.Windows.Forms.Button btnClearSoilLayerInfo;
        private System.Windows.Forms.CheckBox chkEnableWater;
        private System.Windows.Forms.Label lblWaterTableElev;
        private System.Windows.Forms.TextBox tbWaterTableElev;
        private System.Windows.Forms.Label lblWaterUnit;
        private System.Windows.Forms.Label lblWaterSoilMode;
        private System.Windows.Forms.ComboBox cbWaterSoilMode;
        private System.Windows.Forms.CheckBox chkEnableNonlinear;
    }
}