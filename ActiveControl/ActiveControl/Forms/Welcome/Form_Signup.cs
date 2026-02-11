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
    public partial class Form_Signup : Form
    {
        readonly Form_Welcome fw;

        public Form_Signup(Form_Welcome fw)
        {
            InitializeComponent();
            this.fw = fw;
        }

        //【事件】注册
        private void btn_Register_Click(object sender, EventArgs e)
        {
            string RegisterCode = "tdyjy";                           // 注册码
            if (tb_RegNum.Text == RegisterCode)                      // 检验注册码是否正确
            {
                if(tb_RegUn.Text != "" && tb_RegPw.Text != "")
                {
                    string newUsername = Form_Welcome.Encrypt(tb_RegUn.Text);
                    if(!fw.username.Contains(newUsername))           // 检验账户名是否与已有账户重复
                    {
                        if (tb_RegPw.Text == tb_RegPwConfirm.Text)   // 检验两行密码是否一致
                        {

                            string newPassword = Form_Welcome.Encrypt(tb_RegPw.Text);
                            fw.username.Add(newUsername);
                            fw.password.Add(newPassword);

                            StreamWriter sw = new StreamWriter(fw.UserProfilePath, true, Encoding.UTF8);
                            sw.WriteLine(newUsername);
                            sw.WriteLine(newPassword);
                            sw.Close();

                            MessageBox.Show("注册成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Dispose();
                        }
                        else
                        {
                            MessageBox.Show("两次密码输入不一致，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tb_RegPwConfirm.Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("该账户已存在，请修改账户名！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("账号信息输入不完整，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("注册号错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //【事件】退出
        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
