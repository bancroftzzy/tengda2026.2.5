using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ActiveControl
{
    public partial class Form_Main
    {
        private StageResults SelectedStage(bool allowEnvelope)
        {
            if (selectedResults == null || selectedResults.Stages.Count == 0)
            {
                MessageBox.Show(this, "当前没有可查看的结果，请先计算或从工程文件中打开结果。", "没有数据");
                return null;
            }
            if (allowEnvelope && cbStage.Text == "包络") return selectedResults.Stages.Last();
            int number;
            var stage = int.TryParse(cbStage.Text, out number)
                ? selectedResults.Stages.SingleOrDefault(s => s.Number == number) : null;
            if (stage == null) MessageBox.Show(this, "所选施工阶段没有计算数据，请选择已完成的施工阶段。", "没有数据");
            return stage;
        }

        private void DrawSavedResults()
        {
            var stage = SelectedStage(true);
            if (stage == null) return;
            bool envelope = cbStage.Text == "包络";
            string figure = cbFigType.Text;
            double[] ys, maximum, minimum = null;
            var env = selectedResults.Envelope;
            if (envelope)
            {
                switch (figure)
                {
                    case "位移": ys = env.WallNodeY; maximum = env.UxMax; minimum = env.UxMin; break;
                    case "弯矩": ys = env.WallElementY; maximum = env.MomMax; minimum = env.MomMin; break;
                    case "剪力": ys = env.WallElementY; maximum = env.FyMax; minimum = env.FyMin; break;
                    case "轴力": ys = env.SupportY; maximum = env.FxMax; minimum = env.FxMin; break;
                    default: return;
                }
            }
            else if (figure == "位移")
            {
                ys = stage.WallNodeIndices.Select(i => stage.Nodes[i].Y).ToArray();
                maximum = stage.WallNodeIndices.Select(i => stage.Nodes[i].Ux).ToArray();
            }
            else if (figure == "轴力")
            {
                var supports = stage.Supports.Where(s => stage.Elements[s.ElementNo - 1].Alive).ToArray();
                ys = supports.Select(s => stage.Nodes[stage.Elements[s.ElementNo - 1].LeftNo - 1].Y).ToArray();
                maximum = supports.Select(s => stage.Elements[s.ElementNo - 1].Forces[0]).ToArray();
            }
            else
            {
                int component = figure == "弯矩" ? 2 : 1;
                ys = stage.WallElementIndices.Select(i => stage.Nodes[stage.Elements[i].LeftNo - 1].Y).ToArray();
                maximum = stage.WallElementIndices.Select(i => stage.Elements[i].Forces[component]).ToArray();
            }

            Chart.Series.Clear(); Chart.Titles.Clear();
            Chart.Titles.Add(CalculationMethodNames.Get(selectedResults.Method) + " · " +
                (envelope ? "已计算阶段包络" : "第" + stage.Number + "阶段 · " + stage.Status));
            double scale = figure == "位移" ? 1e3 : 1e-3;
            var plotted = maximum.Select(x => x * scale).ToList();
            AddSavedSeries(envelope ? "最大值" : figure, maximum, ys, scale,
                envelope ? Color.BlueViolet : figure == "位移" ? Color.LimeGreen : Color.Red);
            if (minimum != null)
            {
                AddSavedSeries("最小值", minimum, ys, scale, Color.Red);
                plotted.AddRange(minimum.Select(x => x * scale));
            }
            plotted.Add(0);
            double xmin = plotted.Min(), xmax = plotted.Max();
            double padding = Math.Max((xmax - xmin) * 0.1, figure == "位移" ? 0.1 : 1);
            var wallY = stage.WallNodeIndices.Select(i => stage.Nodes[i].Y).ToArray();
            double ymin = wallY.Length == 0 ? stage.Nodes.Min(n => n.Y) : wallY.Min();
            double ymax = wallY.Length == 0 ? stage.Nodes.Max(n => n.Y) : wallY.Max();
            if (ymax <= ymin) ymax = ymin + 1;
            var area = Chart.ChartAreas[0];
            area.AxisX.Minimum = xmin - padding; area.AxisX.Maximum = xmax + padding;
            if (!envelope && figure == "位移")
            {
                // 沿用原位移图的左右对称范围，取该次计算自己的输入，避免旧结果受当前参数影响。
                var parameters = new InputParameters();
                selectedResults.Inputs.Parameters.Apply(parameters, ProjectInputs.ParameterNames);
                double referenceRange = Math.Abs(parameters.LengthOfECS * parameters.EpsDefor * 1e3);
                double range = Math.Max(referenceRange, Math.Max(Math.Abs(xmin), Math.Abs(xmax)) * 1.1);
                range = Math.Max(range, 0.1);
                area.AxisX.Minimum = -range; area.AxisX.Maximum = range;
            }
            area.AxisY.Minimum = ymin; area.AxisY.Maximum = ymax;
            area.AxisX.Title = figure == "位移" ? "位移(mm)" : figure == "弯矩" ? "弯矩(kN·m/m)" : figure + "(kN/m)";
            area.AxisY.Title = "标高(m)";
            area.AxisX.Interval = area.AxisY.Interval = 0;
            var zero = new Series("零线") { ChartType = SeriesChartType.Line, Color = Color.Gray, BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash, IsVisibleInLegend = false };
            zero.Points.AddXY(0, ymin); zero.Points.AddXY(0, ymax); Chart.Series.Add(zero);
            foreach (var support in stage.Supports)
            {
                var element = stage.Elements[support.ElementNo - 1];
                if (!element.Alive) continue;
                double y = stage.Nodes[element.RightNo - 1].Y;
                var marker = new Series("支撑" + (support.Index + 1)) { ChartType = SeriesChartType.Line,
                    Color = envelope ? Color.Gray : Color.Black, BorderWidth = 2,
                    BorderDashStyle = envelope ? ChartDashStyle.Dash : ChartDashStyle.Solid, IsVisibleInLegend = false };
                marker.Points.AddXY(area.AxisX.Minimum, y);
                marker.Points.AddXY(!envelope && figure == "位移" ? support.ContactUx * 1e3 : 0, y);
                Chart.Series.Add(marker);
            }
            if (Chart.Legends.Count > 0) Chart.Legends[0].Enabled = envelope;
        }

        private void AddSavedSeries(string name, double[] values, double[] y, double scale, Color color)
        {
            var line = new Series(name) { ChartType = SeriesChartType.Line, BorderWidth = 2, Color = color };
            for (int i = 0; i < values.Length; i++) line.Points.AddXY(values[i] * scale, y[i]);
            if (values.Length > 0 && name != "位移")
            {
                int index = Array.IndexOf(values, values.OrderByDescending(Math.Abs).First());
                line.Points[index].Label = (values[index] * scale).ToString("F2");
                line.Points[index].MarkerStyle = MarkerStyle.Square;
                line.Points[index].MarkerSize = 6;
            }
            Chart.Series.Add(line);
        }

        private string ResultExportHeader()
        {
            bool stale = selectedResults.Inputs.Signature() != ProjectInputs.Capture(this).Signature();
            return "计算方法：" + CalculationMethodNames.Get(selectedResults.Method) + "\r\n" +
                "状态：" + selectedResults.DisplayStatus() + "\r\n" +
                "计算时间：" + selectedResults.StartedAt + "\r\n" +
                (stale ? "注意：该结果对应保存的旧输入，当前工程输入已修改。\r\n" : "") + "\r\n";
        }

        private static string F(double number) => number.ToString("G17", CultureInfo.InvariantCulture);

        private static void AppendStageExport(StringBuilder text, StageResults stage, string kind)
        {
            text.AppendLine("施工阶段 " + stage.Number + "：" + stage.Status);
            if (kind == "全部" || kind == "节点")
            {
                text.AppendLine("节点编号\tx(m)\ty(m)\tUx(m)\tUy(m)\t转角(rad)");
                foreach (var n in stage.Nodes) text.AppendLine(string.Join("\t", n.No, F(n.X), F(n.Y), F(n.Ux), F(n.Uy), F(n.Rot)));
            }
            if (kind == "全部" || kind == "单元")
            {
                text.AppendLine("单元编号\t左节点\t右节点\t类型\t材料\tE(Pa)\t面积(m²)\tIz(m⁴)\t初应变\t激活\tiFx(N/m)\tiFy(N/m)\tiMom(N·m/m)\tjFx(N/m)\tjFy(N/m)\tjMom(N·m/m)");
                foreach (var e in stage.Elements)
                    text.AppendLine(string.Join("\t", e.No, e.LeftNo, e.RightNo, e.Type, e.Material, F(e.E), F(e.Area), F(e.Iz), F(e.InitialStrain), e.Alive) + "\t" + string.Join("\t", e.Forces.Select(F)));
            }
            if (kind == "全部" || kind == "荷载")
            {
                text.AppendLine("节点编号\tFx(N/m)\tFy(N/m)\tM(N·m/m)");
                for (int i = 0; i < stage.Nodes.Count; i++)
                    text.AppendLine(string.Join("\t", stage.Nodes[i].No, F(stage.AppliedLoads[i * 3]), F(stage.AppliedLoads[i * 3 + 1]), F(stage.AppliedLoads[i * 3 + 2])));
            }
            if (kind == "全部" || kind == "位移")
            {
                text.AppendLine("围护节点编号\t标高(m)\t水平位移(mm)");
                foreach (int i in stage.WallNodeIndices)
                    text.AppendLine(string.Join("\t", stage.Nodes[i].No, F(stage.Nodes[i].Y), F(stage.Nodes[i].Ux * 1e3)));
            }
            if (kind == "全部" || kind == "弯矩" || kind == "剪力")
            {
                text.AppendLine("围护单元编号\t左节点标高(m)\t弯矩(kN·m/m)\t剪力(kN/m)");
                foreach (int i in stage.WallElementIndices)
                {
                    var e = stage.Elements[i];
                    text.AppendLine(string.Join("\t", e.No, F(stage.Nodes[e.LeftNo - 1].Y), F(e.Forces[2] * 1e-3), F(e.Forces[1] * 1e-3)));
                }
            }
            if (kind == "全部" || kind == "轴力")
            {
                text.AppendLine("支撑编号\t单元编号\t激活\t实际轴力(kN/m，受压为正)\t累计行程(mm)\t接触点位移(mm)\t支撑弹性压缩量(mm)");
                foreach (var s in stage.Supports)
                {
                    var e = stage.Elements[s.ElementNo - 1];
                    text.AppendLine(string.Join("\t", s.Index + 1, s.ElementNo, e.Alive, F(-e.Forces[3] * 1e-3),
                        F(s.Stroke * 1e3), F(s.ContactUx * 1e3), F(s.Compression * 1e3)));
                }
            }
            if (!string.IsNullOrEmpty(stage.ManualInput)) text.AppendLine("手动输入目标轴力(kN/m)：" + stage.ManualInput);
            text.AppendLine();
        }

        private void ExportSavedResults(string kind)
        {
            if (!AllowProjectOperation()) return;
            var stage = SelectedStage(kind == "全部" || kind == "节点" || kind == "单元");
            if (stage == null) return;
            var text = new StringBuilder(ResultExportHeader());
            if (kind == "全部")
            {
                foreach (var saved in selectedResults.Stages) AppendStageExport(text, saved, "全部");
                AppendEnvelopeExport(text, selectedResults.Envelope);
                text.AppendLine("计算日志及千斤顶调节过程："); text.Append(selectedResults.Log);
                using (var dialog = new SaveFileDialog
                {
                    Filter = "计算结果文件 (*.dat)|*.dat", DefaultExt = "dat", AddExtension = true,
                    FileName = (projectPath == null ? "未命名工程" : Path.GetFileNameWithoutExtension(projectPath)) + "-" + CalculationMethodNames.Get(selectedResults.Method) + "-计算结果.dat",
                    RestoreDirectory = true
                })
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK) return;
                    try { File.WriteAllText(dialog.FileName, text.ToString(), Encoding.UTF8); PrintString("结果已导出：" + dialog.FileName); }
                    catch (Exception ex) { MessageBox.Show(this, "导出失败：" + ex.Message); }
                }
            }
            else
            {
                AppendStageExport(text, stage, kind);
                rtbOutputWindow.AppendText("\r\n" + text);
                rtbOutputWindow.SelectionStart = rtbOutputWindow.TextLength;
                rtbOutputWindow.ScrollToCaret();
            }
        }

        private static void AppendEnvelopeExport(StringBuilder text, ResultEnvelope envelope)
        {
            text.AppendLine("已计算阶段包络（包含计算中记录的逐根调节状态）：");
            text.AppendLine("围护节点标高(m)\t最大位移(mm)\t最小位移(mm)");
            for (int i = 0; i < envelope.WallNodeY.Length; i++)
                text.AppendLine(string.Join("\t", F(envelope.WallNodeY[i]), F(envelope.UxMax[i] * 1e3), F(envelope.UxMin[i] * 1e3)));
            text.AppendLine("围护单元标高(m)\t最大弯矩(kN·m/m)\t最小弯矩(kN·m/m)\t最大剪力(kN/m)\t最小剪力(kN/m)");
            for (int i = 0; i < envelope.WallElementY.Length; i++)
                text.AppendLine(string.Join("\t", F(envelope.WallElementY[i]), F(envelope.MomMax[i] * 1e-3), F(envelope.MomMin[i] * 1e-3), F(envelope.FyMax[i] * 1e-3), F(envelope.FyMin[i] * 1e-3)));
            text.AppendLine("支撑编号\t标高(m)\t最大轴力(kN/m)\t最小轴力(kN/m)");
            for (int i = 0; i < envelope.SupportY.Length; i++)
                text.AppendLine(string.Join("\t", i + 1, F(envelope.SupportY[i]), F(envelope.FxMax[i] * 1e-3), F(envelope.FxMin[i] * 1e-3)));
            text.AppendLine();
        }
    }
}
