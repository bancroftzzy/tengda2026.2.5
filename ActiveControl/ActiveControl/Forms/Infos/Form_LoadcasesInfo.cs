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
    public partial class Form_LoadcasesInfo : Form
    {

        Form_Main mf;

        public Form_LoadcasesInfo(Form_Main mf)
        {
            InitializeComponent();
            this.mf = mf;
            
            // 设置默认值
            cbLoadcasesIsActive.SelectedIndex = 1;  // 默认"否"
            cbIsRemoveSupport.SelectedIndex = 1;    // 默认"否"
            cbIsAddSlab.SelectedIndex = 1;          // 默认"否"
            
            outputLoadcasesInfoToLV();
        }

        private void btnClearLoadcaseInfo_Click(object sender, EventArgs e)         // 【按钮-工况】清空
        {
            DialogResult AF = MessageBox.Show("确认清除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (AF == DialogResult.OK)
            {
                lvLoadcases.Items.Clear();
                mf.Loadcases.Clear();
                mf.PrintString("工况数据已清空！");
            }
        }

        private void btnDeleLoadcaseInfo_Click(object sender, EventArgs e)          // 【按钮-工况】删除
        {
            if (lvLoadcases.SelectedItems.Count > 0)
            {
                DialogResult AF = MessageBox.Show("确认删除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (AF == DialogResult.OK)
                {
                    // 注意以下两行顺序不能调换，因为ListView中的项移除后，无法再调用 SelectedIndices
                    mf.Loadcases.RemoveAt(lvLoadcases.SelectedIndices[0]);
                    lvLoadcases.Items.Remove(lvLoadcases.SelectedItems[0]);
                    mf.PrintString("工况数据删除成功！");
                }
            }
            else
                MessageBox.Show("请勾选待删除项！", "错误");
        }

        private void btnReadLoadcaseInfo_Click(object sender, EventArgs e)          // 【按钮-工况】读取
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
                    // 读文件前清空工况信息
                    lvLoadcases.Items.Clear();
                    mf.Loadcases.Clear();

                    // 读文件流
                    StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    char[] deli = { '\t' };
                    string line = sr.ReadLine();                             // 读取表头
                    
                    // 支持多种表头格式（旧格式和新格式）
                    bool isOldFormat = (line == "工序编号\t开挖深度\t激活支撑");
                    bool isNewFormat = line.Contains("拆除支撑") || line.Contains("回筑深度");
                    
                    if (isOldFormat || isNewFormat)
                    {
                        while (sr.Peek() > 0)
                        {
                            line = sr.ReadLine();
                            string[] unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            
                            // 读取基本参数
                            double ExcavationDepth = Convert.ToDouble(unit[1]);
                            bool IsActiveSupport = unit[2] == "是" || unit[2] == "True" || unit[2].ToLower() == "true";
                            
                            // 读取扩展参数（拆换撑和回筑）
                            bool IsRemoveSupport = false;
                            int RemoveSupportIndex = -1;
                            bool IsAddSlab = false;
                            double SlabElevation = 0;
                            double SlabThickness = 0;
                            double BackfillDepth = 0;
                            
                            // 检查是否有扩展列
                            if (unit.Length > 3)
                            {
                                IsRemoveSupport = unit[3] == "是" || unit[3] == "True" || unit[3].ToLower() == "true";
                                if (unit.Length > 4)
                                    RemoveSupportIndex = Convert.ToInt32(unit[4]);
                                if (unit.Length > 5)
                                    IsAddSlab = unit[5] == "是" || unit[5] == "True" || unit[5].ToLower() == "true";
                                if (unit.Length > 6)
                                    SlabElevation = Convert.ToDouble(unit[6]);
                                if (unit.Length > 7)
                                    SlabThickness = Convert.ToDouble(unit[7]);
                                if (unit.Length > 8)
                                    BackfillDepth = Convert.ToDouble(unit[8]);
                            }
                            
                            Loadcase lc = new Loadcase(ExcavationDepth, IsActiveSupport,
                                                       IsRemoveSupport, RemoveSupportIndex,
                                                       IsAddSlab, SlabElevation, SlabThickness,
                                                       BackfillDepth);
                            mf.Loadcases.Add(lc);
                        }
                        outputLoadcasesInfoToLV();
                        sr.Close();
                        mf.PrintString("工况数据读取成功！");
                    }
                    else
                    {
                        mf.PrintString("文件数据格式不正确，请重新选择。");
                    }
                }
            }
            catch (Exception ex)
            {
                mf.PrintString("文件数据格式不正确：" + ex.Message);
            }
        }

        private void btnWriteLoadcaseInfo_Click(object sender, EventArgs e)         // 【按钮-工况】写入
        {
            // 打开文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = System.Windows.Forms.Application.StartupPath,
                FileName = "3-LoadCasesInfo",
                Filter = "文本文件|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);      // false 指若已存在同名文件则进行覆盖
                sw.WriteLine("编号\t开挖深度\t是否加支撑\t是否拆支撑\t拆除支撑编号\t是否浇筑顶板\t顶板标高\t顶板厚度\t回筑深度");                       // 写入表头
                for (int i = 0; i < mf.Loadcases.Count(); i++)
                {
                    sw.WriteLine((i + 1).ToString("0") + "\t" + 
                                mf.Loadcases[i].ExcavationDepth + "\t" + 
                                (mf.Loadcases[i].IsActiveSupport ? "True" : "False") + "\t" +
                                (mf.Loadcases[i].IsRemoveSupport ? "True" : "False") + "\t" +
                                mf.Loadcases[i].RemoveSupportIndex + "\t" +
                                (mf.Loadcases[i].IsAddSlab ? "True" : "False") + "\t" +
                                mf.Loadcases[i].SlabElevation + "\t" +
                                mf.Loadcases[i].SlabThickness + "\t" +
                                mf.Loadcases[i].BackfillDepth);
                }
                sw.Close();
                mf.PrintString("工况数据写入成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }

        private void btnInsertLoadcaseInfo_Click(object sender, EventArgs e)        // 【按钮-工况】插入
        {
            try
            {
                // 读取输入栏数据
                double ExcavationDepth = double.Parse(tbLoadcasesEcvDepth.Text, System.Globalization.CultureInfo.InvariantCulture);
                bool IsActiveSupport = cbLoadcasesIsActive.Text == "是";
                bool IsRemoveSupport = cbIsRemoveSupport.Text == "是";
                int RemoveSupportIndex = int.Parse(tbRemoveSupportIndex.Text);
                bool IsAddSlab = cbIsAddSlab.Text == "是";
                double SlabElevation = double.Parse(tbSlabElevation.Text, System.Globalization.CultureInfo.InvariantCulture);
                double SlabThickness = double.Parse(tbSlabThickness.Text, System.Globalization.CultureInfo.InvariantCulture);
                double BackfillDepth = double.Parse(tbBackfillDepth.Text, System.Globalization.CultureInfo.InvariantCulture);

                // 并入工况信息
                Loadcase lc = new Loadcase(ExcavationDepth, IsActiveSupport,
                                           IsRemoveSupport, RemoveSupportIndex,
                                           IsAddSlab, SlabElevation, SlabThickness,
                                           BackfillDepth);
                mf.Loadcases.Insert(lvLoadcases.SelectedIndices[0], lc);
                outputLoadcasesInfoToLV();

                // 输入栏清空
                tbLoadcasesEcvDepth.Text = "";
                mf.PrintString("工况信息插入成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString("输入数据格式错误：" + ex.Message);
            }
        }

        private void btnAppendLoadcaseInfo_Click(object sender, EventArgs e)        // 【按钮-工况】追加
        {
            try
            {
                // 读取输入栏数据
                double ExcavationDepth = double.Parse(tbLoadcasesEcvDepth.Text, System.Globalization.CultureInfo.InvariantCulture);
                bool IsActiveSupport = cbLoadcasesIsActive.Text == "是";
                bool IsRemoveSupport = cbIsRemoveSupport.Text == "是";
                int RemoveSupportIndex = int.Parse(tbRemoveSupportIndex.Text);
                bool IsAddSlab = cbIsAddSlab.Text == "是";
                double SlabElevation = double.Parse(tbSlabElevation.Text, System.Globalization.CultureInfo.InvariantCulture);
                double SlabThickness = double.Parse(tbSlabThickness.Text, System.Globalization.CultureInfo.InvariantCulture);
                double BackfillDepth = double.Parse(tbBackfillDepth.Text, System.Globalization.CultureInfo.InvariantCulture);

                // 并入工况信息
                Loadcase lc = new Loadcase(ExcavationDepth, IsActiveSupport,
                                           IsRemoveSupport, RemoveSupportIndex,
                                           IsAddSlab, SlabElevation, SlabThickness,
                                           BackfillDepth);
                mf.Loadcases.Add(lc);
                outputLoadcasesInfoToLV();

                // 输入栏清空
                tbLoadcasesEcvDepth.Text = "";
                mf.PrintString("工况信息追加成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString("输入数据格式错误：" + ex.Message);
            }
        }

        private void outputLoadcasesInfoToLV()                                      // 【方法-工况】将 List< > 输出至 ListView
        {
            lvLoadcases.Items.Clear();
            for (int i = 0; i < mf.Loadcases.Count; i++)
            {
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString("0");
                li.SubItems.Add(mf.Loadcases[i].ExcavationDepth.ToString("0.00"));
                li.SubItems.Add(mf.Loadcases[i].IsActiveSupport ? "是" : "否");
                li.SubItems.Add(mf.Loadcases[i].IsRemoveSupport ? "是" : "否");
                li.SubItems.Add(mf.Loadcases[i].RemoveSupportIndex.ToString());
                li.SubItems.Add(mf.Loadcases[i].IsAddSlab ? "是" : "否");
                li.SubItems.Add(mf.Loadcases[i].SlabElevation.ToString("0.00"));
                li.SubItems.Add(mf.Loadcases[i].SlabThickness.ToString("0.00"));
                li.SubItems.Add(mf.Loadcases[i].BackfillDepth.ToString("0.00"));
                lvLoadcases.Items.Add(li);
            }
        }

    }
}
