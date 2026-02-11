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
                    // 读文件前清空支撑信息
                    lvSupports.Items.Clear();
                    mf.Supports.Clear();

                    // 读文件流
                    StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    char[] deli = { '\t' };
                    string line = sr.ReadLine();                             // 读取表头
                    if (line == "支撑编号\t材料\t离地距离\t水平间距\t尺寸1\t尺寸2\t轴压承载力\t轴拉承载力\t轴力可调整")
                    {
                        while (sr.Peek() > 0)
                        {
                            line = sr.ReadLine();
                            string[] unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            string Mat = unit[1];
                            double DistToGround = Convert.ToDouble(unit[2]);
                            double HrzDist = Convert.ToDouble(unit[3]);
                            double Size1 = Convert.ToDouble(unit[4]);
                            double Size2 = Convert.ToDouble(unit[5]);
                            double MaxFC = Convert.ToDouble(unit[6]);
                            double MaxFT = Convert.ToDouble(unit[7]);
                            bool AdjAble = unit[8] == "True";
                            Support sp = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble);
                            mf.Supports.Add(sp);
                        }
                        outputSupportsInfoToLV();
                        sr.Close();
                        mf.PrintString("支撑数据读取成功！");
                    }
                    else
                    {
                        mf.PrintString("文件数据格式不正确，请重新选择。");
                    }
                }
            }
            catch
            {
                mf.PrintString("文件数据格式不正确，请重新选择。");
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
                sw.WriteLine("支撑编号\t材料\t离地距离\t水平间距\t尺寸1\t尺寸2\t轴压承载力\t轴拉承载力\t轴力可调整");                       // 写入表头
                for (int i = 0; i < mf.Supports.Count(); i++)
                {
                    sw.WriteLine((i + 1).ToString("0") + "\t" + mf.Supports[i].Mat
                                                       + "\t" + mf.Supports[i].DistToGround.ToString("0.00")
                                                       + "\t" + mf.Supports[i].HrzDist.ToString("0.00")
                                                       + "\t" + mf.Supports[i].Size1.ToString("0.000")
                                                       + "\t" + mf.Supports[i].Size2.ToString("0.000")
                                                       + "\t" + (mf.Supports[i].MaxFC * 1e-3).ToString("0")
                                                       + "\t" + (mf.Supports[i].MaxFT * 1e-3).ToString("0")
                                                       + "\t" + mf.Supports[i].AdjAble.ToString());

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

                // 并入支撑信息
                Support s = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble);
                mf.Supports.Insert(lvSupports.SelectedIndices[0], s);
                outputSupportsInfoToLV();

                // 输入栏清空
                //cbSupportsMat.Text = "";
                tbSupportsDistToGround.Text = "";
                tbSupportsHrzDist.Text = "";
                tbSupportsSize1.Text = "";
                tbSupportsSize2.Text = "";
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

                // 并入支撑信息
                Support s = new Support(Mat, DistToGround, HrzDist, Size1, Size2, MaxFC, MaxFT, AdjAble);
                mf.Supports.Add(s);
                outputSupportsInfoToLV();

                // 输入栏清空
                //cbSupportsMat.Text = "";
                tbSupportsDistToGround.Text = "";
                tbSupportsHrzDist.Text = "";
                tbSupportsSize1.Text = "";
                tbSupportsSize2.Text = "";
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
                lvSupports.Items.Add(li);
            }
        }

    }
}
