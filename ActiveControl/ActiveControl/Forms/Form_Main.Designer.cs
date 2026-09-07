namespace ActiveControl
{
    partial class Form_Main
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.gbOutputWindow = new System.Windows.Forms.GroupBox();
            this.btnWriteLogToFile = new System.Windows.Forms.Button();
            this.btnClearOutputWindow = new System.Windows.Forms.Button();
            this.rtbOutputWindow = new System.Windows.Forms.RichTextBox();
            this.msInput = new System.Windows.Forms.MenuStrip();
            this.msInputData = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInputSoilLayersInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInputSupportsInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInputLoadcasesInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInputLocalLoadsInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInputOtherInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.数据导出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputNodesInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputElementsInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputLoadsInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputResults = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputM = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputFy = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputUx = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOutputFx = new System.Windows.Forms.ToolStripMenuItem();
            this.Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cbFigType = new System.Windows.Forms.ComboBox();
            this.cbStage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbDraw = new System.Windows.Forms.GroupBox();
            this.btnDraw = new System.Windows.Forms.Button();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.gbOutputWindow.SuspendLayout();
            this.msInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
            this.gbDraw.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbOutputWindow
            // 
            this.gbOutputWindow.Controls.Add(this.btnWriteLogToFile);
            this.gbOutputWindow.Controls.Add(this.btnClearOutputWindow);
            this.gbOutputWindow.Controls.Add(this.rtbOutputWindow);
            this.gbOutputWindow.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOutputWindow.Location = new System.Drawing.Point(24, 79);
            this.gbOutputWindow.Name = "gbOutputWindow";
            this.gbOutputWindow.Size = new System.Drawing.Size(1406, 615);
            this.gbOutputWindow.TabIndex = 40;
            this.gbOutputWindow.TabStop = false;
            this.gbOutputWindow.Text = "输出窗口";
            // 
            // btnWriteLogToFile
            // 
            this.btnWriteLogToFile.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWriteLogToFile.Location = new System.Drawing.Point(1291, 33);
            this.btnWriteLogToFile.Name = "btnWriteLogToFile";
            this.btnWriteLogToFile.Size = new System.Drawing.Size(60, 28);
            this.btnWriteLogToFile.TabIndex = 6;
            this.btnWriteLogToFile.Text = "写入";
            this.btnWriteLogToFile.UseVisualStyleBackColor = true;
            this.btnWriteLogToFile.Click += new System.EventHandler(this.btnWriteLogToFile_Click);
            // 
            // btnClearOutputWindow
            // 
            this.btnClearOutputWindow.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearOutputWindow.Location = new System.Drawing.Point(1190, 33);
            this.btnClearOutputWindow.Name = "btnClearOutputWindow";
            this.btnClearOutputWindow.Size = new System.Drawing.Size(60, 28);
            this.btnClearOutputWindow.TabIndex = 5;
            this.btnClearOutputWindow.Text = "清空";
            this.btnClearOutputWindow.UseVisualStyleBackColor = true;
            this.btnClearOutputWindow.Click += new System.EventHandler(this.btnClearOutputWindow_Click);
            // 
            // rtbOutputWindow
            // 
            this.rtbOutputWindow.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutputWindow.Location = new System.Drawing.Point(19, 77);
            this.rtbOutputWindow.Name = "rtbOutputWindow";
            this.rtbOutputWindow.Size = new System.Drawing.Size(1368, 518);
            this.rtbOutputWindow.TabIndex = 1;
            this.rtbOutputWindow.Text = "";
            // 
            // msInput
            // 
            this.msInput.AutoSize = false;
            this.msInput.BackColor = System.Drawing.Color.White;
            this.msInput.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.msInput.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.msInput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msInputData,
            this.数据导出ToolStripMenuItem});
            this.msInput.Location = new System.Drawing.Point(0, 0);
            this.msInput.Name = "msInput";
            this.msInput.Size = new System.Drawing.Size(1926, 50);
            this.msInput.TabIndex = 48;
            this.msInput.Text = "menuStrip1";
            // 
            // msInputData
            // 
            this.msInputData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnInputSoilLayersInfo,
            this.btnInputSupportsInfo,
            this.btnInputLoadcasesInfo,
            this.btnInputLocalLoadsInfo,
            this.btnInputOtherInfo});
            this.msInputData.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.msInputData.Name = "msInputData";
            this.msInputData.Size = new System.Drawing.Size(96, 46);
            this.msInputData.Text = "数据录入";
            // 
            // btnInputSoilLayersInfo
            // 
            this.btnInputSoilLayersInfo.Name = "btnInputSoilLayersInfo";
            this.btnInputSoilLayersInfo.Size = new System.Drawing.Size(166, 28);
            this.btnInputSoilLayersInfo.Text = "土层数据";
            this.btnInputSoilLayersInfo.Click += new System.EventHandler(this.btnInputSoilLayersInfo_Click);
            // 
            // btnInputSupportsInfo
            // 
            this.btnInputSupportsInfo.Name = "btnInputSupportsInfo";
            this.btnInputSupportsInfo.Size = new System.Drawing.Size(166, 28);
            this.btnInputSupportsInfo.Text = "支撑数据";
            this.btnInputSupportsInfo.Click += new System.EventHandler(this.btnInputSupportsInfo_Click);
            // 
            // btnInputLoadcasesInfo
            // 
            this.btnInputLoadcasesInfo.Name = "btnInputLoadcasesInfo";
            this.btnInputLoadcasesInfo.Size = new System.Drawing.Size(166, 28);
            this.btnInputLoadcasesInfo.Text = "施工数据";
            this.btnInputLoadcasesInfo.Click += new System.EventHandler(this.btnInputLoadcasesInfo_Click);
            // 
            // btnInputLocalLoadsInfo
            // 
            this.btnInputLocalLoadsInfo.Name = "btnInputLocalLoadsInfo";
            this.btnInputLocalLoadsInfo.Size = new System.Drawing.Size(166, 28);
            this.btnInputLocalLoadsInfo.Text = "局部荷载";
            this.btnInputLocalLoadsInfo.Click += new System.EventHandler(this.btnInputLocalLoadsInfo_Click);
            // 
            // btnInputOtherInfo
            // 
            this.btnInputOtherInfo.Name = "btnInputOtherInfo";
            this.btnInputOtherInfo.Size = new System.Drawing.Size(166, 28);
            this.btnInputOtherInfo.Text = "其他数据";
            this.btnInputOtherInfo.Click += new System.EventHandler(this.btnInputOtherInfo_Click);
            // 
            // 数据导出ToolStripMenuItem
            // 
            this.数据导出ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOutputAll,
            this.btnOutputNodesInfo,
            this.btnOutputElementsInfo,
            this.btnOutputLoadsInfo,
            this.btnOutputResults});
            this.数据导出ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.数据导出ToolStripMenuItem.Name = "数据导出ToolStripMenuItem";
            this.数据导出ToolStripMenuItem.Size = new System.Drawing.Size(96, 46);
            this.数据导出ToolStripMenuItem.Text = "数据导出";
            // 
            // btnOutputAll
            // 
            this.btnOutputAll.Name = "btnOutputAll";
            this.btnOutputAll.Size = new System.Drawing.Size(166, 28);
            this.btnOutputAll.Text = "全部信息";
            this.btnOutputAll.Click += new System.EventHandler(this.btnOutputAll_Click);
            // 
            // btnOutputNodesInfo
            // 
            this.btnOutputNodesInfo.Name = "btnOutputNodesInfo";
            this.btnOutputNodesInfo.Size = new System.Drawing.Size(166, 28);
            this.btnOutputNodesInfo.Text = "节点信息";
            this.btnOutputNodesInfo.Click += new System.EventHandler(this.btnOutputNodesInfo_Click);
            // 
            // btnOutputElementsInfo
            // 
            this.btnOutputElementsInfo.Name = "btnOutputElementsInfo";
            this.btnOutputElementsInfo.Size = new System.Drawing.Size(166, 28);
            this.btnOutputElementsInfo.Text = "单元信息";
            this.btnOutputElementsInfo.Click += new System.EventHandler(this.btnOutputElementsInfo_Click);
            // 
            // btnOutputLoadsInfo
            // 
            this.btnOutputLoadsInfo.Name = "btnOutputLoadsInfo";
            this.btnOutputLoadsInfo.Size = new System.Drawing.Size(166, 28);
            this.btnOutputLoadsInfo.Text = "荷载信息";
            this.btnOutputLoadsInfo.Click += new System.EventHandler(this.btnOutputLoadsInfo_Click);
            // 
            // btnOutputResults
            // 
            this.btnOutputResults.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOutputM,
            this.btnOutputFy,
            this.btnOutputUx,
            this.btnOutputFx});
            this.btnOutputResults.Name = "btnOutputResults";
            this.btnOutputResults.Size = new System.Drawing.Size(166, 28);
            this.btnOutputResults.Text = "计算结果";
            // 
            // btnOutputM
            // 
            this.btnOutputM.Name = "btnOutputM";
            this.btnOutputM.Size = new System.Drawing.Size(202, 28);
            this.btnOutputM.Text = "围护结构弯矩";
            this.btnOutputM.Click += new System.EventHandler(this.btnOutputM_Click);
            // 
            // btnOutputFy
            // 
            this.btnOutputFy.Name = "btnOutputFy";
            this.btnOutputFy.Size = new System.Drawing.Size(202, 28);
            this.btnOutputFy.Text = "围护结构剪力";
            this.btnOutputFy.Click += new System.EventHandler(this.btnOutputFy_Click);
            // 
            // btnOutputUx
            // 
            this.btnOutputUx.Name = "btnOutputUx";
            this.btnOutputUx.Size = new System.Drawing.Size(202, 28);
            this.btnOutputUx.Text = "围护结构位移";
            this.btnOutputUx.Click += new System.EventHandler(this.btnOutputUx_Click);
            // 
            // btnOutputFx
            // 
            this.btnOutputFx.Name = "btnOutputFx";
            this.btnOutputFx.Size = new System.Drawing.Size(202, 28);
            this.btnOutputFx.Text = "支撑轴力";
            this.btnOutputFx.Click += new System.EventHandler(this.btnOutputFx_Click);
            // 
            // Chart
            // 
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.LabelStyle.Format = "N1";
            chartArea1.AxisX.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.None;
            chartArea1.AxisX.ScaleBreakStyle.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.Title = "位移(mm)";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.LabelStyle.Format = "N1";
            chartArea1.AxisY.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.None;
            chartArea1.AxisY.Title = "深度(m)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.Name = "ChartArea1";
            this.Chart.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.Chart.Legends.Add(legend1);
            this.Chart.Location = new System.Drawing.Point(1436, 181);
            this.Chart.Name = "Chart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.Label = "#MIN";
            series1.LabelFormat = "0.00";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.Chart.Series.Add(series1);
            this.Chart.Size = new System.Drawing.Size(461, 538);
            this.Chart.TabIndex = 6;
            this.Chart.Text = "变形图";
            // 
            // cbFigType
            // 
            this.cbFigType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFigType.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFigType.FormattingEnabled = true;
            this.cbFigType.Items.AddRange(new object[] {
            "位移",
            "弯矩",
            "剪力",
            "轴力"});
            this.cbFigType.Location = new System.Drawing.Point(197, 33);
            this.cbFigType.Name = "cbFigType";
            this.cbFigType.Size = new System.Drawing.Size(80, 28);
            this.cbFigType.TabIndex = 3;
            // 
            // cbStage
            // 
            this.cbStage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStage.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbStage.FormattingEnabled = true;
            this.cbStage.Location = new System.Drawing.Point(65, 33);
            this.cbStage.Name = "cbStage";
            this.cbStage.Size = new System.Drawing.Size(66, 28);
            this.cbStage.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 22);
            this.label1.TabIndex = 56;
            this.label1.Text = "第";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 22);
            this.label2.TabIndex = 57;
            this.label2.Text = "阶段";
            // 
            // gbDraw
            // 
            this.gbDraw.Controls.Add(this.cbFigType);
            this.gbDraw.Controls.Add(this.label2);
            this.gbDraw.Controls.Add(this.btnDraw);
            this.gbDraw.Controls.Add(this.cbStage);
            this.gbDraw.Controls.Add(this.label1);
            this.gbDraw.Location = new System.Drawing.Point(1477, 110);
            this.gbDraw.Name = "gbDraw";
            this.gbDraw.Size = new System.Drawing.Size(404, 77);
            this.gbDraw.TabIndex = 58;
            this.gbDraw.TabStop = false;
            this.gbDraw.Text = "绘图区";
            // 
            // btnDraw
            // 
            this.btnDraw.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDraw.Location = new System.Drawing.Point(311, 29);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(71, 36);
            this.btnDraw.TabIndex = 4;
            this.btnDraw.Text = "绘制";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Font = new System.Drawing.Font("黑体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCalculate.Location = new System.Drawing.Point(1574, 31);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(180, 51);
            this.btnCalculate.TabIndex = 1;
            this.btnCalculate.Text = "计算";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1926, 717);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.gbDraw);
            this.Controls.Add(this.Chart);
            this.Controls.Add(this.gbOutputWindow);
            this.Controls.Add(this.msInput);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msInput;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form_Main";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "软土条形深基坑开挖与支撑过程的智能控制计算程序";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbOutputWindow.ResumeLayout(false);
            this.msInput.ResumeLayout(false);
            this.msInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
            this.gbDraw.ResumeLayout(false);
            this.gbDraw.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbOutputWindow;
        private System.Windows.Forms.Button btnWriteLogToFile;
        private System.Windows.Forms.Button btnClearOutputWindow;
        private System.Windows.Forms.MenuStrip msInput;
        private System.Windows.Forms.ToolStripMenuItem msInputData;
        private System.Windows.Forms.ToolStripMenuItem btnInputSupportsInfo;
        private System.Windows.Forms.ToolStripMenuItem btnInputLoadcasesInfo;
        private System.Windows.Forms.ToolStripMenuItem btnInputLocalLoadsInfo;
        private System.Windows.Forms.ToolStripMenuItem btnInputOtherInfo;
        private System.Windows.Forms.ToolStripMenuItem btnInputSoilLayersInfo;
        private System.Windows.Forms.ToolStripMenuItem 数据导出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnOutputNodesInfo;
        private System.Windows.Forms.ToolStripMenuItem btnOutputElementsInfo;
        private System.Windows.Forms.ToolStripMenuItem btnOutputLoadsInfo;
        private System.Windows.Forms.ToolStripMenuItem btnOutputResults;
        private System.Windows.Forms.ToolStripMenuItem btnOutputM;
        private System.Windows.Forms.ToolStripMenuItem btnOutputFy;
        private System.Windows.Forms.ToolStripMenuItem btnOutputFx;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbDraw;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.ToolStripMenuItem btnOutputUx;
        private System.Windows.Forms.ToolStripMenuItem btnOutputAll;
        private System.Windows.Forms.Button btnCalculate;
        public System.Windows.Forms.ComboBox cbFigType;
        public System.Windows.Forms.ComboBox cbStage;
        public System.Windows.Forms.RichTextBox rtbOutputWindow;
    }
}

