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
    public partial class Form_LocalLoadsInfo : Form
    {
        Form_Main mf;

        public Form_LocalLoadsInfo(Form_Main mf)
        {
            InitializeComponent();
            this.mf = mf;
            outputLocalLoadsInfoToLV();
        }

        private void btnClearLocalLoadsInfo_Click(object sender, EventArgs e)        // 【按钮】清空
        {
            DialogResult AF = MessageBox.Show("确认清除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (AF == DialogResult.OK)
            {
                lvLocalLoads.Items.Clear();
                mf.LocalLoads.Clear();
                mf.PrintString("局部荷载数据已清空！");
            }
        }

        private void btnDeleLocalLoadsInfo_Click(object sender, EventArgs e)         // 【按钮】删除
        {
            if (lvLocalLoads.SelectedItems.Count > 0)
            {
                DialogResult AF = MessageBox.Show("确认删除？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (AF == DialogResult.OK)
                {
                    // 注意以下两行顺序不能调换
                    mf.LocalLoads.RemoveAt(lvLocalLoads.SelectedIndices[0]);
                    lvLocalLoads.Items.Remove(lvLocalLoads.SelectedItems[0]);
                    mf.PrintString("局部荷载数据删除成功！");
                }
            }
            else
                MessageBox.Show("请勾选待删除项！", "错误");
        }

        private void btnReadLocalLoadsInfo_Click(object sender, EventArgs e)         // 【按钮】读取
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
                    // 读文件前清空局部荷载信息
                    lvLocalLoads.Items.Clear();
                    mf.LocalLoads.Clear();

                    // 读文件流
                    StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    char[] deli = { '\t' };
                    string line = sr.ReadLine();                             // 读取表头
                    if (line == "荷载编号\t距围护结构距离(m)\t荷载宽度(m)\t局部地面荷载(kPa)")
                    {
                        while (sr.Peek() > 0)
                        {
                            line = sr.ReadLine();
                            string[] unit = line.Split(deli, StringSplitOptions.RemoveEmptyEntries);
                            double DistToECS = Convert.ToDouble(unit[1]);
                            double Width = Convert.ToDouble(unit[2]);
                            double LocalGroundLoad = Convert.ToDouble(unit[3]);
                            LocalLoad ll = new LocalLoad(DistToECS, Width, LocalGroundLoad);
                            mf.LocalLoads.Add(ll);
                        }
                        outputLocalLoadsInfoToLV();
                        sr.Close();
                        mf.PrintString("局部荷载数据读取成功！");
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

        private void btnWriteLocalLoadsInfo_Click(object sender, EventArgs e)        // 【按钮】写入
        {
            // 打开文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Application.StartupPath,
                FileName = "5-LocalLoadsInfo",
                Filter = "文本文件|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);
                sw.WriteLine("荷载编号\t距围护结构距离(m)\t荷载宽度(m)\t局部地面荷载(kPa)");
                for (int i = 0; i < mf.LocalLoads.Count(); i++)
                {
                    sw.WriteLine((i + 1).ToString("0") + "\t" + mf.LocalLoads[i].DistToECS.ToString("0.00")
                                                       + "\t" + mf.LocalLoads[i].Width.ToString("0.00")
                                                       + "\t" + mf.LocalLoads[i].LocalGroundLoad.ToString("0.00"));
                }
                sw.Close();
                mf.PrintString("局部荷载数据写入成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }

        private void btnInsertLocalLoadsInfo_Click(object sender, EventArgs e)       // 【按钮】插入
        {
            try
            {
                // 读取输入栏数据
                double DistToECS = double.Parse(tbDistToECS.Text);
                double Width = double.Parse(tbWidth.Text);
                double LocalGroundLoad = double.Parse(tbLocalGroundLoad.Text);

                // 并入局部荷载信息
                LocalLoad ll = new LocalLoad(DistToECS, Width, LocalGroundLoad);
                mf.LocalLoads.Insert(lvLocalLoads.SelectedIndices[0], ll);
                outputLocalLoadsInfoToLV();

                // 输入栏清空
                tbDistToECS.Text = "";
                tbWidth.Text = "";
                tbLocalGroundLoad.Text = "";
                mf.PrintString("局部荷载信息插入成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void btnAppendLocalLoadsInfo_Click(object sender, EventArgs e)       // 【按钮】追加
        {
            try
            {
                // 读取输入栏数据
                double DistToECS = double.Parse(tbDistToECS.Text);
                double Width = double.Parse(tbWidth.Text);
                double LocalGroundLoad = double.Parse(tbLocalGroundLoad.Text);

                if (DistToECS * Width * LocalGroundLoad == 0)
                    MessageBox.Show("数据不全，请检查后重新输入！", "错误");
                else
                {
                    // 并入局部荷载信息
                    LocalLoad ll = new LocalLoad(DistToECS, Width, LocalGroundLoad);
                    mf.LocalLoads.Add(ll);
                    outputLocalLoadsInfoToLV();

                    // 输入栏清空
                    tbDistToECS.Text = "";
                    tbWidth.Text = "";
                    tbLocalGroundLoad.Text = "";
                    mf.PrintString("局部荷载信息追加成功！");
                }
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void outputLocalLoadsInfoToLV()                                     // 【方法】将 List<LocalLoad> 输出至 ListView
        {
            lvLocalLoads.Items.Clear();
            for (int i = 0; i < mf.LocalLoads.Count; i++)
            {
                mf.LocalLoads[i].No = i + 1;  // 更新编号
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString("0");
                li.SubItems.Add(mf.LocalLoads[i].DistToECS.ToString("0.00"));
                li.SubItems.Add(mf.LocalLoads[i].Width.ToString("0.00"));
                li.SubItems.Add(mf.LocalLoads[i].LocalGroundLoad.ToString("0.00"));
                lvLocalLoads.Items.Add(li);
            }
        }

        private void Form_LocalLoadsInfo_Load(object sender, EventArgs e)
        {

        }
    }
}
