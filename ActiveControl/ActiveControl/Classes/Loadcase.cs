using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    public class Loadcase
    {
        // 工况特性
        public double ExcavationDepth;
        public bool IsActiveSupport;

        // 静态属性
        public static int CurLCNo = 0;                          // 当前工况编号
        public static double CurElev = 0;                       // 当前开挖面标高
        public static int ExcCount = 0;                         // 开挖次数
        public static int ActSupCount = 0;                      // 已激活支撑数
        public static List<int> AdjSupIndex = new List<int>();  // 可调节支撑索引

        //构造函数
        public Loadcase(double excavationdepth, bool isactivesupport)
        {
            ExcavationDepth = excavationdepth;
            IsActiveSupport = isactivesupport;
        }
    }
}
