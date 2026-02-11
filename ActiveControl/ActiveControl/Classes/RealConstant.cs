using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    // 定义实常数类
    public partial class RealConstant
    {
        public int No;            // 实常数编号
        public double Area;       // 面积
        public double Iz;         // 抗弯惯矩
        public double IniStrn;    // 初应变
        // 构造函数
        public RealConstant(int no, double a, double i, double inistrn)
        {
            No = no;
            Area = a;
            Iz = i;
            IniStrn = inistrn;
        }

        //构造函数重载，初内力缺省为0
        public RealConstant(int no, double a, double i)
        {
            No = no;
            Area = a;
            Iz = i;
            IniStrn = 0;
        }
    }
}
