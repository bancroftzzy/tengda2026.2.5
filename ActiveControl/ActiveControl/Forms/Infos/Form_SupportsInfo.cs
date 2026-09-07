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
    public partial class Form_SupportsInfo : Form
    {

        Form_Main mf;

        public Form_SupportsInfo(Form_Main mf)
        {
            InitializeComponent();
            this.mf = mf;
            outputSupportsInfoToLV();
        }

        private void btnClearSupportInfo_Click(object sender, EventArgs e)          // 【按钮-支撑】清空
        {
            DialogResult AF = MessageBox.Show("确认清除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (AF == DialogResult.OK)
            {
                lvSupports.Items.Clear();
                mf.Supports.Clear();
                mf.PrintString("支撑数据已清空！");
            }
        }

        private void btnDeleSupportInfo_Click(object sender, EventArgs e)           // 【按钮-支撑】删除
        {
            if (lvSupports.SelectedItems.Count > 0)
            {
                DialogResult AF = MessageBox.Show("确认删除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (AF == DialogResult.OK)
                {
                    // 注意以下两行顺序不能调换，因为ListView中的项移除后，无法再调用 SelectedIndices
                    mf.Supports.RemoveAt(lvSupports.SelectedIndices[0]);
                    lvSupports.Items.Remove(lvSupports.SelectedItems[0]);
                    mf.PrintString("支撑数据删除成功！");
                }
            }
            else
                MessageBox.Show("请勾选待删除项！", "错误");
        }

        private void btnReadSupportInfo_Click(object sender, EventArgs e)           // 【按钮-支撑】读取
        {
            try
            {
                // 打开文件对话框
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = System.Windows.Forms.Application.StartupPath,
                    Filter = "文本文件|*.txt",
                    RestoreDirectory = true,
                    FilterIndex = 1
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    char[] deli = { '\t' };
                    const string oldHeader =
                        "支撑编号\t材料\t离地距离\t水平间距\t尺寸1\t尺寸2\t轴压承载力\t轴拉承载力\t轴力可调整";
                    const string newHeader =
                        oldHeader + "\t千斤顶行程上限(mm)";
                    List<Support> importedSupports = new List<Support>();

                    using (StreamReader sr = new StreamReader(
                        openFileDialog.FileName,
                        Encoding.UTF8))
                    {
                        string line = sr.ReadLine();                         // 读取表头
                        bool isOldFormat = line == oldHeader;
                        bool isNewFormat = line == newHeader;
                        if (!isOldFormat && !isNewFormat)
                            throw new FormatException("支撑数据表头与新版或旧版格式都不匹配。");

                        int lineNumber = 1;
                        while ((line = sr.ReadLine()) != null)
                        {
                            lineNumber++;
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            // 旧数据中可能存在连续制表符，读取时忽略由此产生的空列。
                            string[] unit = line
                                .Split(deli, StringSplitOptions.RemoveEmptyEntries)
                                .Select(value => value.Trim())
                                .ToArray();

                            if (unit.Length < 9 || unit.Length > 10)
                            {
                                throw new FormatException(
                                    $"第{lineNumber}行应为9列或10列，实际读取到{unit.Length}列。");
                            }

                            string Mat = unit[1];
                            double DistToGround = Convert.ToDouble(unit[2]);
                            double HrzDist = Convert.ToDouble(unit[3]);
                            double Size1 = Convert.ToDouble(unit[4]);
                            double Size2 = Convert.ToDouble(unit[5]);
                            double MaxFC = Convert.ToDouble(unit[6]);
                            double MaxFT = Convert.ToDouble(unit[7]);
                            bool AdjAble;
                            if (!bool.TryParse(unit[8], out AdjAble))
                                throw new FormatException($"第{lineNumber}行“轴力可调整”必须为True或False。");

                            double JackStrokeMax = 200.0;
                            if (unit.Length >= 10 && !string.IsNullOrWhiteSpace(unit[9]))
                                JackStrokeMax = Convert.ToDouble(unit[9]);

                            Support sp = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble, JackStrokeMax);
                            importedSupports.Add(sp);
                        }
                    }

                    // 只有在文件完整解析成功后，才替换当前支撑数据。
                    lvSupports.Items.Clear();
                    mf.Supports.Clear();
                    mf.Supports.AddRange(importedSupports);
                    outputSupportsInfoToLV();
                    mf.PrintString("支撑数据读取成功！");
                }
            }
            catch (Exception ex)
            {
                mf.PrintString("文件数据格式不正确，请重新选择。\r\n" + ex.Message);
            }
        }

        private void btnWriteSupportInfo_Click(object sender, EventArgs e)          // 【按钮-支撑】写入
        {
            // 打开文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Application.StartupPath,
                FileName = "2-SupportsInfo",
                Filter = "文本文件|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);      // false 指若已存在同名文件则进行覆盖
                sw.WriteLine("支撑编号\t材料\t离地距离\t水平间距\t尺寸1\t尺寸2\t轴压承载力\t轴拉承载力\t轴力可调整\t千斤顶行程上限(mm)");                       // 写入表头
                for (int i = 0; i < mf.Supports.Count(); i++)
                {
                    sw.WriteLine((i + 1).ToString("0") + "\t" + mf.Supports[i].Mat
                                                       + "\t" + mf.Supports[i].DistToGround.ToString("0.00")
                                                       + "\t" + mf.Supports[i].HrzDist.ToString("0.00")
                                                       + "\t" + mf.Supports[i].Size1.ToString("0.000")
                                                       + "\t" + mf.Supports[i].Size2.ToString("0.000")
                                                       + "\t" + (mf.Supports[i].MaxFC * 1e-3).ToString("0")
                                                       + "\t" + (mf.Supports[i].MaxFT * 1e-3).ToString("0")
                                                       + "\t" + mf.Supports[i].AdjAble.ToString()
                                                       + "\t" + (mf.Supports[i].JackStrokeMax * 1e3).ToString("0.00"));

                }
                sw.Close();
                mf.PrintString("支撑数据写入成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }

        private void btnInsertSupportInfo_Click(object sender, EventArgs e)         // 【按钮-支撑】插入
        {
            try
            {
                // 读取输入栏数据
                String Mat = cbSupportsMat.Text;
                double DistToGround = Convert.ToDouble(tbSupportsDistToGround.Text);
                double HrzDist = Convert.ToDouble(tbSupportsHrzDist.Text);
                double Size1 = Convert.ToDouble(tbSupportsSize1.Text);
                double Size2 = Convert.ToDouble(tbSupportsSize2.Text);
                double MaxFC = Convert.ToDouble(tbSupportsMaxFC.Text);
                double MaxFT = Convert.ToDouble(tbSupportsMaxFT.Text);
                bool AdjAble = cbSupportsAdjAble.Text == "是";
                double JackStrokeMax = string.IsNullOrWhiteSpace(tbSupportsJackStrokeMax.Text)
                    ? 200.0
                    : Convert.ToDouble(tbSupportsJackStrokeMax.Text);
                if (JackStrokeMax < 0)
                    throw new ArgumentException("千斤顶行程上限不能小于0。");

                // 并入支撑信息
                Support s = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble, JackStrokeMax);
                mf.Supports.Insert(lvSupports.SelectedIndices[0], s);
                outputSupportsInfoToLV();

                // 输入栏清空
                //cbSupportsMat.Text = "";
                tbSupportsDistToGround.Text = "";
                tbSupportsHrzDist.Text = "";
                tbSupportsSize1.Text = "";
                tbSupportsSize2.Text = "";
                tbSupportsJackStrokeMax.Text = "200";
                mf.PrintString("支撑信息插入成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void btnAppendSopportInfo_Click(object sender, EventArgs e)         // 【按钮-支撑】追加
        {
            try
            {
                // 读取输入栏数据
                String Mat = cbSupportsMat.Text;
                double DistToGround = Convert.ToDouble(tbSupportsDistToGround.Text);
                double HrzDist = Convert.ToDouble(tbSupportsHrzDist.Text);
                double Size1 = Convert.ToDouble(tbSupportsSize1.Text);
                double Size2 = Convert.ToDouble(tbSupportsSize2.Text);
                double MaxFC = Convert.ToDouble(tbSupportsMaxFC.Text);
                double MaxFT = Convert.ToDouble(tbSupportsMaxFT.Text);
                bool AdjAble = cbSupportsAdjAble.Text == "是";
                double JackStrokeMax = string.IsNullOrWhiteSpace(tbSupportsJackStrokeMax.Text)
                    ? 200.0
                    : Convert.ToDouble(tbSupportsJackStrokeMax.Text);
                if (JackStrokeMax < 0)
                    throw new ArgumentException("千斤顶行程上限不能小于0。");

                // 并入支撑信息
                Support s = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble, JackStrokeMax);
                mf.Supports.Add(s);
                outputSupportsInfoToLV();

                // 输入栏清空
                //cbSupportsMat.Text = "";
                tbSupportsDistToGround.Text = "";
                tbSupportsHrzDist.Text = "";
                tbSupportsSize1.Text = "";
                tbSupportsSize2.Text = "";
                tbSupportsJackStrokeMax.Text = "200";
                mf.PrintString("支撑信息追加成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void outputSupportsInfoToLV()                                       // 【方法-支撑】将 List< > 输出至 ListView
        {
            lvSupports.Items.Clear();
            for (int i = 0; i < mf.Supports.Count; i++)
            {
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString("0");
                li.SubItems.Add(mf.Supports[i].Mat);
                li.SubItems.Add(mf.Supports[i].DistToGround.ToString("0.00"));
                li.SubItems.Add(mf.Supports[i].HrzDist.ToString("0.00"));
                li.SubItems.Add(mf.Supports[i].Size1.ToString("0.000"));
                li.SubItems.Add(mf.Supports[i].Size2.ToString("0.000"));
                li.SubItems.Add((mf.Supports[i].MaxFC * 1e-3).ToString("0"));
                li.SubItems.Add((mf.Supports[i].MaxFT * 1e-3).ToString("0"));
                li.SubItems.Add(mf.Supports[i].AdjAble.ToString());
                li.SubItems.Add((mf.Supports[i].JackStrokeMax * 1e3).ToString("0.00"));
                lvSupports.Items.Add(li);
            }
        }

    }
}
