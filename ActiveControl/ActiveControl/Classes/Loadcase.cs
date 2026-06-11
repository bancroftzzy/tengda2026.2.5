using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace ActiveControl
{
    public class Loadcase
    {
        // 工况特性
        public double ExcavationDepth;          // 开挖深度（正值表示向下开挖）
        public bool IsActiveSupport;            // 是否激活支撑
        public bool IsRemoveSupport;            // 是否拆除支撑
        public int RemoveSupportIndex;          // 拆除支撑的索引（-1表示不拆除）
        public bool IsAddSlab;                  // 是否浇筑顶板
        public double SlabElevation;            // 顶板标高
        public double SlabThickness;            // 顶板厚度
        public double BackfillDepth;            // 回筑深度（负值表示向上回填）

        // 静态属性
        public static int CurLCNo = 0;                          // 当前工况编号
        public static double CurElev = 0;                       // 当前开挖面标高
        public static int ExcCount = 0;                         // 开挖次数
        public static int ActSupCount = 0;                      // 已激活支撑数
        public static List<int> AdjSupIndex = new List<int>();  // 可调节支撑索引
        public static List<int> SlabElemIndex = new List<int>(); // 顶板单元索引
        public static bool EnableWater = false;                   // 是否考虑地下水
        public static double WaterTableElev = -9999;              // 地下水位标高
        public static bool UseNonlinearSoilSpring = false;        // 是否使用非线性土弹簧（邓肯-张模型）

        // 构造函数（保持向后兼容）
        public Loadcase(double excavationdepth, bool isactivesupport)
        {
            ExcavationDepth = excavationdepth;
            IsActiveSupport = isactivesupport;
            IsRemoveSupport = false;
            RemoveSupportIndex = -1;
            IsAddSlab = false;
            SlabElevation = 0;
            SlabThickness = 0;
            BackfillDepth = 0;
        }

        // 扩展构造函数（支持拆换撑和回筑）
        public Loadcase(double excavationdepth, bool isactivesupport, 
                       bool isremovesupport, int removesupportindex,
                       bool isaddslab, double slabelevation, double slabthickness,
                       double backfilldepth)
        {
            ExcavationDepth = excavationdepth;
            IsActiveSupport = isactivesupport;
            IsRemoveSupport = isremovesupport;
            RemoveSupportIndex = removesupportindex;
            IsAddSlab = isaddslab;
            SlabElevation = slabelevation;
            SlabThickness = slabthickness;
            BackfillDepth = backfilldepth;
        }
    }
}
