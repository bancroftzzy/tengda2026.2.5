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
            InitializeProjectMenus();
        }
        private void MainForm_Load(object sender, EventArgs e)       // 主程序启动
        {
            LoadLastProject();
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
            if (fmCal != null && !fmCal.IsDisposed)
            {
                fmCal.Show();
                fmCal.Activate();
                return;
            }
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
            if (AllowProjectOperation()) DrawSavedResults();
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
            if (!AllowProjectOperation()) return;
            Form_LoadcasesInfo fm = new Form_LoadcasesInfo(this) { Owner = this };
            fm.ShowDialog();
            InputsEdited();
        }
        private void btnInputSupportsInfo_Click(object sender, EventArgs e)         // 【按钮】弹出支撑信息输入子窗口
        {
            if (!AllowProjectOperation()) return;
            Form_SupportsInfo fm = new Form_SupportsInfo(this) { Owner = this };
            fm.ShowDialog();
            InputsEdited();
        }
        private void btnInputSoilLayersInfo_Click(object sender, EventArgs e)       // 【按钮】弹出土层信息输入子窗口
        {
            if (!AllowProjectOperation()) return;
            Form_SoilLayersInfo fm = new Form_SoilLayersInfo(this) { Owner = this };
            fm.ShowDialog();
            InputsEdited();
        }

        private void btnInputLocalLoadsInfo_Click(object sender, EventArgs e)       // 【按钮】弹出局部荷载信息输入子窗口
        {
            if (!AllowProjectOperation()) return;
            Form_LocalLoadsInfo fm = new Form_LocalLoadsInfo(this) { Owner = this };
            fm.ShowDialog();
            InputsEdited();
        }

        private void btnInputOtherInfo_Click(object sender, EventArgs e)            // 【按钮】弹出其他信息输入子窗口
        {
            if (!AllowProjectOperation()) return;
            Form_OtherInfo fm = new Form_OtherInfo(this) { Owner = this };
            fm.ShowDialog();
            InputsEdited();
        }

        // 【按钮】输出模型信息和计算结果
        private void btnOutputAll_Click(object sender, EventArgs e)                 // 【按钮】输出全部信息至文本文件
        {
            ExportSavedResults("全部");
        }
        private void btnOutputNodesInfo_Click(object sender, EventArgs e)           // 【按钮】向文本框输出节点信息
        {
            ExportSavedResults("节点");
        }
        private void btnOutputElementsInfo_Click(object sender, EventArgs e)        // 【按钮】向文本框输出单元信息
        {
            ExportSavedResults("单元");
        }
        private void btnOutputLoadsInfo_Click(object sender, EventArgs e)           // 【按钮】向文本框输出主动土压力荷载信息
        {
            ExportSavedResults("荷载");
        }
        private void btnOutputM_Click(object sender, EventArgs e)                   // 【按钮】向文本框输出当前工况下围护结构弯矩
        {
            ExportSavedResults("弯矩");
        }
        private void btnOutputFy_Click(object sender, EventArgs e)                  // 【按钮】向文本框输出当前工况下围护结构剪力
        {
            ExportSavedResults("剪力");
        }
        private void btnOutputUx_Click(object sender, EventArgs e)                  // 【按钮】向文本框输出当前工况下围护结构水平位移
        {
            ExportSavedResults("位移");
        }
        private void btnOutputFx_Click(object sender, EventArgs e)                  // 【按钮】向文本框输出当前工况下支撑轴力
        {
            ExportSavedResults("轴力");
        }
    }
}
