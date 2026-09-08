using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ActiveControl
{
    public partial class Form_Main
    {
        private ProjectDocument project = new ProjectDocument();
        private string projectPath;
        private string savedInputSignature;
        private bool resultsDirty;
        private bool calculationBusy;
        private bool updatingResultSelection;
        private MethodResults selectedResults;
        private ToolStripMenuItem projectMenu, viewResultsMenu;
        private readonly Dictionary<CalculationMethod, ToolStripMenuItem> resultMenuItems =
            new Dictionary<CalculationMethod, ToolStripMenuItem>();
        private Label currentResultLabel;

        public bool IsCalculationBusy => calculationBusy || (Calculate != null && Calculate.IsAlive);
        internal MethodResults SelectedResults => selectedResults;
        private static string RecentProjectFile => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ActiveControl", "last-project.txt");

        private void InitializeProjectMenus()
        {
            projectMenu = new ToolStripMenuItem("工程菜单") { Font = msInputData.Font, Name = "projectMenu" };
            projectMenu.DropDownItems.Add("新建工程", null, (s, e) => NewProject());
            projectMenu.DropDownItems.Add("打开工程", null, (s, e) => OpenProject());
            projectMenu.DropDownItems.Add("保存工程", null, (s, e) => SaveProject());
            msInput.Items.Insert(0, projectMenu);

            viewResultsMenu = new ToolStripMenuItem("查看结果") { Font = msInputData.Font, Name = "viewResultsMenu" };
            foreach (CalculationMethod method in Enum.GetValues(typeof(CalculationMethod)))
            {
                CalculationMethod selected = method;
                var item = new ToolStripMenuItem(CalculationMethodNames.Get(method));
                item.Click += (s, e) => SelectMethodResults(selected, true);
                viewResultsMenu.DropDownItems.Add(item);
                resultMenuItems.Add(method, item);
            }
            msInput.Items.Add(viewResultsMenu);
            currentResultLabel = new Label
            {
                Name = "currentResultLabel", AutoSize = false, AutoEllipsis = true,
                Font = new Font("微软雅黑", 10), Text = "当前结果：无"
            };
            Controls.Add(currentResultLabel);
            LayoutProjectResultControls();
            Shown += (s, e) => LayoutProjectResultControls();
            cbStage.SelectedIndexChanged += (s, e) => ResultSelectionChanged();
            cbFigType.SelectedIndexChanged += (s, e) => ResultSelectionChanged();
            FormClosing += ProjectFormClosing;
        }

        private void LayoutProjectResultControls()
        {
            // 状态标签占独立的一行，避免覆盖施工阶段选择框；按实际缩放后的控件高度排布。
            btnCalculate.Top = msInput.Bottom + 8;
            gbDraw.Top = btnCalculate.Bottom + 10;
            currentResultLabel.SetBounds(Chart.Left, gbDraw.Bottom + 6, Chart.Width, currentResultLabel.Font.Height + 8);
            Chart.Top = currentResultLabel.Bottom + 6;
            Chart.Height = Math.Max(180, ClientSize.Height - Chart.Top - 12);
        }

        private void LoadLastProject()
        {
            ResetProjectData();
            try
            {
                if (File.Exists(RecentProjectFile))
                {
                    string path = File.ReadAllText(RecentProjectFile, Encoding.UTF8).Trim();
                    if (path.Length > 0 && File.Exists(path))
                    {
                        LoadProjectFile(path);
                        return;
                    }
                    if (path.Length > 0) PrintString("上次工程文件不存在，请从工程菜单选择打开工程。");
                }
            }
            catch (Exception ex) { PrintString("上次工程恢复失败：" + ex.Message); }
            savedInputSignature = ProjectInputs.Capture(this).Signature();
            PrintString("当前为空白工程，可录入数据或打开已保存的工程。");
        }

        private bool AllowProjectOperation()
        {
            if (!IsCalculationBusy) return true;
            MessageBox.Show(this, "计算正在进行，请暂停并终止计算后再操作工程或切换结果。", "提示");
            return false;
        }

        private bool ConfirmUnsavedChanges()
        {
            if (!resultsDirty && ProjectInputs.Capture(this).Signature() == savedInputSignature) return true;
            var answer = MessageBox.Show(this, "当前工程有未保存的输入或计算结果，是否先保存？", "工程",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (answer == DialogResult.Cancel) return false;
            return answer != DialogResult.Yes || SaveProject();
        }

        private void NewProject()
        {
            if (!AllowProjectOperation() || !ConfirmUnsavedChanges()) return;
            ResetProjectData();
            savedInputSignature = ProjectInputs.Capture(this).Signature();
            RememberProjectPath("");
            PrintString("已新建空白工程。原工程文件保留在磁盘中。");
        }

        private void ResetProjectData()
        {
            if (fmCal != null && !fmCal.IsDisposed) fmCal.Close();
            fmCal = null;
            project = new ProjectDocument();
            projectPath = null;
            selectedResults = null;
            resultsDirty = false;
            Supports.Clear(); SoilLayers.Clear(); Loadcases.Clear(); LocalLoads.Clear();
            foreach (string name in ProjectInputs.ParameterNames)
                typeof(Form_Main).GetField(name).SetValue(this, 0.0);
            EpsDefor = 8.0e-4;
            EnableWater = Loadcase.EnableWater = false;
            WaterTableElev = Loadcase.WaterTableElev = -9999;
            Loadcase.UseNonlinearSoilSpring = false;
            ResetCalculationData();
            rtbOutputWindow.Clear();
            Chart.Series.Clear(); Chart.Titles.Clear();
            updatingResultSelection = true;
            cbStage.Items.Clear(); cbStage.Text = "";
            cbFigType.SelectedItem = "位移";
            updatingResultSelection = false;
            UpdateProjectLabels();
        }

        internal void ResetCalculationData()
        {
            Nodes.Clear(); Elements.Clear();
            DispSum = AliveSum = InistrnSum = IniForceSum = JackStrokeSum = null;
            JackStrokeCurrent = null;
            UxMax = UxMin = MomMax = MomMin = FxMax = FxMin = FyMax = FyMin = null;
            Calculate = null; mre.Set();
            Loadcase.CurLCNo = Loadcase.ActSupCount = Loadcase.ExcCount = 0;
            Loadcase.CurElev = ElevOfGround;
            Loadcase.AdjSupIndex.Clear(); Loadcase.SlabElemIndex.Clear();
        }

        private void OpenProject()
        {
            if (!AllowProjectOperation()) return;
            using (var dialog = new OpenFileDialog { Filter = "基坑工程文件 (*.acproj)|*.acproj", RestoreDirectory = true })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    // 先完整验证，错误文件不清空当前工程；确认保存完成后才切换。
                    var loaded = ProjectFile.Read(dialog.FileName);
                    if (!ConfirmUnsavedChanges()) return;
                    ApplyProjectDocument(loaded, dialog.FileName);
                    RememberProjectPath(projectPath);
                }
                catch (Exception ex) { MessageBox.Show(this, "打开工程失败：" + ex.Message, "工程读取错误"); }
            }
        }

        public void LoadProjectFile(string path)
        {
            if (IsCalculationBusy) throw new InvalidOperationException("计算中不能打开工程。");
            var loaded = ProjectFile.Read(path);
            ApplyProjectDocument(loaded, path);
        }

        private void ApplyProjectDocument(ProjectDocument loaded, string path)
        {
            // 所有转换在替换当前模型之前完成。
            var soils = loaded.Inputs.CreateSoils();
            var supports = loaded.Inputs.CreateSupports();
            var stages = loaded.Inputs.CreateLoadcases();
            var loads = loaded.Inputs.CreateLocalLoads();
            ResetProjectData();
            project = loaded;
            projectPath = Path.GetFullPath(path);
            SoilLayers.AddRange(soils); Supports.AddRange(supports);
            Loadcases.AddRange(stages); LocalLoads.AddRange(loads);
            loaded.Inputs.Parameters.Apply(this, ProjectInputs.ParameterNames);
            EnableWater = Loadcase.EnableWater = loaded.Inputs.EnableWater;
            WaterTableElev = Loadcase.WaterTableElev = loaded.Inputs.WaterTableElev;
            Loadcase.UseNonlinearSoilSpring = loaded.Inputs.UseNonlinearSoilSpring;
            Loadcase.CurElev = ElevOfGround;
            savedInputSignature = ProjectInputs.Capture(this).Signature();
            var available = project.Results.FirstOrDefault(r => loaded.SelectedMethod == r.Method && r.Stages.Count > 0)
                ?? project.Results.FirstOrDefault(r => r.Stages.Count > 0);
            if (available != null)
            {
                SelectMethodResults(available.Method, false);
                updatingResultSelection = true;
                if (cbStage.Items.Contains(loaded.SelectedStage)) cbStage.SelectedItem = loaded.SelectedStage;
                if (cbFigType.Items.Contains(loaded.FigureType)) cbFigType.SelectedItem = loaded.FigureType;
                updatingResultSelection = false;
                DrawSavedResults();
            }
            else PrintString("工程已打开，尚无计算结果。可以开始计算。");
            UpdateProjectLabels();
        }

        private bool SaveProject()
        {
            if (!AllowProjectOperation()) return false;
            using (var dialog = new SaveFileDialog
            {
                Filter = "基坑工程文件 (*.acproj)|*.acproj", DefaultExt = "acproj", AddExtension = true,
                OverwritePrompt = true, RestoreDirectory = true,
                InitialDirectory = projectPath == null ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Path.GetDirectoryName(projectPath),
                FileName = projectPath == null ? "未命名工程.acproj" : Path.GetFileName(projectPath)
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return false;
                try
                {
                    SaveProjectFile(dialog.FileName);
                    RememberProjectPath(projectPath);
                    PrintString("工程已保存：" + projectPath);
                    return true;
                }
                catch (Exception ex) { MessageBox.Show(this, "保存工程失败：" + ex.Message, "工程保存错误"); return false; }
            }
        }

        public void SaveProjectFile(string path)
        {
            if (IsCalculationBusy) throw new InvalidOperationException("计算中不能保存工程，请先结束本次计算。");
            var document = new ProjectDocument
            {
                Name = Path.GetFileNameWithoutExtension(path), SavedAt = DateTime.Now.ToString("O"),
                Inputs = ProjectInputs.Capture(this), Results = project.Results,
                SelectedMethod = selectedResults?.Method, SelectedStage = cbStage.Text, FigureType = cbFigType.Text
            };
            ProjectFile.Write(Path.GetFullPath(path), document);
            project = document;
            projectPath = Path.GetFullPath(path);
            savedInputSignature = document.Inputs.Signature();
            resultsDirty = false;
            UpdateProjectLabels();
        }

        private void RememberProjectPath(string path)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(RecentProjectFile));
                File.WriteAllText(RecentProjectFile, path, Encoding.UTF8);
            }
            catch (Exception ex) { PrintString("工程数据已保留，但无法记录最近工程路径：" + ex.Message); }
        }

        private void ProjectFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowProjectOperation() || !ConfirmUnsavedChanges()) e.Cancel = true;
        }

        internal void InputsEdited()
        {
            UpdateProjectLabels();
        }

        internal void SetCalculationBusy(bool busy)
        {
            calculationBusy = busy;
            projectMenu.Enabled = msInputData.Enabled = viewResultsMenu.Enabled = !busy;
            数据导出ToolStripMenuItem.Enabled = btnDraw.Enabled = btnWriteLogToFile.Enabled = !busy;
            cbStage.Enabled = cbFigType.Enabled = !busy;
            btnClearOutputWindow.Enabled = !busy;
        }

        internal MethodResults BeginMethodResults(CalculationMethod method, ProjectInputs inputs)
        {
            var result = new MethodResults
            {
                Method = method, Inputs = inputs, Status = "计算中",
                StartedAt = DateTime.Now.ToString("O"), UpdatedAt = DateTime.Now.ToString("O")
            };
            // 每种方法独立一份；新的计算开始后只替换该方法，其他方法的快照保持不变。
            project.Results.RemoveAll(r => r.Method == method);
            project.Results.Add(result);
            selectedResults = result;
            resultsDirty = true;
            rtbOutputWindow.Clear();
            Chart.Series.Clear(); Chart.Titles.Clear();
            UpdateProjectLabels();
            return result;
        }

        internal void ResumeMethodResults(MethodResults result)
        {
            selectedResults = result;
            rtbOutputWindow.Text = result.Log;
            UpdateProjectLabels();
        }

        internal void CaptureStageResults(MethodResults result, Forms.Form_Calculate control, string status, string manualInput)
        {
            var stage = new StageResults
            {
                Number = Loadcase.CurLCNo, Status = status, ManualInput = manualInput,
                ExcavationElevation = Loadcase.CurElev,
                Nodes = Nodes.Select(n => new SavedNode { No = n.No, X = n.Nx, Y = n.Ny, Ux = n.Ux, Uy = n.Uy, Rot = n.Rot }).ToList(),
                Elements = new List<SavedElement>(), Supports = new List<SavedSupport>(),
                WallNodeIndices = control.CM_Node_ECS.ToArray(), WallElementIndices = control.CM_Elem_ECS.ToArray(),
                AppliedLoads = control.Fs.ToArray(), Reactions = control.RForce.ToArray()
            };
            foreach (var element in Elements)
            {
                element.getNodalForce();
                stage.Elements.Add(new SavedElement
                {
                    No = element.No, LeftNo = element.Left.No, RightNo = element.Right.No,
                    Type = element.ElementType, Material = element.Material.Name, E = element.Material.Emodulus,
                    Area = element.RealConstant.Area, Iz = element.RealConstant.Iz,
                    InitialStrain = element.RealConstant.IniStrn, Alive = element.isAlive,
                    Forces = new[] { element.iFx, element.iFy, element.iMom, element.jFx, element.jFy, element.jMom },
                    EquivalentLoads = new[] { element.iFx0, element.iFy0, element.iMom0, element.jFx0, element.jFy0, element.jMom0 }
                });
            }
            for (int i = 0; i < control.CM_Elem_Supports.Count; i++)
            {
                var element = Elements[control.CM_Elem_Supports[i]];
                double length = Math.Sqrt(Math.Pow(element.Right.Nx - element.Left.Nx, 2) + Math.Pow(element.Right.Ny - element.Left.Ny, 2));
                double ea = element.Material.Emodulus * element.RealConstant.Area;
                stage.Supports.Add(new SavedSupport
                {
                    Index = i, ElementNo = element.No, Stroke = JackStrokeCurrent[i], ContactUx = element.Right.Ux,
                    Compression = element.isAlive && Math.Abs(ea) > 1e-12 ? -element.jFx * length / ea : 0
                });
            }
            result.Stages.Add(stage);
            result.Envelope = new ResultEnvelope
            {
                WallNodeY = control.CM_Node_ECS.Select(i => Nodes[i].Ny).ToArray(),
                WallElementY = control.CM_Elem_ECS.Select(i => Elements[i].Left.Ny).ToArray(),
                SupportY = control.CM_Elem_Supports.Select(i => Elements[i].Left.Ny).ToArray(),
                UxMax = UxMax.ToArray(), UxMin = UxMin.ToArray(), MomMax = MomMax.ToArray(), MomMin = MomMin.ToArray(),
                FyMax = FyMax.ToArray(), FyMin = FyMin.ToArray(), FxMax = FxMax.ToArray(), FxMin = FxMin.ToArray()
            };
            result.Log = rtbOutputWindow.Text;
            result.UpdatedAt = DateTime.Now.ToString("O");
            resultsDirty = true;
            updatingResultSelection = true;
            cbStage.Items.Clear(); cbStage.Items.Add("包络");
            foreach (var item in result.Stages) cbStage.Items.Add(item.Number.ToString());
            cbStage.SelectedItem = stage.Number.ToString();
            cbFigType.SelectedItem = "位移";
            updatingResultSelection = false;
            DrawSavedResults();
        }

        internal void FinishMethodResults(MethodResults result, string status)
        {
            result.Status = status;
            result.Log = rtbOutputWindow.Text;
            result.UpdatedAt = DateTime.Now.ToString("O");
            resultsDirty = true;
            UpdateProjectLabels();
        }

        public bool SelectMethodResults(CalculationMethod method, bool showMessage)
        {
            if (IsCalculationBusy) return false;
            var result = project.Results.SingleOrDefault(r => r.Method == method);
            if (result == null || result.Stages.Count == 0)
            {
                if (showMessage) MessageBox.Show(this, "当前工程没有“" + CalculationMethodNames.Get(method) +
                    "”的可查看计算结果，请先执行计算。" + (result == null ? "" : "\r\n状态：" + result.Status), "没有数据");
                return false;
            }
            selectedResults = result;
            rtbOutputWindow.Text = result.Log ?? "";
            updatingResultSelection = true;
            string previousStage = cbStage.Text;
            cbStage.Items.Clear(); cbStage.Items.Add("包络");
            foreach (var stage in result.Stages) cbStage.Items.Add(stage.Number.ToString());
            cbStage.SelectedItem = cbStage.Items.Contains(previousStage) ? previousStage : result.Stages.Last().Number.ToString();
            if (cbFigType.SelectedIndex < 0) cbFigType.SelectedItem = "位移";
            updatingResultSelection = false;
            UpdateProjectLabels();
            DrawSavedResults();
            return true;
        }

        private void UpdateProjectLabels()
        {
            if (currentResultLabel == null) return;
            bool stale = selectedResults != null && selectedResults.Inputs.Signature() != ProjectInputs.Capture(this).Signature();
            string text = selectedResults == null ? "当前结果：无" : "当前结果：" + CalculationMethodNames.Get(selectedResults.Method) +
                " · " + selectedResults.DisplayStatus() + (stale ? " · 对应旧输入" : "");
            currentResultLabel.Text = text;
            currentResultLabel.ForeColor = stale ? Color.DarkOrange : Color.Black;
            Text = (projectPath == null ? "未命名工程" : Path.GetFileNameWithoutExtension(projectPath)) + " - 基坑支护计算程序";
            foreach (var pair in resultMenuItems)
            {
                pair.Value.Checked = selectedResults != null && selectedResults.Method == pair.Key;
                var data = project.Results.FirstOrDefault(r => r.Method == pair.Key);
                pair.Value.Text = CalculationMethodNames.Get(pair.Key) + "（" + (data == null ? "未计算" : data.DisplayStatus()) + "）";
            }
        }

        private void ResultSelectionChanged()
        {
            if (!updatingResultSelection && !IsCalculationBusy && selectedResults != null) DrawSavedResults();
        }
    }
}
