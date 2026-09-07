using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ActiveControl.Forms;
using Microsoft.Win32;
using MathNet.Numerics.LinearAlgebra;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace ActiveControl
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)       // 主程序启动
        {
            // 程序调试，默认导入四个数据文件
            string path, line;
            string[] unit;
            char[] deli = { '\t' };

            // 尝试读取土层数据
            try
            {
                path = Application.StartupPath + "\\ImportData\\1-SoilLayersInfo.txt";
                if (File.Exists(path))
                {
                    StreamReader sr1 = new StreamReader(path, Encoding.UTF8);
                    line = sr1.ReadLine();  // 读取第一行

                    // 判断文件格式：检查是否有地下水信息
                    bool hasWaterInfo = false;
                    if (line.StartsWith("EnableWater"))
                    {
                        hasWaterInfo = true;

                        // 读取地下水开关
                        unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        if (unit.Length >= 2)
                        {
                            Loadcase.EnableWater = (unit[1] == "True");
                        }

                        // 读取地下水位标高
                        line = sr1.ReadLine();
                        if (line != null && line.StartsWith("WaterTableElev"))
                        {
                            unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            if (unit.Length >= 2)
                            {
                                Loadcase.WaterTableElev = Convert.ToDouble(unit[1]);
                            }
                        }

                        // 读取土层数据表头
                        line = sr1.ReadLine();
                    }
                    else
                    {
                        // 老格式文件，没有地下水信息
                        Loadcase.EnableWater = false;
                        Loadcase.WaterTableElev = -9999;
                    }

                    // 判断文件格式：新格式（10列）或老格式（9列）
                    bool isNewFormat = (line == "土层编号\t厚度\tc\tphi\tK0\tEs\tm\t重度\t土性\t水土模式");
                    bool isOldFormat = (line == "土层编号\t厚度\tc\tphi\tK0\tEs\tm\t重度\t土性");

                    while (sr1.Peek() > 0)
                    {
                        line = sr1.ReadLine();
                        unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        double Thick = Convert.ToDouble(unit[1]);
                        double C = Convert.ToDouble(unit[2]);
                        double Phi = Convert.ToDouble(unit[3]);
                        double K0 = Convert.ToDouble(unit[4]);
                        double Es = Convert.ToDouble(unit[5]);
                        double M = Convert.ToDouble(unit[6]);
                        double Gamma = Convert.ToDouble(unit[7]);
                        string Type = unit[8];

                        SoilLayer sl;
                        if (isNewFormat && unit.Length >= 10)
                        {
                            // 新格式：读取水土模式
                            string waterSoilMode = unit[9];
                            sl = new SoilLayer(Thick, C, Phi, K0, Es, M, Gamma, Type, waterSoilMode);
                        }
                        else
                        {
                            // 老格式：水土模式设为"自动"
                            sl = new SoilLayer(Thick, C, Phi, K0, Es, M, Gamma, Type, "自动（根据土性）");
                        }
                        SoilLayers.Add(sl);
                    }
                    sr1.Close();
                    rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  土层数据读取成功！\r\n";
                }
                else
                    PrintString("1-SoilLayersInfo.txt文件不存在，请进入土层数据输入窗体手动输入。");
            }
            catch
            {
                PrintString("土层数据读取失败，请进入土层数据输入窗体手动输入。");
            }

            // 尝试读取支撑数据
            try
            {
                path = Application.StartupPath + "\\ImportData\\2-SupportsInfo.txt";
                if (File.Exists(path))
                {
                    StreamReader sr2 = new StreamReader(path, Encoding.UTF8);
                    line = sr2.ReadLine();
                    while (sr2.Peek() > 0)
                    {
                        line = sr2.ReadLine();
                        unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        string Mat = unit[1];
                        double DistToGround = Convert.ToDouble(unit[2]);
                        double HrzDist = Convert.ToDouble(unit[3]);
                        double Size1 = Convert.ToDouble(unit[4]);
                        double Size2 = Convert.ToDouble(unit[5]);
                        double MaxFC = Convert.ToDouble(unit[6]);
                        double MaxFT = Convert.ToDouble(unit[7]);
                        bool AdjAble = unit[8] == "True";
                        double JackStrokeMax = 200.0;
                        if (unit.Length >= 10 && !string.IsNullOrWhiteSpace(unit[9]))
                            JackStrokeMax = Convert.ToDouble(unit[9]);
                        Support sp = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble, JackStrokeMax);
                        Supports.Add(sp);
                    }
                    sr2.Close();
                    rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  支撑数据读取成功！\r\n";
                }
                else
                    PrintString("2-SupportsInfo.txt文件不存在，请进入支撑数据输入窗体手动输入。");
            }
            catch
            {
                PrintString("支撑数据读取失败，请进入支撑数据输入窗体手动输入。");
            }

            // 尝试读取工况数据
            try
            {
                path = Application.StartupPath + "\\ImportData\\3-LoadCasesInfo.txt";
                if (File.Exists(path))
                {
                    StreamReader sr3 = new StreamReader(path, Encoding.UTF8);
                    line = sr3.ReadLine();
                    while (sr3.Peek() > 0)
                    {
                        line = sr3.ReadLine();
                        unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        double ExcavationDepth = Convert.ToDouble(unit[1]);
                        bool IsActiveSupport = unit[2] == "True";
                        Loadcase lc = new Loadcase(ExcavationDepth, IsActiveSupport);
                        Loadcases.Add(lc);
                    }
                    //outputLoadcasesInfoToLV();
                    sr3.Close();
                    rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  工况数据读取成功！\r\n";
                }
                else
                    PrintString("3-LoadCasesInfo.txt文件不存在，请进入工况数据输入窗体手动输入。");
            }
            catch
            {
                PrintString("工况数据读取失败，请进入工况数据输入窗体手动输入。");
            }

            // 尝试读取局部荷载数据
            try
            {
                path = Application.StartupPath + "\\ImportData\\5-LocalLoadsInfo.txt";
                if (File.Exists(path))
                {
                    StreamReader sr5 = new StreamReader(path, Encoding.UTF8);
                    line = sr5.ReadLine();
                    if (line == "荷载编号\t距围护结构距离(m)\t荷载宽度(m)\t局部地面荷载(kPa)")
                    {
                        while (sr5.Peek() > 0)
                        {
                            line = sr5.ReadLine();
                            unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            if (unit.Length >= 4)  // 确保有足够的列
                            {
                                double DistToECS = Convert.ToDouble(unit[1]);
                                double Width = Convert.ToDouble(unit[2]);
                                double LocalGroundLoad = Convert.ToDouble(unit[3]);
                                LocalLoad ll = new LocalLoad(DistToECS, Width, LocalGroundLoad);
                                LocalLoads.Add(ll);
                            }
                        }
                        sr5.Close();
                        rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  局部荷载数据读取成功！共 " + LocalLoads.Count.ToString() + " 个\r\n";
                    }
                    else
                    {
                        sr5.Close();
                        PrintString("5-LocalLoadsInfo.txt文件格式不正确。");
                    }
                }
                else
                    PrintString("5-LocalLoadsInfo.txt文件不存在，如需添加局部荷载，请进入局部荷载数据输入窗体手动输入。");
            }
            catch
            {
                PrintString("局部荷载数据读取失败，请进入局部荷载数据输入窗体手动输入。");
            }

            // 其他数据
            try
            {
                path = Application.StartupPath + "\\ImportData\\4-OtherParas.txt";
                if (File.Exists(path))
                {
                    StreamReader sr4 = new StreamReader(path, Encoding.UTF8);
                    line = sr4.ReadLine();
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); LengthOfECS = Convert.ToDouble(unit[1]);
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); ThickOfECS = Convert.ToDouble(unit[1]);
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); MaxMommentOfECS1 = Convert.ToDouble(unit[1]) * 1e3;
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); MaxMommentOfECS2 = Convert.ToDouble(unit[1]) * 1e3;
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); MaxShearForceOfECS = Convert.ToDouble(unit[1]) * 1e3;
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); LengthOfSupports = Convert.ToDouble(unit[1]);
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); ElevOfCollar = Convert.ToDouble(unit[1]);
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); ElevOfGround = Convert.ToDouble(unit[1]);
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); GroundLoad = Convert.ToDouble(unit[1]);
                    unit = sr4.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries); EpsDefor = Convert.ToDouble(unit[1]);
                    sr4.Close();
                    rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  其他数据读取成功！\r\n";
                }
                else
                    PrintString("4-OtherParas.txt文件不存在，请进入其他数据输入窗体手动输入。");
            }
            catch
            {
                PrintString("其他数据读取失败，请进入其他数据输入窗体手动输入。");
            }

            // 当前开挖面初始化
            Loadcase.CurElev = ElevOfGround;

        }
        public Form_Calculate fmCal;

        // 建立顺序表，存放节点、单元、材料、实常数
        public List<Node> Nodes = new List<Node>();
        public List<Element> Elements = new List<Element>();
        public Matrix<double> DispSum;
        public Matrix<double> AliveSum;
        public Matrix<double> InistrnSum;
        public Matrix<double> IniForceSum;
        public Vector<double> JackStrokeCurrent;  // 当前各支撑千斤顶累计行程，单位m
        public Matrix<double> JackStrokeSum;      // 各施工阶段结束时的累计行程

        // 建立顺序表，存放土层、支撑信息、施工顺序
        public List<Support> Supports = new List<Support>();
        public List<Loadcase> Loadcases = new List<Loadcase>();
        public List<SoilLayer> SoilLayers = new List<SoilLayer>();
        public List<LocalLoad> LocalLoads = new List<LocalLoad>();

        // 内力包络
        public Vector<double> UxMax;
        public Vector<double> UxMin;
        public Vector<double> MomMax;
        public Vector<double> MomMin;
        public Vector<double> FxMax;
        public Vector<double> FxMin;
        public Vector<double> FyMax;
        public Vector<double> FyMin;

        // 其他数据
        public double LengthOfECS;                         // 围护结构长度
        public double ThickOfECS;                          // 围护结构厚度
        public double LengthOfSupports;                    // 支撑长度，取半结构
        public double ElevOfCollar;                        // 钻孔孔口标高
        public double ElevOfGround;                        // 开挖地面标高
        public double GroundLoad;                          // 地面超载，假设与围护结构距离为0
        public bool EnableWater;                           // 是否考虑地下水
        public double WaterTableElev;                      // 地下水位标高
        public double EpsDefor;                            // 侧向变形限值与基坑深度的比值
        public double MaxMommentOfECS1;                    // 围护结构迎土侧极限弯矩
        public double MaxMommentOfECS2;                    // 围护结构背土侧极限弯矩
        public double MaxShearForceOfECS;                  // 围护结构极限剪力

        // 计算线程控制
        public Thread Calculate;
        public ManualResetEvent mre = new ManualResetEvent(true);

        private void btnCalculate_Click(object sender, EventArgs e)  // 启动计算
        {
            fmCal = new Form_Calculate(this) { Owner = this };
            fmCal.Show();
        }

        public List<Node> NodesDistinct(List<Node> Nodes)            // 【方法】节点列表去重
        {
            List<Node> newNodes = new List<Node>();
            for (int i = 0; i < Nodes.Count(); i++)
            {
                bool flag = false;     // 标记是否重复
                for (int j = 0; j < i; j++)
                {
                    if (Math.Abs(Nodes[j].Nx - Nodes[i].Nx) < 1e-5 && Math.Abs(Nodes[j].Ny - Nodes[i].Ny) < 1e-5)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    newNodes.Add(Nodes[i]);
            }
            return newNodes;
        }
        public void PrintString(string str)                          // 【方法】在输出窗口中打印字符串
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                rtbOutputWindow.AppendText(">> " + System.DateTime.Now.ToString() + "  " + str + "\r\n");
                rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
                rtbOutputWindow.ScrollToCaret();
            }));
        }

        // 【按钮】绘图及其子函数
        public void btnDraw_Click(object sender, EventArgs e)        // 【按钮】触发绘图主函数
        {
            //if (Calculate.ThreadState == ThreadState.Background)
            //    return;
            //if (Calculate.ThreadState == (ThreadState.Background | ThreadState.WaitSleepJoin))
            //    return;
            if (Calculate == null)
                return;

            if (Calculate.ThreadState == ThreadState.Background)
                return;
            if (Calculate.ThreadState == (ThreadState.Background | ThreadState.WaitSleepJoin))
                return;

            // 调取当前状态
            if (cbStage.Text == "包络")
            {
                // 绘制图像
                switch (cbFigType.Text)
                {
                    case "位移":
                        //UpdateDefChart_Env();
                        UpdateDefChart_FullyEnv();
                        break;
                    case "弯矩":
                        //UpdateMomChart_Env();
                        UpdateMomChart_FullyEnv();
                        break;
                    case "剪力":
                        //UpdateFyChart_Env();
                        UpdateFyChart_FullyEnv();
                        break;
                    case "轴力":
                        //UpdateFyChart_Env();
                        UpdateFxChart_FullyEnv();
                        break;
                }
            }
            else
            {
                int Stage = int.Parse(cbStage.Text) - 1;
                for (int i = 0; i < Nodes.Count(); i++)         // 还原节点位移
                    Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);
                for (int i = 0; i < Elements.Count(); i++)      // 还原支撑单元初应变
                    Elements[i].RealConstant.IniStrn = InistrnSum[i, Stage];
                // 还原单元生死状态
                int ActSupCount = 0;
                for (int i = 0; i < Stage + 1; i++)
                {
                    if (Loadcases[i].IsActiveSupport == true)
                    {
                        Elements[fmCal.CM_Elem_Supports[ActSupCount]].isAlive = true;
                        ActSupCount++;
                    }
                }
                for (int i = Stage + 1; i < Loadcases.Count(); i++)
                {
                    if (Loadcases[i].IsActiveSupport == true)
                    {
                        Elements[fmCal.CM_Elem_Supports[ActSupCount]].isAlive = false;
                        ActSupCount++;
                    }
                }

                // 绘制图像
                switch (cbFigType.Text)
                {
                    case "位移":
                        UpdateDefChart();
                        break;
                    case "弯矩":
                        UpdateMomChart();
                        break;
                    case "剪力":
                        UpdateFyChart();
                        break;
                    case "轴力":
                        UpdateFxChart();
                        break;
                }
            }
        }
        public void UpdateDefChart()                                 // 【方法】绘制给定阶段围护结构变形图
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                //Chart.Series.Clear();

                //// 围护结构变形曲线
                //Series Ux = new Series
                //{
                //    ChartType = SeriesChartType.Line,
                //    BorderWidth = 2,
                //    Color = Color.LimeGreen
                //};
                //foreach (Node node in Nodes)
                //    if (node.Nx == 0)
                //        Ux.Points.AddXY(node.Ux * 1e3, node.Ny);
                //Chart.Series.Add(Ux);

                //// 变形前曲线
                //Series ZeroLine = new Series
                //{
                //    ChartType = SeriesChartType.Line,
                //    BorderWidth = 2,
                //    BorderDashStyle = ChartDashStyle.Dash,
                //    Color = Color.Gray
                //};
                //ZeroLine.Points.AddXY(0, Nodes.Max(o => o.Ny));
                //ZeroLine.Points.AddXY(0, Nodes.Min(o => o.Ny));
                //Chart.Series.Add(ZeroLine);

                ////    // 支撑位置标记
                ////    foreach (Element element in Elements)
                ////    {
                ////        if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                ////        {
                ////            double supportDispMm = element.Right.Ux * 1e3;
                ////            // 数值保护：检查支撑位移是否异常
                ////            if (double.IsNaN(supportDispMm) || double.IsInfinity(supportDispMm))
                ////            {
                ////                continue; // 跳过异常值
                ////            }
                ////            // 限制支撑位移显示范围
                ////            supportDispMm = Math.Max(-10000, Math.Min(10000, supportDispMm));

                ////            System.Windows.Forms.DataVisualization.Charting.Series Support = new System.Windows.Forms.DataVisualization.Charting.Series
                ////            {
                ////                ChartType = SeriesChartType.Line,
                ////                BorderWidth = 2,
                ////                Color = Color.Black
                ////            };
                ////            Support.Points.AddXY(element.Left.Nx * 1e3, element.Left.Ny);
                ////            Support.Points.AddXY(supportDispMm, element.Right.Ny);
                ////            Chart.Series.Add(Support);
                ////        }
                ////    }

                ////    // 更新坐标轴范围
                ////    Chart.ChartAreas[0].AxisX.Maximum = +LengthOfECS * EpsDefor * 1 * 1e3;
                ////    Chart.ChartAreas[0].AxisX.Minimum = -LengthOfECS * EpsDefor * 1 * 1e3;
                ////    Chart.ChartAreas[0].AxisY.Maximum = Nodes.Max(o => o.Ny);
                ////    Chart.ChartAreas[0].AxisY.Minimum = Nodes.Min(o => o.Ny);

                ////    Chart.ChartAreas[0].Axes[0].Title = "位移(mm)";
                ////}
                ////catch (OverflowException ex)
                ////{
                ////    PrintString("图表更新溢出错误：" + ex.Message);
                ////}
                ////catch (Exception ex)
                ////{
                ////    PrintString("图表更新异常：" + ex.Message);
                ////}
                //try
                //{
                //    // 支撑位置标记
                //    foreach (Element element in Elements)
                //    {
                //        if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                //        {
                //            double supportDispMm = element.Right.Ux * 1e3;

                //            // 数值保护：检查支撑位移是否异常
                //            if (double.IsNaN(supportDispMm) || double.IsInfinity(supportDispMm))
                //                continue;

                //            // 限制支撑位移显示范围
                //            supportDispMm = Math.Max(-10000, Math.Min(10000, supportDispMm));

                //            Series Support = new Series
                //            {
                //                ChartType = SeriesChartType.Line,
                //                BorderWidth = 2,
                //                Color = Color.Black
                //            };
                //            Support.Points.AddXY(element.Left.Nx * 1e3, element.Left.Ny);
                //            Support.Points.AddXY(supportDispMm, element.Right.Ny);
                //            Chart.Series.Add(Support);
                //        }
                //    }

                //    // 更新坐标轴范围
                //    Chart.ChartAreas[0].AxisX.Maximum = +LengthOfECS * EpsDefor * 1 * 1e3;
                //    Chart.ChartAreas[0].AxisX.Minimum = -LengthOfECS * EpsDefor * 1 * 1e3;
                //    Chart.ChartAreas[0].AxisY.Maximum = Nodes.Max(o => o.Ny);
                //    Chart.ChartAreas[0].AxisY.Minimum = Nodes.Min(o => o.Ny);

                //    Chart.ChartAreas[0].Axes[0].Title = "位移(mm)";
                //}
                //catch (OverflowException ex)
                //{
                //    PrintString("图表更新溢出错误：" + ex.Message);
                //}
                //catch (Exception ex)
                //{
                //    PrintString("图表更新异常：" + ex.Message);
                //}
                Chart.Series.Clear();

                try
                {
                    // 围护结构变形曲线
                    Series Ux = new Series
                    {
                        ChartType = SeriesChartType.Line,
                        BorderWidth = 2,
                        Color = Color.LimeGreen
                    };

                    foreach (Node node in Nodes)
                    {
                        if (node.Nx != 0) continue;

                        double x = node.Ux * 1e3;   // mm
                        double y = node.Ny;

                        // 数值保护：任何 NaN / Infinity 都不能进 Chart
                        if (double.IsNaN(x) || double.IsInfinity(x)) continue;
                        if (double.IsNaN(y) || double.IsInfinity(y)) continue;

                        // 可选：限制极端值，避免坐标轴内部溢出
                        x = Math.Max(-1e6, Math.Min(1e6, x));
                        y = Math.Max(-1e6, Math.Min(1e6, y));

                        Ux.Points.AddXY(x, y);
                    }
                    Chart.Series.Add(Ux);

                    // 变形前曲线（同样做保护：Nodes 为空会崩）
                    if (Nodes.Count > 0)
                    {
                        double yMax = Nodes.Max(o => o.Ny);
                        double yMin = Nodes.Min(o => o.Ny);

                        if (!double.IsNaN(yMax) && !double.IsInfinity(yMax) &&
                            !double.IsNaN(yMin) && !double.IsInfinity(yMin))
                        {
                            Series ZeroLine = new Series
                            {
                                ChartType = SeriesChartType.Line,
                                BorderWidth = 2,
                                BorderDashStyle = ChartDashStyle.Dash,
                                Color = Color.Gray
                            };
                            ZeroLine.Points.AddXY(0, yMax);
                            ZeroLine.Points.AddXY(0, yMin);
                            Chart.Series.Add(ZeroLine);
                        }
                    }

                    // 支撑位置标记（你原来这里已经有保护，我保留并建议再补 y 的保护）
                    foreach (Element element in Elements)
                    {
                        if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                        {
                            double supportDispMm = element.Right.Ux * 1e3;
                            double y1 = element.Left.Ny;
                            double y2 = element.Right.Ny;

                            if (double.IsNaN(supportDispMm) || double.IsInfinity(supportDispMm)) continue;
                            if (double.IsNaN(y1) || double.IsInfinity(y1)) continue;
                            if (double.IsNaN(y2) || double.IsInfinity(y2)) continue;

                            supportDispMm = Math.Max(-10000, Math.Min(10000, supportDispMm));

                            Series Support = new Series
                            {
                                ChartType = SeriesChartType.Line,
                                BorderWidth = 2,
                                Color = Color.Black
                            };
                            Support.Points.AddXY(element.Left.Nx * 1e3, y1);
                            Support.Points.AddXY(supportDispMm, y2);
                            Chart.Series.Add(Support);
                        }
                    }

                    // 坐标轴范围（也要保护，避免 max/min = NaN）
                    double xMax = +LengthOfECS * EpsDefor * 1e3;
                    double xMin = -LengthOfECS * EpsDefor * 1e3;

                    if (!double.IsNaN(xMax) && !double.IsInfinity(xMax) &&
                        !double.IsNaN(xMin) && !double.IsInfinity(xMin))
                    {
                        Chart.ChartAreas[0].AxisX.Maximum = xMax;
                        Chart.ChartAreas[0].AxisX.Minimum = xMin;
                    }

                    if (Nodes.Count > 0)
                    {
                        double yMax = Nodes.Max(o => o.Ny);
                        double yMin = Nodes.Min(o => o.Ny);

                        if (!double.IsNaN(yMax) && !double.IsInfinity(yMax) &&
                            !double.IsNaN(yMin) && !double.IsInfinity(yMin))
                        {
                            Chart.ChartAreas[0].AxisY.Maximum = yMax;
                            Chart.ChartAreas[0].AxisY.Minimum = yMin;
                        }
                    }

                    Chart.ChartAreas[0].Axes[0].Title = "位移(mm)";
                }
                catch (OverflowException ex)
                {
                    PrintString("图表更新溢出错误：" + ex.Message);
                }
                catch (Exception ex)
                {
                    PrintString("图表更新异常：" + ex.Message);
                }


                //Ux.Points.ResumeUpdates();

            }));
        }
        public void UpdateDefChart_Env()                             // 【方法】绘制围护结构变形包络（本函数未考虑轴力调整过程中的位移，已弃用）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算包络数据
                List<int> Index = new List<int>();
                List<double> Ny = new List<double>();

                for (int i = 0; i < Nodes.Count(); i++)
                {
                    if (Nodes[i].Nx == 0)
                    {
                        Index.Add(i);
                        Ny.Add(Nodes[i].Ny);
                    }
                }
                List<double> UxMaxData = new List<double>();
                List<double> UxMinData = new List<double>();
                foreach(int i in Index)
                {
                    UxMaxData.Add(DispSum[i * 3, 0]);
                    UxMinData.Add(DispSum[i * 3, 0]);
                }

                for (int Stage = 1; Stage < DispSum.ColumnCount; Stage++)
                {
                    for (int i = 0; i < Index.Count(); i++) 
                    {
                        if (DispSum[Index[i] * 3, Stage] > UxMaxData[i])
                            UxMaxData[i] = DispSum[Index[i] * 3, Stage];
                        if (DispSum[Index[i] * 3, Stage] < UxMinData[i])
                            UxMinData[i] = DispSum[Index[i] * 3, Stage];
                    }
                }

                // 围护结构变形曲线
                Series UxMax = new Series();
                Series UxMin = new Series();
                UxMax.ChartType = SeriesChartType.Line;
                UxMin.ChartType = SeriesChartType.Line;
                UxMax.BorderWidth = 2;
                UxMin.BorderWidth = 2;
                UxMax.Color = Color.BlueViolet;
                UxMin.Color = Color.Red;
                for (int i = 0; i < Index.Count(); i++)
                {
                    UxMax.Points.AddXY(UxMaxData[i] * 1e3, Ny[i]);
                    UxMin.Points.AddXY(UxMinData[i] * 1e3, Ny[i]);
                }
                Chart.Series.Add(UxMax);
                Chart.Series.Add(UxMin);

                // 变形前曲线
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            BorderDashStyle = ChartDashStyle.Dash,
                            Color = Color.Gray
                        };
                        Support.Points.AddXY(element.Left.Nx * 1e3, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (UxMaxData.Max() + (UxMaxData.Max() - UxMinData.Min()) * 0.1) * 1e3;
                Chart.ChartAreas[0].AxisX.Minimum = (UxMinData.Min() - (UxMaxData.Max() - UxMinData.Min()) * 0.1) * 1e3; 
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "位移(mm)";

            }));
        }
        public void UpdateDefChart_FullyEnv()                        // 【方法】绘制围护结构变形包络（考虑轴力调整过程中的位移）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算 y 坐标
                List<double> Ny = new List<double>();
                for (int i = 0; i < Nodes.Count(); i++)
                    if (Nodes[i].Nx == 0)
                        Ny.Add(Nodes[i].Ny);

                // 围护结构变形曲线
                Series UxMaxLine = new Series();
                Series UxMinLine = new Series();
                UxMaxLine.ChartType = SeriesChartType.Line;
                UxMinLine.ChartType = SeriesChartType.Line;
                UxMaxLine.BorderWidth = 2;
                UxMinLine.BorderWidth = 2;
                UxMaxLine.Color = Color.BlueViolet;
                UxMinLine.Color = Color.Red;
                for (int i = 0; i < UxMax.Count(); i++)
                {
                    UxMaxLine.Points.AddXY(UxMax[i] * 1e3, Ny[i]);
                    UxMinLine.Points.AddXY(UxMin[i] * 1e3, Ny[i]);
                }
                Chart.Series.Add(UxMaxLine);
                Chart.Series.Add(UxMinLine);

                // 最值点标记
                Series MPoint = new Series
                {
                    ChartType = SeriesChartType.Point,
                    MarkerBorderColor = Color.Red,
                    MarkerColor = Color.Salmon,
                    MarkerStyle = MarkerStyle.Square,
                    Label = "#VALX{N1}",
                    Font = new Font("Times New Roman", 10, FontStyle.Regular)
                };
                MPoint.Points.AddXY(UxMax.Max() * 1e3, Ny[UxMax.Find(o => o == UxMax.Max()).Item1]);
                MPoint.Points.AddXY(UxMin.Min() * 1e3, Ny[UxMin.Find(o => o == UxMin.Min()).Item1]);
                Chart.Series.Add(MPoint);

                // 变形前曲线
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            BorderDashStyle = ChartDashStyle.Dash,
                            Color = Color.Gray
                        };
                        Support.Points.AddXY(element.Left.Nx * 1e3, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (UxMax.Max() + (UxMax.Max() - UxMin.Min()) * 0.1) * 1e3;
                Chart.ChartAreas[0].AxisX.Minimum = (UxMin.Min() - (UxMax.Max() - UxMin.Min()) * 0.1) * 1e3;
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "位移(mm)";

            }));
        }
        public void UpdateMomChart()                                 // 【方法】绘制给定阶段围护结构弯矩图
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 围护结构弯矩曲线
                Series Mom = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    Color = Color.Red
                };
                foreach (Element element in Elements)
                {
                    element.getNodalForce();
                    if (element.Left.Nx == 0 && element.Right.Nx == 0)
                        Mom.Points.AddXY(element.iMom * 1e-3, element.Left.Ny);
                }
                Chart.Series.Add(Mom);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Nodes.Max(o => o.Ny));
                ZeroLine.Points.AddXY(0, Nodes.Min(o => o.Ny));
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            Color = Color.Black
                        };
                        Support.Points.AddXY(-1e10, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = Math.Max((Elements.Max(o => o.iMom) + (Elements.Max(o => o.iMom) - Elements.Min(o => o.iMom)) * 0.1) * 1e-3, 100);
                Chart.ChartAreas[0].AxisX.Minimum = Math.Min((Elements.Min(o => o.iMom) - (Elements.Max(o => o.iMom) - Elements.Min(o => o.iMom)) * 0.1) * 1e-3, -100);
                Chart.ChartAreas[0].AxisY.Maximum = Nodes.Max(o => o.Ny);
                Chart.ChartAreas[0].AxisY.Minimum = Nodes.Min(o => o.Ny);

                Chart.ChartAreas[0].Axes[0].Title = "弯矩(kN·m/m)";

                //Ux.Points.ResumeUpdates();

            }));
        }
        public void UpdateMomChart_Env()                             // 【方法】绘制围护结构弯矩包络（本函数未考虑轴力调整过程中的弯矩，已弃用）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算包络数据
                List<int> Index = new List<int>();
                List<double> Ny = new List<double>();

                for (int i = 0; i < Elements.Count(); i++)
                {
                    if (Elements[i].Left.Nx == 0 && Elements[i].Right.Nx == 0)
                    {
                        Index.Add(i);
                        Ny.Add(Elements[i].Left.Ny);
                    }
                }
                for (int i = 0; i < Nodes.Count(); i++)
                    Nodes[i].LoadDisp(DispSum[i * 3, 0], DispSum[i * 3 + 1, 0], DispSum[i * 3 + 2, 0]);
                List<double> MomMaxData = new List<double>();
                List<double> MomMinData = new List<double>();
                foreach (int i in Index)
                {
                    MomMaxData.Add(Elements[i].iMom);
                    MomMinData.Add(Elements[i].iMom);
                }

                for (int Stage = 1; Stage < DispSum.ColumnCount; Stage++)
                {
                    for (int i = 0; i < Nodes.Count(); i++)
                        Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);
                    for (int i = 0; i < Index.Count(); i++)
                    {
                        Elements[Index[i]].getNodalForce();
                        if (Elements[Index[i]].iMom > MomMaxData[i])
                            MomMaxData[i] = Elements[Index[i]].iMom;
                        if (Elements[Index[i]].iMom < MomMinData[i])
                            MomMinData[i] = Elements[Index[i]].iMom;
                    }
                }

                // 围护结构变形曲线
                Series MomMax = new Series();
                Series MomMin = new Series();
                MomMax.ChartType = SeriesChartType.Line;
                MomMin.ChartType = SeriesChartType.Line;
                MomMax.BorderWidth = 2;
                MomMin.BorderWidth = 2;
                MomMax.Color = Color.BlueViolet;
                MomMin.Color = Color.Red;
                for (int i = 0; i < Index.Count(); i++)
                {
                    MomMax.Points.AddXY(MomMaxData[i] * 1e-3, Ny[i]);
                    MomMin.Points.AddXY(MomMinData[i] * 1e-3, Ny[i]);
                }
                Chart.Series.Add(MomMax);
                Chart.Series.Add(MomMin);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            BorderDashStyle = ChartDashStyle.Dash,
                            Color = Color.Gray
                        };
                        Support.Points.AddXY(element.Left.Nx * 1e3, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (MomMaxData.Max() + (MomMaxData.Max() - MomMinData.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisX.Minimum = (MomMinData.Min() - (MomMaxData.Max() - MomMinData.Min()) * 0.1) * 1e-3; 
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "弯矩(kN·m)";

            }));
        }
        public void UpdateMomChart_FullyEnv()                        // 【方法】绘制围护结构弯矩包络（考虑轴力调整过程中的弯矩）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算 y 坐标
                List<double> Ny = new List<double>();
                for (int i = 0; i < Elements.Count(); i++)
                    if (Elements[i].Left.Nx == 0 && Elements[i].Right.Nx == 0)
                        Ny.Add(Elements[i].Left.Ny);

                // 围护结构变形曲线
                Series MomMaxLine = new Series();
                Series MomMinLine = new Series();
                MomMaxLine.ChartType = SeriesChartType.Line;
                MomMinLine.ChartType = SeriesChartType.Line;
                MomMaxLine.BorderWidth = 2;
                MomMinLine.BorderWidth = 2;
                MomMaxLine.Color = Color.BlueViolet;
                MomMinLine.Color = Color.Red;
                for (int i = 0; i < MomMax.Count(); i++)
                {
                    MomMaxLine.Points.AddXY(MomMax[i] * 1e-3, Ny[i]);
                    MomMinLine.Points.AddXY(MomMin[i] * 1e-3, Ny[i]);
                }
                Chart.Series.Add(MomMaxLine);
                Chart.Series.Add(MomMinLine);

                // 最值点标记
                Series MPoint = new Series
                {
                    ChartType = SeriesChartType.Point,
                    MarkerBorderColor = Color.Red,
                    MarkerColor = Color.Salmon,
                    MarkerStyle = MarkerStyle.Square,
                    Label = "#VALX{N1}",
                    Font = new Font("Times New Roman", 10, FontStyle.Regular)
                };
                MPoint.Points.AddXY(MomMax.Max() * 1e-3, Ny[MomMax.Find(o => o == MomMax.Max()).Item1]);
                MPoint.Points.AddXY(MomMin.Min() * 1e-3, Ny[MomMin.Find(o => o == MomMin.Min()).Item1]);
                Chart.Series.Add(MPoint);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            BorderDashStyle = ChartDashStyle.Dash,
                            Color = Color.Gray
                        };
                        Support.Points.AddXY(element.Left.Nx * 1e3, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (MomMax.Max() + (MomMax.Max() - MomMin.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisX.Minimum = (MomMin.Min() - (MomMax.Max() - MomMin.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "弯矩(kN·m/m)";

            }));
        }
        public void UpdateFyChart()                                  // 【方法】绘制给定阶段围护结构剪力图
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 围护结构剪力曲线
                Series Fy = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    Color = Color.Red
                };
                foreach (Element element in Elements)
                {
                    element.getNodalForce();
                    if (element.Left.Nx == 0 && element.Right.Nx == 0)
                        Fy.Points.AddXY(element.iFy * 1e-3, element.Left.Ny);
                }
                Chart.Series.Add(Fy);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Nodes.Max(o => o.Ny));
                ZeroLine.Points.AddXY(0, Nodes.Min(o => o.Ny));
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            Color = Color.Black
                        };
                        Support.Points.AddXY(-1e10, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = Math.Max((Elements.Max(o => o.iFy) + (Elements.Max(o => o.iFy) - Elements.Min(o => o.iFy)) * 0.1) * 1e-3, 100);
                Chart.ChartAreas[0].AxisX.Minimum = Math.Min((Elements.Min(o => o.iFy) - (Elements.Max(o => o.iFy) - Elements.Min(o => o.iFy)) * 0.1) * 1e-3, -100);
                Chart.ChartAreas[0].AxisY.Maximum = Nodes.Max(o => o.Ny);
                Chart.ChartAreas[0].AxisY.Minimum = Nodes.Min(o => o.Ny);

                Chart.ChartAreas[0].Axes[0].Title = "剪力(kN/m)";

            }));
        }
        public void UpdateFyChart_Env()                              // 【方法】绘制围护结构剪力包络（本函数未考虑轴力调整过程中的剪力，已弃用）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算包络数据
                List<int> Index = new List<int>();
                List<double> Ny = new List<double>();

                for (int i = 0; i < Elements.Count(); i++)
                {
                    if (Elements[i].Left.Nx == 0 && Elements[i].Right.Nx == 0)
                    {
                        Index.Add(i);
                        Ny.Add(Elements[i].Left.Ny);
                    }
                }
                for (int i = 0; i < Nodes.Count(); i++)
                    Nodes[i].LoadDisp(DispSum[i * 3, 0], DispSum[i * 3 + 1, 0], DispSum[i * 3 + 2, 0]);
                List<double> FyMaxData = new List<double>();
                List<double> FyMinData = new List<double>();
                foreach (int i in Index)
                {
                    FyMaxData.Add(Elements[i].iFy);
                    FyMinData.Add(Elements[i].iFy);
                }

                for (int Stage = 1; Stage < DispSum.ColumnCount; Stage++)
                {
                    for (int i = 0; i < Nodes.Count(); i++)
                        Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);
                    for (int i = 0; i < Index.Count(); i++)
                    {
                        Elements[Index[i]].getNodalForce();
                        if (Elements[Index[i]].iFy > FyMaxData[i])
                            FyMaxData[i] = Elements[Index[i]].iFy;
                        if (Elements[Index[i]].iFy < FyMinData[i])
                            FyMinData[i] = Elements[Index[i]].iFy;
                    }
                }

                // 围护结构变形曲线
                Series FyMax = new Series();
                Series FyMin = new Series();
                FyMax.ChartType = SeriesChartType.Line;
                FyMin.ChartType = SeriesChartType.Line;
                FyMax.BorderWidth = 2;
                FyMin.BorderWidth = 2;
                FyMax.Color = Color.BlueViolet;
                FyMin.Color = Color.Red;
                for (int i = 0; i < Index.Count(); i++)
                {
                    FyMax.Points.AddXY(FyMaxData[i] * 1e-3, Ny[i]);
                    FyMin.Points.AddXY(FyMinData[i] * 1e-3, Ny[i]);
                }
                Chart.Series.Add(FyMax);
                Chart.Series.Add(FyMin);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            BorderDashStyle = ChartDashStyle.Dash,
                            Color = Color.Gray
                        };
                        Support.Points.AddXY(FyMinData.Min() * 1.2e-3, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (FyMaxData.Max() + (FyMaxData.Max() - FyMinData.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisX.Minimum = (FyMinData.Min() - (FyMaxData.Max() - FyMinData.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "剪力(kN)";

            }));
        }
        public void UpdateFyChart_FullyEnv()                         // 【方法】绘制围护结构剪力包络（考虑轴力调整过程中的剪力）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算 y 坐标
                List<double> Ny = new List<double>();
                for (int i = 0; i < Elements.Count(); i++)
                    if (Elements[i].Left.Nx == 0 && Elements[i].Right.Nx == 0)
                        Ny.Add(Elements[i].Left.Ny);

                // 围护结构变形曲线
                Series FyMaxLine = new Series();
                Series FyMinLine = new Series();
                FyMaxLine.ChartType = SeriesChartType.Line;
                FyMinLine.ChartType = SeriesChartType.Line;
                FyMaxLine.BorderWidth = 2;
                FyMinLine.BorderWidth = 2;
                FyMaxLine.Color = Color.BlueViolet;
                FyMinLine.Color = Color.Red;
                for (int i = 0; i < FyMax.Count(); i++)
                {
                    FyMaxLine.Points.AddXY(FyMax[i] * 1e-3, Ny[i]);
                    FyMinLine.Points.AddXY(FyMin[i] * 1e-3, Ny[i]);
                }
                Chart.Series.Add(FyMaxLine);
                Chart.Series.Add(FyMinLine);

                // 最值点标记
                Series MPoint = new Series
                {
                    ChartType = SeriesChartType.Point,
                    MarkerBorderColor = Color.Red,
                    MarkerColor = Color.Salmon,
                    MarkerStyle = MarkerStyle.Square,
                    Label = "#VALX{N1}",
                    Font = new Font("Times New Roman", 10, FontStyle.Regular)
                };
                MPoint.Points.AddXY(FyMax.Max() * 1e-3, Ny[FyMax.Find(o => o == FyMax.Max()).Item1]);
                MPoint.Points.AddXY(FyMin.Min() * 1e-3, Ny[FyMin.Find(o => o == FyMin.Min()).Item1]);
                Chart.Series.Add(MPoint);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            BorderDashStyle = ChartDashStyle.Dash,
                            Color = Color.Gray
                        };
                        Support.Points.AddXY(FyMin.Min() * 1.2e-3, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (FyMax.Max() + (FyMax.Max() - FyMin.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisX.Minimum = (FyMin.Min() - (FyMax.Max() - FyMin.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "剪力(kN/m)";

            }));
        }
        public void UpdateFxChart()                                  // 【方法】绘制给定阶段支撑轴力图
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 围护结构轴力曲线
                Series Fx = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    Color = Color.Red
                };
                for (int i = 0; i < fmCal.CM_Elem_Supports.Count(); i++)
                {
                    Elements[fmCal.CM_Elem_Supports[i]].getNodalForce();
                    Fx.Points.AddXY(Elements[fmCal.CM_Elem_Supports[i]].iFx * 1e-3, Elements[fmCal.CM_Elem_Supports[i]].Left.Ny);
                }
                Chart.Series.Add(Fx);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Nodes.Max(o => o.Ny));
                ZeroLine.Points.AddXY(0, Nodes.Min(o => o.Ny));
                Chart.Series.Add(ZeroLine);

                // 支撑位置标记
                foreach (Element element in Elements)
                {
                    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                    {
                        Series Support = new Series
                        {
                            ChartType = SeriesChartType.Line,
                            BorderWidth = 2,
                            Color = Color.Black
                        };
                        Support.Points.AddXY(-1e10, element.Left.Ny);
                        Support.Points.AddXY(0, element.Right.Ny);
                        Chart.Series.Add(Support);
                    }
                }

                // 以下四行在绘图区以外添加点，避免数据点横坐标为0时曲线乱飘
                Series tempSeries = new Series
                {
                    ChartType = SeriesChartType.Line
                };
                tempSeries.Points.AddXY(-1e10, 0);
                Chart.Series.Add(tempSeries);

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = Math.Max((Elements.Max(o => o.iFx) + (Elements.Max(o => o.iFx) - Elements.Min(o => o.iFx)) * 0.1) * 1e-3, 100);
                Chart.ChartAreas[0].AxisX.Minimum = Math.Min((Elements.Min(o => o.iFx) - (Elements.Max(o => o.iFx) - Elements.Min(o => o.iFx)) * 0.1) * 1e-3, -100);
                Chart.ChartAreas[0].AxisY.Maximum = Nodes.Max(o => o.Ny);
                Chart.ChartAreas[0].AxisY.Minimum = Nodes.Min(o => o.Ny);

                Chart.ChartAreas[0].Axes[0].Title = "轴力(kN/m)";

            }));
        }
        public void UpdateFxChart_FullyEnv()                         // 【方法】绘制支撑轴力包络（考虑轴力调整过程中的支撑轴力）
        {
            mre.WaitOne();
            Invoke(new Action(() =>
            {
                Chart.Series.Clear();

                // 计算 y 坐标
                List<double> Ny = new List<double>();
                for (int i = 0; i < Nodes.Count(); i++)
                    if (Nodes[i].Nx == -LengthOfSupports) 
                        Ny.Add(Nodes[i].Ny);

                // 围护结构轴力曲线
                Series FxMaxLine = new Series();
                Series FxMinLine = new Series();
                FxMaxLine.ChartType = SeriesChartType.Line;
                FxMinLine.ChartType = SeriesChartType.Line;
                FxMaxLine.BorderWidth = 2;
                FxMinLine.BorderWidth = 2;
                FxMaxLine.Color = Color.BlueViolet;
                FxMinLine.Color = Color.Red;
                for (int i = 0; i < FxMax.Count(); i++)
                {
                    FxMaxLine.Points.AddXY(FxMax[i] * 1e-3, Ny[i]);
                    FxMinLine.Points.AddXY(FxMin[i] * 1e-3, Ny[i]);
                }
                Chart.Series.Add(FxMaxLine);
                Chart.Series.Add(FxMinLine);

                // 最值点标记
                Series MPoint = new Series
                {
                    ChartType = SeriesChartType.Point,
                    MarkerBorderColor = Color.Red,
                    MarkerColor = Color.Salmon,
                    MarkerStyle = MarkerStyle.Square,
                    Label = "#VALX{N1}",
                    Font = new Font("Times New Roman", 10, FontStyle.Regular)
                };
                MPoint.Points.AddXY(FxMax.Max() * 1e-3, Ny[FxMax.Find(o => o == FxMax.Max()).Item1]);
                MPoint.Points.AddXY(FxMin.Min() * 1e-3, Ny[FxMin.Find(o => o == FxMin.Min()).Item1]);
                Chart.Series.Add(MPoint);

                // 零位置标记
                Series ZeroLine = new Series
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    Color = Color.Gray
                };
                ZeroLine.Points.AddXY(0, Ny.Max());
                ZeroLine.Points.AddXY(0, Ny.Min());
                Chart.Series.Add(ZeroLine);

                //// 支撑位置标记
                //foreach (Element element in Elements)
                //{
                //    if (element.Left.Nx == -LengthOfSupports && element.isAlive)
                //    {
                //        Series Support = new Series();
                //        Support.ChartType = SeriesChartType.Line;
                //        Support.BorderWidth = 2;
                //        Support.BorderDashStyle = ChartDashStyle.Dash;
                //        Support.Color = Color.Gray;
                //        Support.Points.AddXY(FyMin.Min() * 1.2e-3, element.Left.Ny);
                //        Support.Points.AddXY(0, element.Right.Ny);
                //        Chart.Series.Add(Support);
                //    }
                //}

                // 更新坐标轴范围
                Chart.ChartAreas[0].AxisX.Maximum = (FxMax.Max() + (FxMax.Max() - FxMin.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisX.Minimum = (FxMin.Min() - (FxMax.Max() - FxMin.Min()) * 0.1) * 1e-3;
                Chart.ChartAreas[0].AxisY.Maximum = Ny.Max();
                Chart.ChartAreas[0].AxisY.Minimum = Ny.Min();

                Chart.ChartAreas[0].Axes[0].Title = "轴力(kN/m)";

            }));
        }

        // 【按钮】输出窗口文本操作
        private void btnClearOutputWindow_Click(object sender, EventArgs e)         // 【按钮】清空输出窗口
        {
            DialogResult AF = MessageBox.Show("确认清空？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (AF == DialogResult.OK)
                rtbOutputWindow.Clear();
        }
        private void btnWriteLogToFile_Click(object sender, EventArgs e)            // 【按钮】将输出窗口文本写至文件
        {
            // 打开文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = System.Windows.Forms.Application.StartupPath,
                FileName = DateTime.Now.Year.ToString("0") + "." + DateTime.Now.Month.ToString("0") + "."
                + DateTime.Now.Day.ToString("0") + "-程序日志",
                Filter = "文本文件|*.log"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);      // false 指若已存在同名文件则进行覆盖
                sw.WriteLine("基坑支护计算程序日志");                                 // 写入表头
                sw.WriteLine("导出时间：" + System.DateTime.Now.ToString() + "\r\n\r\n");
                for (int i = 0; i < rtbOutputWindow.Lines.Count(); i++)
                    sw.WriteLine(rtbOutputWindow.Lines[i]);
                sw.Close();
                PrintString("日志导出成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }

        // 【按钮】输入模型信息
        private void btnInputLoadcasesInfo_Click(object sender, EventArgs e)        // 【按钮】弹出工况信息输入子窗口
        {
            Form_LoadcasesInfo fm = new Form_LoadcasesInfo(this) { Owner = this };
            fm.ShowDialog();
        }
        private void btnInputSupportsInfo_Click(object sender, EventArgs e)         // 【按钮】弹出支撑信息输入子窗口
        {
            Form_SupportsInfo fm = new Form_SupportsInfo(this) { Owner = this };
            fm.ShowDialog();
        }
        private void btnInputSoilLayersInfo_Click(object sender, EventArgs e)       // 【按钮】弹出土层信息输入子窗口
        {
            Form_SoilLayersInfo fm = new Form_SoilLayersInfo(this) { Owner = this };
            fm.ShowDialog();
        }

        private void btnInputLocalLoadsInfo_Click(object sender, EventArgs e)       // 【按钮】弹出局部荷载信息输入子窗口
        {
            Form_LocalLoadsInfo fm = new Form_LocalLoadsInfo(this) { Owner = this };
            fm.ShowDialog();
        }

        private void btnInputOtherInfo_Click(object sender, EventArgs e)            // 【按钮】弹出其他信息输入子窗口
        {
            Form_OtherInfo fm = new Form_OtherInfo(this) { Owner = this };
            fm.ShowDialog();
        }

        // 【按钮】输出模型信息和计算结果
        private void btnOutputAll_Click(object sender, EventArgs e)                 // 【按钮】输出全部信息至文本文件
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = System.Windows.Forms.Application.StartupPath,
                FileName = DateTime.Now.Year.ToString("0") + "." + DateTime.Now.Month.ToString("0") + "."
                + DateTime.Now.Day.ToString("0") + "-计算结果",
                Filter = "文本文件|*.dat"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);      // false 指若已存在同名文件则进行覆盖

                #region 输出文件头
                sw.WriteLine("==============================================");
                sw.WriteLine("        基坑支护计算程序计算结果");
                sw.WriteLine("        导出时间：" + System.DateTime.Now.ToString());
                sw.WriteLine("==============================================\r\n\r\n");
                #endregion

                #region 输出节点信息
                sw.WriteLine("**********************************************");
                sw.WriteLine("节点信息：（节点编号，x坐标，y坐标）\r\n");
                sw.WriteLine("NodeNo     Nx(m)     Ny(m)");
                sw.WriteLine("------     -----     -----"); 
                foreach (Node node in Nodes)
                    sw.WriteLine("{0,6}{1,10:f2}{2,10:f2}", node.No, node.Nx, node.Ny);
                sw.WriteLine("\r\n");
                #endregion

                #region 输出单元信息
                sw.WriteLine("**********************************************");
                sw.WriteLine("单元信息：（单元编号，左节点编号，右节点编号，单元类型，材料，截面面积，抗弯惯矩）\r\n");
                sw.WriteLine("ElemNo  Left Right    Type    Material    Area(m)     Iz(m^4)");
                sw.WriteLine("------  ---- -----    ----    --------   ---------   ---------");
                foreach (Element elem in Elements)
                    sw.WriteLine("{0,6}{1,6}{2,6}{3,8}{4,12}{5,12:e2}{6,12:e2}", elem.No, elem.Left.No, elem.Right.No, elem.ElementType, elem.Material.Name, elem.RealConstant.Area, elem.RealConstant.Iz);
                sw.WriteLine("\r\n");
                #endregion

                #region 输出荷载信息
                sw.WriteLine("**********************************************");
                sw.WriteLine("主动区土压力等效节点荷载：（节点号、水平力、竖向力、弯矩）\r\n");
                sw.WriteLine("NodeNo     Fx(N)       Fy(N)       M(N*m)");
                sw.WriteLine("------   ---------   ---------   ---------");
                for (int i = 0; i < fmCal.Fs.Count() / 3; i++)
                    if (Nodes[i].Nx == 0)
                        sw.WriteLine("{0,6}{1,12:e3}{2,12:e3}{3,12:e3}", i + 1, fmCal.Fs[i * 3], fmCal.Fs[i * 3 + 1], fmCal.Fs[i * 3 + 2]);
                sw.WriteLine("\r\n");
                #endregion

                #region 输出各工况结点位移
                sw.WriteLine("----------------------------------------------");
                sw.WriteLine("各工况结点位移：（工况序号、节点号、水平位移、竖向位移、转角）\r\n");
                sw.WriteLine("LC  NodeNo      Ux(m)         Uy(m)      Theta(rad)");
                sw.WriteLine("--  ------  ------------   -----------  ------------");
                for (int LC = 0; LC < DispSum.ColumnCount; LC++)
                    for (int iNode = 0; iNode < Nodes.Count; iNode++)
                        sw.WriteLine("{0,2}{1,8}{2,14:e4}{3,14:e4}{4,14:e4}", LC+1, iNode + 1, DispSum[3 * iNode, LC], DispSum[3 * iNode + 1, LC], DispSum[3 * iNode + 2, LC]);
                sw.WriteLine("\r\n");
                #endregion

                #region 输出各工况单元内力
                sw.WriteLine("----------------------------------------------");
                sw.WriteLine("各工况单元内力：（工况序号、单元号、6项杆端力）\r\n");
                sw.WriteLine("LC  ElemNo      iFx(N)        iFy(N)      iMom(N*m)       jFx(N)        jFy(N)      jMom(N*m)");
                sw.WriteLine("--  ------   -----------   -----------   -----------   -----------   -----------   -----------");
                for (int Stage = 0; Stage < DispSum.ColumnCount; Stage++)
                {
                    for (int i = 0; i < Nodes.Count(); i++)
                        Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);
                    for (int i = 0; i < Elements.Count(); i++)
                    {
                        Elements[i].RealConstant.IniStrn = InistrnSum[i, Stage];
                        Elements[i].isAlive = AliveSum[i, Stage] == 1;
                        Elements[i].getNodalForce();
                        sw.WriteLine("{0,2}{1,8}{2,14:e4}{3,14:e4}{4,14:e4}{5,14:e4}{6,14:e4}{7,14:e4}", Stage + 1, i + 1, Elements[i].iFx, Elements[i].iFy, Elements[i].iMom, Elements[i].jFx, Elements[i].jFy, Elements[i].jMom);
                    }
                }
                sw.WriteLine("\r\n");
                #endregion

                sw.Close();
                PrintString("数据文件导出成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }
        private void btnOutputNodesInfo_Click(object sender, EventArgs e)           // 【按钮】向文本框输出节点信息
        {
            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出节点信息：\r\n节点编号\tx坐标\ty坐标\r\n";
            for (int i = 0; i < Nodes.Count; i++)
                rtbOutputWindow.Text += Nodes[i].No.ToString("0") + "\t" + Nodes[i].Nx.ToString("0.00") + "\t" + Nodes[i].Ny.ToString("0.00") + "\r\n";
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();
        }
        private void btnOutputElementsInfo_Click(object sender, EventArgs e)        // 【按钮】向文本框输出单元信息
        {
            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出单元信息：\r\n单元编号\ti节点编号\tj节点编号\r\n";
            for (int i = 0; i < Elements.Count; i++)
                rtbOutputWindow.Text += i + 1.ToString("0") + "\t" + Elements[i].Left.No.ToString("0") + "\t" + Elements[i].Right.No.ToString("0") + "\r\n";
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();
        }
        private void btnOutputLoadsInfo_Click(object sender, EventArgs e)           // 【按钮】向文本框输出主动土压力荷载信息
        {
            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出围护结构等效节点荷载信息：\r\n节点编号\ty坐标\t荷载方向\t荷载值\t单位\r\n";
            foreach (int i in fmCal.CM_Node_ECS)
            {
                rtbOutputWindow.Text += Nodes[i].No.ToString("0") + "\t" + Nodes[i].Ny.ToString("0.00") + "\tFx\t" + (fmCal.Fs[i * 3] * 1e-3).ToString("0.00") + "\tkN/m\r\n";
                rtbOutputWindow.Text += Nodes[i].No.ToString("0") + "\t" + Nodes[i].Ny.ToString("0.00") + "\tM\t" + (fmCal.Fs[i * 3 + 2] * 1e-3).ToString("0.00") + "\tkN·m/m\r\n";
            }
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();
        }
        private void btnOutputM_Click(object sender, EventArgs e)                   // 【按钮】向文本框输出当前工况下围护结构弯矩
        {
            if (cbStage.Text == "包络" || cbStage.Text == "")
            {
                PrintString("请在右侧指定工况！");
                return;
            }

            int Stage = int.Parse(cbStage.Text) - 1;
            for (int i = 0; i < Nodes.Count(); i++)         // 还原节点位移
                Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);

            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出第 " + cbStage.Text + " 个施工阶段对应围护结构弯矩：\r\n单元编号\ti节点标高\ti节点弯矩(kN·m/m)\r\n";
            foreach (int i in fmCal.CM_Elem_ECS)
            {
                Elements[i].getNodalForce();
                rtbOutputWindow.Text += Elements[i].No.ToString("0") + "\t" + Elements[i].Left.Ny.ToString("0.00") + "\t" + (Elements[i].iMom * 1e-3).ToString("0.00") + "\r\n";
            }
            rtbOutputWindow.Text += "\r\n";
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();
        }
        private void btnOutputFy_Click(object sender, EventArgs e)                  // 【按钮】向文本框输出当前工况下围护结构剪力
        {
            if (cbStage.Text == "包络" || cbStage.Text == "")
            {
                PrintString("请在右侧指定工况！");
                return;
            }

            int Stage = int.Parse(cbStage.Text) - 1;
            for (int i = 0; i < Nodes.Count(); i++)         // 还原节点位移
                Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);

            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出第 " + cbStage.Text + " 个施工阶段对应围护结构剪力：\r\n单元编号\ti节点标高\ti节点剪力(kN/m)\r\n";
            foreach (int i in fmCal.CM_Elem_ECS)
            {
                Elements[i].getNodalForce();
                rtbOutputWindow.Text += Elements[i].No.ToString("0") + "\t" + Elements[i].Left.Ny.ToString("0.00") + "\t" + (Elements[i].iFy * 1e-3).ToString("0.00") + "\r\n";
            }
            rtbOutputWindow.Text += "\r\n";
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();
        }
        private void btnOutputUx_Click(object sender, EventArgs e)                  // 【按钮】向文本框输出当前工况下围护结构水平位移
        {
            if (cbStage.Text == "包络" || cbStage.Text == "")
            {
                PrintString("请在右侧指定工况！");
                return;
            }

            int Stage = int.Parse(cbStage.Text) - 1;
            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出第 " + cbStage.Text + " 个施工阶段对应围护结构水平位移：\r\n节点编号\t节点标高\t水平位移(mm)\r\n";
            foreach (int i in fmCal.CM_Node_ECS)
                rtbOutputWindow.Text += Nodes[i].No.ToString("0") + "\t" + Nodes[i].Ny.ToString("0.00") + "\t" + (DispSum[i * 3, Stage] * 1e3).ToString("0.000") + "\r\n";
            rtbOutputWindow.Text += "\r\n";
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();
        }
        private void btnOutputFx_Click(object sender, EventArgs e)                  // 【按钮】向文本框输出当前工况下支撑轴力
        {
            if (cbStage.Text == "包络" || cbStage.Text == "")
            {
                PrintString("请在右侧指定工况！");
                return;
            }

            int Stage = int.Parse(cbStage.Text) - 1;
            for (int i = 0; i < Nodes.Count(); i++)         // 还原节点位移
                Nodes[i].LoadDisp(DispSum[i * 3, Stage], DispSum[i * 3 + 1, Stage], DispSum[i * 3 + 2, Stage]);
            for (int i = 0; i < Elements.Count(); i++)      // 还原支撑单元初应变
                Elements[i].RealConstant.IniStrn = InistrnSum[i, Stage];
            
            // 还原单元生死状态
            int ActSupCount = 0;
            for (int i = 0; i < Stage + 1; i++)
            {
                if (Loadcases[i].IsActiveSupport == true)
                {
                    Elements[fmCal.CM_Elem_Supports[ActSupCount]].isAlive = true;
                    ActSupCount++;
                }
            }
            for (int i = Stage + 1; i < Loadcases.Count(); i++)
            {
                if (Loadcases[i].IsActiveSupport == true)
                {
                    Elements[fmCal.CM_Elem_Supports[ActSupCount]].isAlive = false;
                    ActSupCount++;
                }
            }

            rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出第 " + cbStage.Text + " 个施工阶段对应支撑轴力：\r\n单元编号\ti支撑标高\t轴力(kN/m)\r\n";
            foreach (int i in fmCal.CM_Elem_Supports)
            {
                Elements[i].getNodalForce();
                rtbOutputWindow.Text += Elements[i].No.ToString("0") + "\t" + Elements[i].Left.Ny.ToString("0.00") + "\t" + (Elements[i].iFx * 1e-3).ToString("0.00") + "\r\n";
            }
            rtbOutputWindow.Text += "\r\n";
            rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            rtbOutputWindow.ScrollToCaret();

        }

    }
}
