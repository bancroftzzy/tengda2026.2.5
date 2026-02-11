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

// 屏蔽 CS1690 警告，关于警告信息参考：https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/compiler-messages/cs1690
#pragma warning disable 1690

namespace ActiveControl.Forms
{
    public partial class Form_OtherInfo : Form
    {

        Form_Main mf;

        public Form_OtherInfo(Form_Main mf)
        {
            InitializeComponent();
            this.mf = mf;
            outputOtherInfoToLV();
        }

        private void btnReadOtherInfo_Click(object sender, EventArgs e)             // 【按钮-其他】读取
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
                    // 读文件流
                    StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    char[] deli = { '\t' };
                    string[] unit;
                    string line = sr.ReadLine();                             // 读去表头
                    if (line == "变量名\t值")
                    {
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbLengthOfECS.Text = unit[1];
                        mf.LengthOfECS = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbThickOfECS.Text = unit[1];
                        mf.ThickOfECS = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbMaxMommentOfECS1.Text = unit[1];
                        mf.MaxMommentOfECS1 = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbMaxMommentOfECS2.Text = unit[1];
                        mf.MaxMommentOfECS2 = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbMaxShearForceOfECS.Text = unit[1];
                        mf.MaxShearForceOfECS = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbLengthOfSupports.Text = unit[1];
                        mf.LengthOfSupports = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbElevOfCollar.Text = unit[1];
                        mf.ElevOfCollar = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbElevOfGround.Text = unit[1];
                        mf.ElevOfGround = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbGroundLoad.Text = unit[1];
                        mf.GroundLoad = Convert.ToDouble(unit[1]);
                        unit = sr.ReadLine().Split(deli, StringSplitOptions.RemoveEmptyEntries);
                        tbEpsDefor.Text = unit[1];
                        mf.EpsDefor = Convert.ToDouble(unit[1]);
                        sr.Close();
                        mf.PrintString("其他数据读取成功！");
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

        private void btnWriteOtherInfo_Click(object sender, EventArgs e)            // 【按钮-其他】写入
        {
            // 打开文件对话框
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = System.Windows.Forms.Application.StartupPath,
                FileName = "4-OtherParas",
                Filter = "文本文件|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);      // false 指若已存在同名文件则进行覆盖
                sw.WriteLine("变量名\t值");                       // 写入表头
                sw.WriteLine("LengthOfECS\t" + mf.LengthOfECS.ToString("0.00"));
                sw.WriteLine("ThickOfECS\t" + mf.ThickOfECS.ToString("0.00"));
                sw.WriteLine("MaxMommentOfECS1\t" + (mf.MaxMommentOfECS1 * 1e-3).ToString("0.00"));
                sw.WriteLine("MaxMommentOfECS2\t" + (mf.MaxMommentOfECS2 * 1e-3).ToString("0.00"));
                sw.WriteLine("MaxShearForceOfECS\t" + (mf.MaxShearForceOfECS * 1e-3).ToString("0.00"));
                sw.WriteLine("LengthOfSupports\t" + mf.LengthOfSupports.ToString("0.00"));
                sw.WriteLine("ElevOfCollar\t" + mf.ElevOfCollar.ToString("0.00"));
                sw.WriteLine("ElevOfGround\t" + mf.ElevOfGround.ToString("0.00"));
                sw.WriteLine("GroundLoad\t" + mf.GroundLoad.ToString("0.00"));
                sw.WriteLine("EpsDefor\t" + mf.EpsDefor.ToString("0.00e0"));
                sw.Close();
                mf.PrintString("工况数据写入成功！文件目录：\r\n" + saveFileDialog.FileName);
            }
        }

        private void btnConfirmOtherInfo_Click(object sender, EventArgs e)          // 【按钮-其他】确认
        {
            try
            {
                mf.LengthOfECS = Convert.ToDouble(tbLengthOfECS.Text); 
                mf.ThickOfECS = Convert.ToDouble(tbThickOfECS.Text);
                mf.MaxMommentOfECS1 = Convert.ToDouble(tbMaxMommentOfECS1.Text);
                mf.MaxMommentOfECS2 = Convert.ToDouble(tbMaxMommentOfECS2.Text);
                mf.MaxShearForceOfECS = Convert.ToDouble(tbMaxShearForceOfECS.Text);
                mf.LengthOfSupports = Convert.ToDouble(tbLengthOfSupports.Text);
                mf.ElevOfCollar = Convert.ToDouble(tbElevOfCollar.Text);
                mf.ElevOfGround = Convert.ToDouble(tbElevOfGround.Text);
                mf.GroundLoad = Convert.ToDouble(tbGroundLoad.Text);
                mf.EpsDefor = Convert.ToDouble(tbEpsDefor.Text);
                mf.PrintString("其他数据录入成功！");
            }
            catch (Exception ex)
            {
                mf.PrintString(ex.Message);
            }
        }

        private void outputOtherInfoToLV()                                          // 【方法-其他】将 List< > 输出至 ListView
        {
            tbLengthOfECS.Text = mf.LengthOfECS.ToString("0.00");
            tbThickOfECS.Text = mf.ThickOfECS.ToString("0.00");
            tbMaxMommentOfECS1.Text = (mf.MaxMommentOfECS1 * 1e-3).ToString("0.00");
            tbMaxMommentOfECS2.Text = (mf.MaxMommentOfECS2 * 1e-3).ToString("0.00");
            tbMaxShearForceOfECS.Text = (mf.MaxShearForceOfECS * 1e-3).ToString("0.00");
            tbLengthOfSupports.Text = mf.LengthOfSupports.ToString("0.00");
            tbElevOfCollar.Text = mf.ElevOfCollar.ToString("0.00");
            tbElevOfGround.Text = mf.ElevOfGround.ToString("0.00");
            tbGroundLoad.Text = mf.GroundLoad.ToString("0.00");
            tbEpsDefor.Text = mf.EpsDefor.ToString("0.00e0");
        }

    }
}
