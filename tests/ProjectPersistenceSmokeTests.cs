using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ActiveControl;
using ActiveControl.Forms;

// 独立STA集成测试：不操作用户正在运行的程序，不写最近工程配置，也不覆盖输入TXT。
internal static class ProjectPersistenceSmokeTests
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    private static TextWriter report;
    private static object Invoke(object target, string name, params object[] args)
        => target.GetType().GetMethod(name, Flags).Invoke(target, args);
    private static T Field<T>(object target, string name)
        => (T)target.GetType().GetField(name, Flags | BindingFlags.Public).GetValue(target);
    private static void Assert(bool value, string message)
    {
        if (!value) throw new Exception(message);
        report.WriteLine("PASS " + message); report.Flush();
    }

    [STAThread]
    private static int Main(string[] args)
    {
        string directory = args[0];
        Directory.CreateDirectory(directory);
        using (report = new StreamWriter(Path.Combine(directory, "verification.log"), false, Encoding.UTF8))
        {
            // 求解器诊断信息较多，另存，测试报告保持可读。
            using (var diagnostics = new StreamWriter(Path.Combine(directory, "solver.log"), false, Encoding.UTF8))
            {
                Console.SetOut(diagnostics);
                try { Run(directory); report.WriteLine("ALL TESTS PASSED"); return 0; }
                catch (Exception ex) { report.WriteLine("FAIL " + ex); return 1; }
            }
        }
    }

    private static Form_Main CreateForm()
    {
        var form = new Form_Main();
        var handle = form.Handle; // 建立Invoke消息队列，不Show，避免触发真实用户的最近工程恢复。
        Invoke(form, "ResetProjectData");
        return form;
    }

    private static void FillFixture(Form_Main form)
    {
        form.LengthOfECS = 6; form.ThickOfECS = 0.8; form.LengthOfSupports = 10;
        form.ElevOfCollar = form.ElevOfGround = 0;
        form.GroundLoad = 20; form.EpsDefor = 8e-4;
        form.MaxMommentOfECS1 = form.MaxMommentOfECS2 = form.MaxShearForceOfECS = 1e9;
        form.SoilLayers.Add(new SoilLayer(20, 5, 25, 0.5, 8, 5, 18, "粉质黏土"));
        form.Supports.Add(new Support("钢", 1, 3, 0.609, 0.016, 3000, 0, true, 200));
        form.Supports.Add(new Support("钢", 2, 3, 0.609, 0.016, 3000, 0, true, 200));
        form.Loadcases.Add(new Loadcase(1.5, false));
        form.Loadcases.Add(new Loadcase(0, true));
        form.Loadcases.Add(new Loadcase(1, false));
        form.Loadcases.Add(new Loadcase(0, true));
        Loadcase.EnableWater = false; Loadcase.UseNonlinearSoilSpring = false;
    }

    private static void Calculate(Form_Main form, Form_Calculate control, CalculationMethod method, bool all, string manual = "")
    {
        string radio = method == CalculationMethod.Direct ? "rbtDirect" : method == CalculationMethod.GlobalPSO ? "rbtGlobalPSO"
            : method == CalculationMethod.PartitionPSO ? "rbtPartitionPSO" : method == CalculationMethod.ZeroDisp ? "rbtZeroDisp" : "rbtManual";
        Field<RadioButton>(control, radio).Checked = true;
        Field<TextBox>(control, "tbForces").Text = manual;
        Invoke(control, "StartCalculation", all);
        var watch = Stopwatch.StartNew();
        while (form.IsCalculationBusy)
        {
            Application.DoEvents();
            Thread.Sleep(10);
            if (watch.Elapsed.TotalSeconds > 240) throw new TimeoutException("计算超时：" + method);
        }
        Application.DoEvents();
        var run = Field<MethodResults>(control, "activeRun");
        if (run == null || run.Status == "计算异常") report.WriteLine(run?.Log ?? form.rtbOutputWindow.Text);
        Assert(run != null && run.Stages.Count > 0 && run.Status != "计算异常", method + " calculation and stage capture: " + run?.Status);
    }

    private static void Run(string directory)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var baseline = DirectBaseline();
        string saved = Path.Combine(directory, "all-methods.acproj");
        using (var form = CreateForm())
        {
            var menu = Field<MenuStrip>(form, "msInput");
            Assert(string.Join("|", menu.Items.Cast<ToolStripItem>().Select(x => x.Text)) == "工程菜单|数据录入|数据导出|查看结果", "menu order");
            FillFixture(form);
            string originalInput = ProjectInputs.Capture(form).Signature();
            form.SaveProjectFile(Path.Combine(directory, "inputs-only.acproj"));
            using (var emptyResults = CreateForm())
            {
                emptyResults.LoadProjectFile(Path.Combine(directory, "inputs-only.acproj"));
                Assert(ProjectInputs.Capture(emptyResults).Signature() == originalInput, "all input fields round trip without precision/unit loss");
                Assert(!emptyResults.SelectMethodResults(CalculationMethod.GlobalPSO, false), "uncalculated method returns no data");
                Assert(emptyResults.LocalLoads.Count == 0, "empty local loads remain empty");
                Assert(emptyResults.Supports[0].JackStrokeMax == 0.2, "200mm jack limit stays 0.2m internally");
            }

            using (var control = new Form_Calculate(form))
            {
                form.fmCal = control;
                var calculationHandle = control.Handle;
                Calculate(form, control, CalculationMethod.Direct, true);
                form.SaveProjectFile(Path.Combine(directory, "direct.acproj"));
                var direct = ProjectFile.Read(Path.Combine(directory, "direct.acproj")).Results.Single();
                Assert(direct.Stages.Count == 4 && direct.Status == "已完成", "direct has all four completed stages");
                for (int i = 0; i < direct.Stages.Count; i++)
                    Assert(direct.Stages[i].Nodes.Select(n => n.Ux).Zip(baseline[i], (actual, expected) => Math.Abs(actual - expected)).Max() < 1e-12,
                        "direct stage " + (i + 1) + " matches original Construction/Solve workflow");
                Assert(direct.Stages.All(s => s.Supports.All(x => x.Stroke == 0)), "direct calculation introduces no jack stroke");
                double directUx = direct.Stages.Last().Nodes[0].Ux;
                // 开关并保留在输入快照中；随后还原为线性进行优化样例。
                Loadcase.EnableWater = true; Loadcase.WaterTableElev = -0.25; Loadcase.UseNonlinearSoilSpring = true;
                form.SoilLayers[0].K_Duncan = 543.1234567890123;
                form.LocalLoads.Add(new LocalLoad(1, 2, 12.3456789012345));
                form.SaveProjectFile(Path.Combine(directory, "changed-inputs.acproj"));
                var changed = ProjectFile.Read(Path.Combine(directory, "changed-inputs.acproj"));
                Assert(changed.Inputs.EnableWater && changed.Inputs.UseNonlinearSoilSpring && changed.Inputs.WaterTableElev == -0.25, "water and nonlinear switches retained");
                Assert(changed.Inputs.Signature() != changed.Results[0].Inputs.Signature(), "old result keeps its own input snapshot");
                Loadcase.EnableWater = false; Loadcase.WaterTableElev = -9999; Loadcase.UseNonlinearSoilSpring = false;
                form.LocalLoads.Clear();

                Calculate(form, control, CalculationMethod.GlobalPSO, true);
                Calculate(form, control, CalculationMethod.PartitionPSO, true);
                Calculate(form, control, CalculationMethod.ZeroDisp, true);
                Calculate(form, control, CalculationMethod.Manual, false);
                Calculate(form, control, CalculationMethod.Manual, false, "100");
                form.SaveProjectFile(saved);
                var document = ProjectFile.Read(saved);
                Assert(document.Results.Count == 5, "five method results coexist");
                Assert(document.Results.Single(r => r.Method == CalculationMethod.Direct).Stages.Last().Nodes[0].Ux == directUx,
                    "later methods do not overwrite direct result");
                Assert(document.Results.Single(r => r.Method == CalculationMethod.Manual).Stages.Count == 2,
                    "manual partial result records only completed stages");
                Assert(document.Results.Single(r => r.Method == CalculationMethod.Manual).Stages.Last().ManualInput == "100",
                    "manual target input is preserved");
                foreach (var result in document.Results)
                    Assert(!string.IsNullOrEmpty(result.Log) && result.Envelope != null, result.Method + " has independent log and envelope");
            }
        }

        using (var reopened = CreateForm())
        {
            reopened.LoadProjectFile(saved);
            Assert(reopened.Calculate == null && reopened.fmCal == null, "open project does not initialize model or start a calculation thread");
            foreach (CalculationMethod method in Enum.GetValues(typeof(CalculationMethod)))
            {
                Assert(reopened.SelectMethodResults(method, false), method + " can be selected after reopen");
                foreach (string kind in new[] { "位移", "弯矩", "剪力", "轴力" })
                {
                    reopened.cbFigType.SelectedItem = kind;
                    foreach (string stage in new[] { "1", "包络" }) reopened.cbStage.SelectedItem = stage;
                }
            }
            reopened.SelectMethodResults(CalculationMethod.Direct, false);
            reopened.cbFigType.SelectedItem = "位移"; reopened.cbStage.SelectedItem = "4";
            var chart = Field<System.Windows.Forms.DataVisualization.Charting.Chart>(reopened, "Chart");
            Assert(chart.Series["位移"].Color == Color.LimeGreen, "displacement curve retains old green color");
            Assert(chart.Series["支撑1"].Color == Color.Black, "support lines remain black");
            var current = Field<MethodResults>(reopened, "selectedResults");
            Assert(chart.Series["位移"].Points[0].XValue == current.Stages[3].Nodes[0].Ux * 1e3, "plot uses archived numerical displacement");
            Assert(chart.Series["支撑1"].Points[1].XValue == current.Stages[3].Supports[0].ContactUx * 1e3, "support joins displaced wall");
            chart.SaveImage(Path.Combine(directory, "restored-displacement.png"), System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
            // 离屏显示以创建全部子控件；移除Load处理器，避免读取/改写用户的最近工程配置。
            reopened.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), reopened,
                typeof(Form_Main).GetMethod("MainForm_Load", Flags));
            reopened.ShowInTaskbar = false;
            reopened.StartPosition = FormStartPosition.Manual;
            reopened.Location = new Point(-10000, -10000);
            reopened.Show();
            Application.DoEvents();
            Assert(!Field<Label>(reopened, "currentResultLabel").Bounds.IntersectsWith(Field<GroupBox>(reopened, "gbDraw").Bounds),
                "result label does not cover stage and figure controls");
            Assert(Field<Button>(reopened, "btnCalculate").Top >= Field<MenuStrip>(reopened, "msInput").Bottom,
                "calculation button is not covered by menu");
            using (var bitmap = new Bitmap(reopened.Width, reopened.Height))
            {
                reopened.DrawToBitmap(bitmap, new Rectangle(0, 0, reopened.Width, reopened.Height));
                bitmap.Save(Path.Combine(directory, "project-ui.png"));
            }
            reopened.Hide();
            Invoke(reopened, "ExportSavedResults", "轴力");
            Assert(reopened.rtbOutputWindow.Text.Contains("计算方法：直接计算") && reopened.rtbOutputWindow.Text.Contains("实际轴力"),
                "data export uses selected method");
            string before = ProjectInputs.Capture(reopened).Signature();
            var bad = ProjectFile.Read(saved); bad.Version = 999;
            string invalid = Path.Combine(directory, "invalid.acproj");
            File.WriteAllBytes(invalid, ProjectFile.Serialize(bad));
            bool rejected = false;
            try { reopened.LoadProjectFile(invalid); } catch { rejected = true; }
            Assert(rejected && ProjectInputs.Capture(reopened).Signature() == before, "invalid file leaves current inputs intact");
            reopened.SaveProjectFile(saved);
            Assert(ProjectFile.Read(saved).Results.Count == 5, "overwrite save retains all methods");
            Invoke(reopened, "ResetProjectData");
            Assert(reopened.Supports.Count == 0 && reopened.SoilLayers.Count == 0 && reopened.Loadcases.Count == 0 && reopened.LocalLoads.Count == 0,
                "new project clears all input lists");
            Assert(!reopened.SelectMethodResults(CalculationMethod.Direct, false) && File.Exists(saved), "new project clears results without deleting saved file");
        }
        VerifyFailedStage(directory);
    }

    private static List<double[]> DirectBaseline()
    {
        var result = new List<double[]>();
        using (var form = CreateForm())
        {
            FillFixture(form);
            using (var control = new Form_Calculate(form))
            {
                var handle = control.Handle;
                form.fmCal = control;
                Invoke(control, "Init");
                var solver = typeof(Form_Calculate).GetMethods(Flags).Single(m => m.Name == "SolveWithMode" && m.GetParameters().Length == 6);
                for (int i = 0; i < form.Loadcases.Count; i++)
                {
                    Invoke(control, "Construction");
                    object[] args = { form.Elements, form.Nodes, control.Fs, control.ConstrainedDOFIndex, null, null };
                    solver.Invoke(control, args);
                    control.Disp = (MathNet.Numerics.LinearAlgebra.Vector<double>)args[4];
                    control.RForce = (MathNet.Numerics.LinearAlgebra.Vector<double>)args[5];
                    Invoke(control, "UpdateEnvData");
                    Invoke(control, "CopyToSum");
                    result.Add(form.Nodes.Select(n => n.Ux).ToArray());
                }
            }
        }
        return result;
    }

    private static void VerifyFailedStage(string directory)
    {
        using (var form = CreateForm())
        {
            FillFixture(form);
            using (var control = new Form_Calculate(form))
            {
                var handle = control.Handle; form.fmCal = control;
                Calculate(form, control, CalculationMethod.Manual, false);
                Calculate(form, control, CalculationMethod.Manual, false, "-1000000");
                string path = Path.Combine(directory, "failed-stage.acproj");
                form.SaveProjectFile(path);
                var result = ProjectFile.Read(path).Results.Single();
                Assert(result.Stages[1].Status.Contains("失败") && result.Status.Contains("失败"), "failed adjustment is saved as failure, not success");
                Assert(result.Stages[1].Supports[0].Stroke == 0, "rejected target is not saved as committed stroke");
            }
        }
    }
}
