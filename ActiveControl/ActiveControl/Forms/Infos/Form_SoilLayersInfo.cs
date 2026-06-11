using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ActiveControl.Forms
{
    public partial class Form_SoilLayersInfo : Form
    {

        Form_Main mf;

        public Form_SoilLayersInfo(Form_Main mf)
        {
            InitializeComponent();
            this.mf = mf;
            outputSoilLayersInfoToLV();
        }

        private void btnClearSoilLayerInfo_Click(object sender, EventArgs e)        // 【按钮-土层】清空
        {
            DialogResult AF = MessageBox.Show("确认清除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (AF == DialogResult.OK)
            {
                lvSoilLayers.Items.Clear();
                mf.SoilLayers.Clear();
                mf.PrintString("土层数据已清空！");
            }
        }

        private void btnDeleSoilLayerInfo_Click(object sender, EventArgs e)         // 【按钮-土层】删除
        {

            if (lvSoilLayers.SelectedItems.Count > 0)
            {
                DialogResult AF = MessageBox.Show("确认删除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (AF == DialogResult.OK)
                {
                    // 注意以下两行顺序不能调换，因为ListView中的项移除后，无法再调用 SelectedIndices
                    mf.SoilLayers.RemoveAt(lvSoilLayers.SelectedIndices[0]);
                    lvSoilLayers.Items.Remove(lvSoilLayers.SelectedItems[0]);
                    mf.PrintString("土层数据删除成功！");
                }
            }
            else
                MessageBox.Show("请勾选待删除项！", "错误");
        }

        private void btnReadSoilLayerInfo_Click(object sender, EventArgs e)         // 【按钮-土层】读取
        {
            try
            {
                // 打开文件对话框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = Application.StartupPath,
                    Filter = "文本文件|*.txt",
                    RestoreDirectory = true,
                    FilterIndex = 1
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 读文件前清空土层信息
                    lvSoilLayers.Items.Clear();
                    mf.SoilLayers.Clear();

                    // 读文件流
                    StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    char[] deli = { '\t' };
                    string line = sr.ReadLine();                             // 读取第一行

                    // 判断文件格式：检查是否有地下水信息
                    bool hasWaterInfo = false;
                    if (line.StartsWith("EnableWater"))
                    {
                        hasWaterInfo = true;

                        // 读取地下水开关
                        string[] waterEnableUnit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        if (waterEnableUnit.Length >= 2)
                        {
                            bool enableWater = waterEnableUnit[1] == "True";
                            chkEnableWater.Checked = enableWater;
                            Loadcase.EnableWater = enableWater;
                        }

                        // 读取地下水位标高
                        line = sr.ReadLine();
                        if (line != null && line.StartsWith("WaterTableElev"))
                        {
                            string[] waterElevUnit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            if (waterElevUnit.Length >= 2)
                            {
                                double waterElev = Convert.ToDouble(waterElevUnit[1]);
                                if (waterElev > -9999)
                                {
                                    tbWaterTableElev.Text = waterElev.ToString("0.00");
                                    Loadcase.WaterTableElev = waterElev;
                                }
                            }
                        }

                        // 读取非线性土弹簧开关（新增）
                        line = sr.ReadLine();
                        if (line != null && line.StartsWith("UseNonlinearSoilSpring"))
                        {
                            string[] nonlinearUnit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            if (nonlinearUnit.Length >= 2)
                            {
                                bool useNonlinear = nonlinearUnit[1] == "True";
                                chkEnableNonlinear.Checked = useNonlinear;
                                Loadcase.UseNonlinearSoilSpring = useNonlinear;
                            }
                        }

                        // 读取土层数据表头
                        line = sr.ReadLine();
                    }
                    else
                    {
                        // 老格式文件，没有地下水信息
                        chkEnableWater.Checked = false;
                        Loadcase.EnableWater = false;
                        Loadcase.WaterTableElev = -9999;
                        chkEnableNonlinear.Checked = false;
                        Loadcase.UseNonlinearSoilSpring = false;
                    }

                    // 判断土层数据格式：新格式（10列）或老格式（9列）
                    bool isNewFormat = (line == "土层编号\t厚度\tc\tphi\tK0\tEs\tm\t重度\t土性\t水土模式");
                    bool isOldFormat = (line == "土层编号\t厚度\tc\tphi\tK0\tEs\tm\t重度\t土性");

                    if (isNewFormat || isOldFormat)
                    {
                        while (sr.Peek() > 0)
                        {
                            line = sr.ReadLine();
                            string[] unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
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
                            mf.SoilLayers.Add(sl);
                        }
                        outputSoilLayersInfoToLV();
                        sr.Close();
                        mf.PrintString("土层数据读取成功！");
                    }
                    else
                        mf.PrintString("文件数据格式不正确，请重新选择。");
                }
            }
            catch
            {
                mf.PrintString("文件数据格式不正确，请重新选择。");
            }
        }

        private void btnWriteSoilLayerInfo_Click(object sender, EventArgs e)        // 【按钮-土层】写入
        {
            // 打开文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Application.StartupPath,
                FileName = "1-SoilLayersInfo",
                Filter = "文本文件|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);      // false 指若已存在同名文件则进行覆盖

                // 写入地下水信息
                sw.WriteLine("EnableWater\t" + chkEnableWater.Checked.ToString());
                if (chkEnableWater.Checked && !string.IsNullOrEmpty(tbWaterTableElev.Text))
                {
                    sw.WriteLine("WaterTableElev\t" + tbWaterTableElev.Text);
                }
                else
                {
                    sw.WriteLine("WaterTableElev\t-9999");
                }

                // 写入非线性土弹簧设置（新增）
                sw.WriteLine("UseNonlinearSoilSpring\t" + chkEnableNonlinear.Checked.ToString());

                // 写入土层数据表头
                sw.WriteLine("土层编号\t厚度\tc\tphi\tK0\tEs\tm\t重度\t土性\t水土模式");

                // 写入土层数据
                for (int i = 0; i < mf.SoilLayers.Count(); i++)
                {
                    sw.WriteLine((i + 1).ToString("0") + "\t" + mf.SoilLayers[i].Thick.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].C.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].Phi.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].K0.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].Es.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].M.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].Gamma.ToString("0.00")
                                                       + "\t" + mf.SoilLayers[i].Type
                                                       + "\t" + mf.SoilLayers[i].WaterSoilMode);
                }
                sw.Close();
                mf.PrintString("土层数据写入成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }

        private void btnInsertSoilLayerInfo_Click(object sender, EventArgs e)       // 【按钮-土层】插入
        {
            try
            {
                // 读取输入栏数据
                double Thick = double.Parse(tbSoilLayersThick.Text);
                double C = double.Parse(tbSoilLayersC.Text);
                double Phi = double.Parse(tbSoilLayersPhi.Text);
                double K0 = double.Parse(tbSoilLayersK0.Text);
                double Es = double.Parse(tbSoilLayersEs.Text);
                double M = double.Parse(tbSoilLayersM.Text);
                double Gamma = double.Parse(tbSoilLayersGamma.Text);
                string Type = cbSoilLayersType.Text;

                // 并入土层信息
                SoilLayer s = new SoilLayer(Thick, C, Phi, K0, Es, M, Gamma, Type);
                mf.SoilLayers.Insert(lvSoilLayers.SelectedIndices[0], s);
                outputSoilLayersInfoToLV();

                // 输入栏清空
                tbSoilLayersThick.Text = "";
                tbSoilLayersC.Text = "";
                tbSoilLayersPhi.Text = "";
                tbSoilLayersK0.Text = "";
                tbSoilLayersEs.Text = "";
                tbSoilLayersM.Text = "";
                tbSoilLayersGamma.Text = "";
                cbSoilLayersType.Text = "";
                mf.PrintString("土层信息插入成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void btnAppendSoilLayerInfo_Click(object sender, EventArgs e)       // 【按钮-土层】追加
        {
            try
            {
                // 读取输入栏数据
                double Thick = double.Parse(tbSoilLayersThick.Text);
                double C = double.Parse(tbSoilLayersC.Text);
                double Phi = double.Parse(tbSoilLayersPhi.Text);
                double K0 = double.Parse(tbSoilLayersK0.Text);
                double Es = double.Parse(tbSoilLayersEs.Text);
                double M = double.Parse(tbSoilLayersM.Text);
                double Gamma = double.Parse(tbSoilLayersGamma.Text);
                string Type = cbSoilLayersType.Text;

                if (Thick * Phi * K0 * Es * M * Gamma == 0 || Type == "")
                    MessageBox.Show("数据不全，请检查后重新输入！", "错误");
                else
                {
                    // 创建土层对象
                    SoilLayer s;
                    if (chkEnableWater.Checked && cbWaterSoilMode.SelectedIndex >= 0)
                    {
                        // 考虑地下水：使用带水土模式的构造函数
                        string waterSoilMode = cbWaterSoilMode.Text;
                        s = new SoilLayer(Thick, C, Phi, K0, Es, M, Gamma, Type, waterSoilMode);
                    }
                    else
                    {
                        // 不考虑地下水：使用原构造函数
                        s = new SoilLayer(Thick, C, Phi, K0, Es, M, Gamma, Type);
                    }

                    mf.SoilLayers.Add(s);
                    outputSoilLayersInfoToLV();

                    // 输入栏清空
                    tbSoilLayersThick.Text = "";
                    tbSoilLayersC.Text = "";
                    tbSoilLayersPhi.Text = "";
                    tbSoilLayersK0.Text = "";
                    tbSoilLayersEs.Text = "";
                    tbSoilLayersM.Text = "";
                    tbSoilLayersGamma.Text = "";
                    cbSoilLayersType.Text = "";
                    cbWaterSoilMode.SelectedIndex = -1;
                    mf.PrintString("土层信息追加成功！");
                }
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void outputSoilLayersInfoToLV()                                     // 【方法-土层】将 List< > 输出至 ListView
        {
            lvSoilLayers.Items.Clear();
            for (int i = 0; i < mf.SoilLayers.Count; i++)
            {
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString("0");
                li.SubItems.Add(mf.SoilLayers[i].Thick.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].C.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].Phi.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].K0.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].Es.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].M.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].Gamma.ToString("0.00"));
                li.SubItems.Add(mf.SoilLayers[i].Type);
                li.SubItems.Add(mf.SoilLayers[i].WaterSoilMode);  // 新增：显示水土模式
                lvSoilLayers.Items.Add(li);
            }
        }

        private void Form_SoilLayersInfo_Load(object sender, EventArgs e)
        {
            // 初始化非线性土弹簧复选框状态
            chkEnableNonlinear.Checked = Loadcase.UseNonlinearSoilSpring;
        }

        private void chkEnableWater_CheckedChanged(object sender, EventArgs e)      // 【事件】考虑地下水复选框改变
        {
            tbWaterTableElev.Enabled = chkEnableWater.Checked;
            cbWaterSoilMode.Enabled = chkEnableWater.Checked;
        }

        private void cbSoilLayersType_SelectedIndexChanged(object sender, EventArgs e)  // 【事件】土性选择改变，自动推荐水土模式
        {
            if (!chkEnableWater.Checked) return;

            string soilType = cbSoilLayersType.Text;

            // 根据土性自动推荐水土模式
            if (soilType == "杂填土" || soilType == "砂质粉土" || soilType == "粉砂")
            {
                cbWaterSoilMode.SelectedIndex = 1;  // 水土分算
            }
            else if (soilType == "粉质黏土" || soilType == "淤泥质粘土")
            {
                cbWaterSoilMode.SelectedIndex = 2;  // 水土合算
            }
            else
            {
                cbWaterSoilMode.SelectedIndex = 0;  // 自动（根据土性）
            }
        }

        private void chkEnableNonlinear_CheckedChanged(object sender, EventArgs e)   // 【事件】启用非线性土弹簧复选框改变
        {
            Loadcase.UseNonlinearSoilSpring = chkEnableNonlinear.Checked;

            if (chkEnableNonlinear.Checked)
            {
                mf.PrintString("已启用非线性土弹簧计算（邓肯-张模型）");
            }
            else
            {
                mf.PrintString("已关闭非线性土弹簧计算（使用线性m法）");
            }
        }


    }
}
