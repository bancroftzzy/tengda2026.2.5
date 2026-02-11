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

namespace ActiveControl
{
    public partial class Form_ChangePassword : Form
    {
        readonly Form_Welcome fw;

        public Form_ChangePassword(Form_Welcome fw)
        {
            InitializeComponent();
            this.fw = fw;
        }

        // 【事件】确认修改
        private void btn_CpConfirm_Click(object sender, EventArgs e)
        {
            if(tb_CpUn.Text == "" || tb_CpOldpw.Text == "" || tb_CpNewPw.Text == "" || tb_CpNewPwConfirm.Text == "")
            {
                MessageBox.Show("账号信息输入不完整，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string OldUsername = Form_Welcome.Encrypt(tb_CpUn.Text);
                int index = fw.username.FindIndex(o => o == OldUsername);
                if(index == -1)                                 // 验证用户名是否存在
                {
                    MessageBox.Show("未找到该用户，请检查用户名！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string OldPassword = Form_Welcome.Encrypt(tb_CpOldpw.Text);
                    if (fw.password[index] == OldPassword)      // 验证旧密码是否正确
                    {
                        if (tb_CpNewPw.Text == tb_CpNewPwConfirm.Text)    // 验证两次新密码是否一致
                        {
                            // 修改内存中的password数组
                            string NewPassword = Form_Welcome.Encrypt(tb_CpNewPw.Text);
                            fw.password[index] = NewPassword;

                            // 将修改后的username和password重新写入文本文件。未找到直接向指定行直接写入的方法
                            StreamWriter sw = new StreamWriter(fw.UserProfilePath, false, Encoding.UTF8);    // 注意要覆写
                            for (int i = 0; i < fw.username.Count; i++)
                            {
                                sw.WriteLine(fw.username[i]);
                                sw.WriteLine(fw.password[i]);
                            }
                            sw.Close();
                        }
                        else
                        {
                            MessageBox.Show("两次密码输入不一致，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("密码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        // 【事件】退出
        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
