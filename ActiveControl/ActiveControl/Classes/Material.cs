using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    // 定义材料类，使用 partial 关键字  
    public partial class Material
    {
        public string Name;
        public double Emodulus;      // 弹性模量  
        public double Density;       // 密度  
        public double PoissonRatio;  // 泊松比（用于壳单元等）

        // 构造函数  
        public Material(string name, double E, double D)
        {
            Name = name;
            Emodulus = E;
            Density = D;
            PoissonRatio = 0.3;  // 设置默认值  
        }

        // 添加带泊松比的构造函数  
        public Material(string name, double E, double D, double nu)
        {
            Name = name;
            Emodulus = E;
            Density = D;
            PoissonRatio = nu;
        }
    }
}