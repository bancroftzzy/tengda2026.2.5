using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Threading;


namespace ActiveControl
{
    public partial class Form_Welcome : Form
    {
        public Form_Welcome()
        {
            InitializeComponent();
        }

        public List<string> username = new List<string>();
        public List<string> password = new List<string>();
        public string UserProfilePath = Application.StartupPath + "\\ImportData\\UserProfile.xml";

        private void Form_Welcome_Load(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(190, Color.White);
            // 读取用户配置文件
            if (File.Exists(UserProfilePath))
            {
                StreamReader sr = new StreamReader(UserProfilePath, Encoding.UTF8);
                while (sr.Peek() > 0)
                {
                    username.Add(sr.ReadLine());
                    password.Add(sr.ReadLine());
                }
                sr.Close();
            }
            else
            {
                MessageBox.Show("用户配置文件缺失，程序即将关闭！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Dispose();
            }
        }

        //【事件】登录
        private void btn_Login_Click(object sender, EventArgs e)
        {
            if (tb_Username.Text == "" || tb_Password.Text == "")
            {
                MessageBox.Show("账号信息输入不完整，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int index = username.FindIndex(o => o == Encrypt(tb_Username.Text));
                if (index == -1)
                {
                    MessageBox.Show("未找到该账号，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (password[index] != Encrypt(tb_Password.Text))
                    {
                        MessageBox.Show("密码错误，请检查输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tb_Password.Text = "";
                    }
                    else
                    {
                        Form_Main fm = new Form_Main();
                        this.Hide();
                        fm.ShowDialog();
                        this.Dispose();
                    }
                }
            }
        }

        //【事件】退出
        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // 【事件】注册
        private void lb_Signup_Click(object sender, EventArgs e)
        {
            Form_Signup fmSignup = new Form_Signup(this) { Owner = this };
            fmSignup.Show();
        }

        // 【事件】修改密码
        private void lb_ChangePassword_Click(object sender, EventArgs e)
        {
            Form_ChangePassword fmForm_ChangePassword = new Form_ChangePassword(this) { Owner = this };
            fmForm_ChangePassword.Show();
        }

        // 【函数】以下四个函数为注册和修改密码label的下划线相关操作
        private void lb_Signup_MouseEnter(object sender, EventArgs e)
        { lb_Signup.Font = new Font(lb_Signup.Font.FontFamily, lb_Signup.Font.Size, FontStyle.Underline); }
        private void lb_Signup_MouseLeave(object sender, EventArgs e)
        { lb_Signup.Font = new Font(lb_Signup.Font.FontFamily, lb_Signup.Font.Size); }
        private void lb_ChangePassword_MouseEnter(object sender, EventArgs e)
        { lb_ChangePassword.Font = new Font(lb_ChangePassword.Font.FontFamily, lb_ChangePassword.Font.Size, FontStyle.Underline); }
        private void lb_ChangePassword_MouseLeave(object sender, EventArgs e)
        { lb_ChangePassword.Font = new Font(lb_ChangePassword.Font.FontFamily, lb_ChangePassword.Font.Size); }

        // 【函数】采用MD5码加密字符串
        public static string Encrypt(string str)
        {
            string res = "";                               // 记录加密后的数值
            MD5 md5 = new MD5CryptoServiceProvider();      // 创建MD5对象（MD5类为抽象类不能被实例化）
            byte[] data = Encoding.Default.GetBytes(str);  // 将字符串编码转换为一个字节序列
            byte[] data1 = md5.ComputeHash(data);          // 计算data字节数组的哈希值（加密）
            md5.Clear();                                   // 释放类资源
            for (int i = 0; i < data1.Length - 1; i++)     // 遍历加密后的数值到变量res
                res += data1[i].ToString("X");
            return res;
        }


    }
}
