using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ActiveControl;
using ActiveControl.Forms;
using MathNet.Numerics.LinearAlgebra;

internal static class CompareRunner
{
    private static readonly BindingFlags PrivateInstance =
        BindingFlags.Instance | BindingFlags.NonPublic;

    [STAThread]
    private static int Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Directory.CreateDirectory("results");

        RunCase(false, Path.Combine("results", "linear.tsv"));
        RunCase(true, Path.Combine("results", "nonlinear.tsv"));
        WriteComparison(Path.Combine("results", "linear.tsv"),
            Path.Combine("results", "nonlinear.tsv"),
            Path.Combine("results", "comparison.tsv"));

        return 0;
    }

    private static void RunCase(bool nonlinear, string outputPath)
    {
        ResetLoadcaseStatics();
        Loadcase.UseNonlinearSoilSpring = nonlinear;

        using (Form_Main mf = new Form_Main())
        {
            mf.ShowInTaskbar = false;
            mf.WindowState = FormWindowState.Minimized;
            mf.Show();
            Application.DoEvents();

            Loadcase.UseNonlinearSoilSpring = nonlinear;

            using (Form_Calculate cal = new Form_Calculate(mf))
            {
                mf.fmCal = cal;

                MethodInfo construction = typeof(Form_Calculate).GetMethod("Construction", PrivateInstance);
                MethodInfo solveWithMode = typeof(Form_Calculate).GetMethod("SolveWithMode", PrivateInstance);
                MethodInfo updateEnvData = typeof(Form_Calculate).GetMethod("UpdateEnvData", PrivateInstance);
                MethodInfo copyToSum = typeof(Form_Calculate).GetMethod("CopyToSum", PrivateInstance);

                StiffnessStats initStats = GetSoilSpringStats(mf, cal, activeOnly: false);

                List<StageResult> results = new List<StageResult>();
                for (int stage = 0; stage < mf.Loadcases.Count; stage++)
                {
                    construction.Invoke(cal, null);
                    StiffnessStats preStats = GetSoilSpringStats(mf, cal, activeOnly: true);

                    object[] args = new object[]
                    {
                        mf.Elements,
                        mf.Nodes,
                        cal.Fs,
                        cal.ConstrainedDOFIndex,
                        null,
                        null
                    };
                    solveWithMode.Invoke(cal, args);
                    mf.Nodes = (List<Node>)args[1];
                    cal.Disp = (Vector<double>)args[4];
                    cal.RForce = (Vector<double>)args[5];

                    updateEnvData.Invoke(cal, null);
                    copyToSum.Invoke(cal, null);

                    StiffnessStats postStats = GetSoilSpringStats(mf, cal, activeOnly: true);
                    results.Add(new StageResult
                    {
                        Stage = stage + 1,
                        CurElev = Loadcase.CurElev,
                        MaxAbsUxMm = GetMaxWallDisplacementMm(mf, cal),
                        PreAvgK = preStats.Avg,
                        PreMinK = preStats.Min,
                        PreMaxK = preStats.Max,
                        PreCount = preStats.Count,
                        PostAvgK = postStats.Avg,
                        PostMinK = postStats.Min,
                        PostMaxK = postStats.Max,
                        PostCount = postStats.Count
                    });

                    Application.DoEvents();
                }

                using (StreamWriter sw = new StreamWriter(outputPath, false, Encoding.UTF8))
                {
                    sw.WriteLine("Mode\tUseNonlinear\tInitCount\tInitAvgK_N_per_m\tInitMinK_N_per_m\tInitMaxK_N_per_m");
                    sw.WriteLine(string.Join("\t",
                        nonlinear ? "Nonlinear" : "Linear",
                        nonlinear ? "True" : "False",
                        initStats.Count,
                        F(initStats.Avg),
                        F(initStats.Min),
                        F(initStats.Max)));
                    sw.WriteLine();
                    sw.WriteLine("Stage\tCurElev\tMaxAbsUx_mm\tPreActiveCount\tPreAvgK_N_per_m\tPreMinK_N_per_m\tPreMaxK_N_per_m\tPostActiveCount\tPostAvgK_N_per_m\tPostMinK_N_per_m\tPostMaxK_N_per_m");
                    foreach (StageResult r in results)
                    {
                        sw.WriteLine(string.Join("\t",
                            r.Stage,
                            F(r.CurElev),
                            F(r.MaxAbsUxMm),
                            r.PreCount,
                            F(r.PreAvgK),
                            F(r.PreMinK),
                            F(r.PreMaxK),
                            r.PostCount,
                            F(r.PostAvgK),
                            F(r.PostMinK),
                            F(r.PostMaxK)));
                    }
                }

                Console.WriteLine((nonlinear ? "Nonlinear" : "Linear") + " complete: " + outputPath);
            }

            mf.Close();
        }
    }

    private static void ResetLoadcaseStatics()
    {
        Loadcase.CurLCNo = 0;
        Loadcase.CurElev = 0;
        Loadcase.ExcCount = 0;
        Loadcase.ActSupCount = 0;
        Loadcase.AdjSupIndex = new List<int>();
        Loadcase.SlabElemIndex = new List<int>();
        Loadcase.EnableWater = false;
        Loadcase.WaterTableElev = -9999;
        Loadcase.UseNonlinearSoilSpring = false;
    }

    private static StiffnessStats GetSoilSpringStats(Form_Main mf, Form_Calculate cal, bool activeOnly)
    {
        IEnumerable<double> values = cal.CM_Elem_SoilSpring
            .Select(i => mf.Elements[i])
            .Where(e => !activeOnly || e.isAlive)
            .Select(e => e.RealConstant.Area)
            .Where(k => k > 0 && !double.IsNaN(k) && !double.IsInfinity(k));

        List<double> list = values.ToList();
        if (list.Count == 0)
            return new StiffnessStats();

        return new StiffnessStats
        {
            Count = list.Count,
            Avg = list.Average(),
            Min = list.Min(),
            Max = list.Max()
        };
    }

    private static double GetMaxWallDisplacementMm(Form_Main mf, Form_Calculate cal)
    {
        double max = 0.0;
        foreach (int nodeIndex in cal.CM_Node_ECS)
        {
            double value = Math.Abs(mf.Nodes[nodeIndex].Ux) * 1000.0;
            if (value > max)
                max = value;
        }
        return max;
    }

    private static void WriteComparison(string linearPath, string nonlinearPath, string outputPath)
    {
        Dictionary<int, StageResult> linear = ReadStageResults(linearPath);
        Dictionary<int, StageResult> nonlinear = ReadStageResults(nonlinearPath);

        using (StreamWriter sw = new StreamWriter(outputPath, false, Encoding.UTF8))
        {
            sw.WriteLine("Stage\tLinearUx_mm\tNonlinearUx_mm\tUxDelta_mm\tUxRatio_NL_to_L\tLinearPreAvgK\tNonlinearPreAvgK\tPreKRatio_NL_to_L\tLinearPostAvgK\tNonlinearPostAvgK\tPostKRatio_NL_to_L");
            foreach (int stage in linear.Keys.OrderBy(i => i))
            {
                StageResult l = linear[stage];
                StageResult n = nonlinear[stage];
                sw.WriteLine(string.Join("\t",
                    stage,
                    F(l.MaxAbsUxMm),
                    F(n.MaxAbsUxMm),
                    F(n.MaxAbsUxMm - l.MaxAbsUxMm),
                    F(SafeRatio(n.MaxAbsUxMm, l.MaxAbsUxMm)),
                    F(l.PreAvgK),
                    F(n.PreAvgK),
                    F(SafeRatio(n.PreAvgK, l.PreAvgK)),
                    F(l.PostAvgK),
                    F(n.PostAvgK),
                    F(SafeRatio(n.PostAvgK, l.PostAvgK))));
            }
        }
    }

    private static Dictionary<int, StageResult> ReadStageResults(string path)
    {
        Dictionary<int, StageResult> dict = new Dictionary<int, StageResult>();
        bool inStages = false;
        foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
        {
            if (line.StartsWith("Stage\t", StringComparison.Ordinal))
            {
                inStages = true;
                continue;
            }
            if (!inStages || string.IsNullOrWhiteSpace(line))
                continue;

            string[] p = line.Split('\t');
            StageResult r = new StageResult
            {
                Stage = int.Parse(p[0], CultureInfo.InvariantCulture),
                CurElev = double.Parse(p[1], CultureInfo.InvariantCulture),
                MaxAbsUxMm = double.Parse(p[2], CultureInfo.InvariantCulture),
                PreCount = int.Parse(p[3], CultureInfo.InvariantCulture),
                PreAvgK = double.Parse(p[4], CultureInfo.InvariantCulture),
                PreMinK = double.Parse(p[5], CultureInfo.InvariantCulture),
                PreMaxK = double.Parse(p[6], CultureInfo.InvariantCulture),
                PostCount = int.Parse(p[7], CultureInfo.InvariantCulture),
                PostAvgK = double.Parse(p[8], CultureInfo.InvariantCulture),
                PostMinK = double.Parse(p[9], CultureInfo.InvariantCulture),
                PostMaxK = double.Parse(p[10], CultureInfo.InvariantCulture)
            };
            dict[r.Stage] = r;
        }
        return dict;
    }

    private static double SafeRatio(double numerator, double denominator)
    {
        if (Math.Abs(denominator) < 1e-12)
            return double.NaN;
        return numerator / denominator;
    }

    private static string F(double value)
    {
        return value.ToString("G17", CultureInfo.InvariantCulture);
    }

    private struct StiffnessStats
    {
        public int Count;
        public double Avg;
        public double Min;
        public double Max;
    }

    private sealed class StageResult
    {
        public int Stage;
        public double CurElev;
        public double MaxAbsUxMm;
        public int PreCount;
        public double PreAvgK;
        public double PreMinK;
        public double PreMaxK;
        public int PostCount;
        public double PostAvgK;
        public double PostMinK;
        public double PostMaxK;
    }
}
