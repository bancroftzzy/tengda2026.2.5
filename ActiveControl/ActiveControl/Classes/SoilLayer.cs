using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    public class SoilLayer
    {
        // 土层特性
        public double Thick;
        public double C;
        public double Phi;
        public double K0;
        public double Es;
        public double M;
        public double Gamma;
        public string Type;

        //构造函数
        public SoilLayer(double thick, double c, double phi, double k0, double es, double m, double gamma, string type)
        {
            Thick = thick;
            C = c;
            Phi = phi;
            K0 = k0;
            Es = es;
            M = m;
            Gamma = gamma;
            Type = type;
        }
    }
}
