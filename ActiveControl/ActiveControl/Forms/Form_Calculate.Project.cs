using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

#pragma warning disable CS0197 // 与既有求解接口一致：在单一计算线程中传入主窗体节点列表。

namespace ActiveControl.Forms
{
    public partial class Form_Calculate
    {
        private MethodResults activeRun;
        private bool stageAdjustmentSucceeded = true;

        private CalculationMethod SelectedCalculationMethod()
        {
            if (rbtGlobalPSO.Checked) return CalculationMethod.GlobalPSO;
            if (rbtPartitionPSO.Checked) return CalculationMethod.PartitionPSO;
            if (rbtZeroDisp.Checked) return CalculationMethod.ZeroDisp;
            if (rbtManual.Checked) return CalculationMethod.Manual;
            return CalculationMethod.Direct;
        }

        private void CalculationFormClosing(object sender, FormClosingEventArgs e)
        {
            if (mf.IsCalculationBusy)
            {
                e.Cancel = true;
                MessageBox.Show(this, "请先终止计算，再关闭计算控制窗口。", "计算中");
            }
        }

        private void StartCalculation(bool allStages)
        {
            if (mf.IsCalculationBusy) return;
            CalculationMethod method = SelectedCalculationMethod();
            if (allStages && method == CalculationMethod.Manual)
            {
                mf.PrintString("手动赋值不支持全部计算，请输入本阶段轴力后执行逐工况计算。");
                return;
            }

            try
            {
                string signature = ProjectInputs.Capture(mf).Signature();
                bool newRun = activeRun == null || activeRun.Method != method ||
                    activeRun.Inputs.Signature() != signature || activeRun.Status == "已终止" ||
                    activeRun.Status == "计算异常" || activeRun.Stages.Count >= mf.Loadcases.Count;
                if (newRun)
                {
                    mf.SetCalculationBusy(true);
                    mf.ResetCalculationData();
                    Init();
                    if (mf.DispSum == null)
                    {
                        mf.SetCalculationBusy(false);
                        return; // 初始化输入校核未通过。
                    }
                    activeRun = mf.BeginMethodResults(method, ProjectInputs.Capture(mf));
                }
                else
                {
                    activeRun.Status = "计算中";
                    mf.ResumeMethodResults(activeRun);
                }

                string manualInput = method == CalculationMethod.Manual ? tbForces.Text : null;
                Vector<double> manualForce = method == CalculationMethod.Manual ? ParseManualForce(manualInput) : null;
                int stageCount = allStages ? mf.Loadcases.Count - Loadcase.CurLCNo : 1;
                mf.mre.Set();
                mf.SetCalculationBusy(true);
                gbOptMethod.Enabled = btnStartOnce.Enabled = false;
                btnStartAll.Visible = false;
                btnPause.Visible = true;
                btnContinue.Visible = false;

                mf.Calculate = new Thread(() => RunArchivedStages(stageCount, manualForce, manualInput)) { IsBackground = true };
                mf.Calculate.Start();
            }
            catch (Exception ex)
            {
                mf.SetCalculationBusy(false);
                mf.PrintString("计算准备失败：" + ex.Message);
                if (activeRun != null && activeRun.Method == method)
                    mf.FinishMethodResults(activeRun, activeRun.Stages.Count == 0 ? "未计算" : "部分完成");
            }
        }

        private Vector<double> ParseManualForce(string text)
        {
            var indices = Loadcase.AdjSupIndex.ToList();
            var stage = mf.Loadcases[Loadcase.CurLCNo];
            if (stage.IsActiveSupport && mf.Supports[Loadcase.ActSupCount].AdjAble)
                indices.Add(Loadcase.ActSupCount);
            var parts = (text ?? "").Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != indices.Count)
                throw new FormatException("本阶段需输入" + indices.Count + "个支撑目标轴力，使用/分隔；不调节的位置填写_。");
            var force = Vector<double>.Build.Dense(indices.Count);
            for (int i = 0; i < parts.Length; i++)
            {
                force[i] = parts[i].Trim() == "_" ? 1e12 : Convert.ToDouble(parts[i]) * 1e3;
                if (double.IsNaN(force[i]) || double.IsInfinity(force[i])) throw new FormatException("手动轴力必须是有效数值。");
            }
            return force;
        }

        private void RunArchivedStages(int count, Vector<double> manualForce, string manualInput)
        {
            string finalStatus = "部分完成";
            try
            {
                for (int i = 0; i < count; i++)
                {
                    mf.mre.WaitOne();
                    stageAdjustmentSucceeded = true;
                    mf.PrintString("施工阶段 " + (Loadcase.CurLCNo + 1) + " 开始计算……");
                    Construction();
                    switch (activeRun.Method)
                    {
                        case CalculationMethod.Direct:
                            SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                            mf.PrintString("施工阶段 " + Loadcase.CurLCNo + " 计算完成！");
                            break;
                        case CalculationMethod.GlobalPSO:
                            SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                            GlobalPSO();
                            break;
                        case CalculationMethod.PartitionPSO:
                            SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                            PartitionPSO();
                            break;
                        case CalculationMethod.ZeroDisp: ZeroDisp(); break;
                        case CalculationMethod.Manual:
                            SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                            Manual(manualForce);
                            break;
                    }
                    // 失败的试算不作为已调节结果保存；保留该阶段未执行调节的结构状态，并明确标记失败。
                    if (!stageAdjustmentSucceeded)
                        SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                    UpdateEnvData();
                    CopyToSum();
                    mf.Invoke(new Action(() => mf.CaptureStageResults(activeRun, this,
                        stageAdjustmentSucceeded ? "完成" : "调节失败（未执行目标调节）", manualInput)));
                }
                bool failed = activeRun.Stages.Any(s => s.Status != "完成");
                finalStatus = activeRun.Stages.Count == mf.Loadcases.Count
                    ? (failed ? "计算结束，存在调节失败" : "已完成") : (failed ? "部分完成，存在调节失败" : "部分完成");
            }
            catch (ThreadAbortException)
            {
                finalStatus = "已终止";
                // 只保留已经完整归档的阶段。终止在小步中的模型不能直接继续。
                Thread.ResetAbort();
                mf.mre.Set();
                mf.PrintString("计算已终止，保留已完成阶段的结果；重新启动计算将从第一阶段开始。");
            }
            catch (Exception ex)
            {
                finalStatus = "计算异常";
                mf.mre.Set();
                mf.PrintString("计算异常，已完成阶段结果保留：" + ex.Message);
            }
            finally
            {
                string status = finalStatus;
                mf.BeginInvoke(new Action(() =>
                {
                    mf.SetCalculationBusy(false);
                    mf.FinishMethodResults(activeRun, status);
                    if (!IsDisposed)
                    {
                        gbOptMethod.Enabled = btnStartOnce.Enabled = true;
                        btnStartAll.Visible = true;
                        btnPause.Visible = btnContinue.Visible = false;
                    }
                }));
            }
        }
    }
}
