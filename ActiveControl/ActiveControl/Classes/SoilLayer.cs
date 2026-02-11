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

        // 邓肯-张模型参数
        public double K_Duncan;      // 模量系数（无量纲）
        public double n_Duncan;      // 模量指数（无量纲）
        public double Rf;            // 破坏比（无量纲）
        public double Kb;            // 体积模量系数（无量纲）
        public double m_Duncan;      // 体积模量指数（无量纲）
        
        // 模型选择标志
        public bool UseDuncanChang;  // true=邓肯张模型, false=线性模型

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
            
            // 默认使用线性模型
            UseDuncanChang = false;
            
            // 根据土类设置默认邓肯-张参数
            SetDefaultDuncanChangParameters();
        }
        
        /// <summary>
        /// 根据土层类型设置默认的邓肯-张参数
        /// </summary>
        private void SetDefaultDuncanChangParameters()
        {
            // 根据Es估算K值（经验公式）
            double Pa = 100.0; // 大气压 kPa
            
            if (Type.Contains("软") || Type.Contains("流"))
            {
                // 软塑粘土
                K_Duncan = 200;
                n_Duncan = 0.4;
                Rf = 0.75;
                Kb = 100;
                m_Duncan = 0.2;
            }
            else if (Type.Contains("可塑") || Type.Contains("中"))
            {
                // 可塑粘土
                K_Duncan = 400;
                n_Duncan = 0.5;
                Rf = 0.80;
                Kb = 200;
                m_Duncan = 0.3;
            }
            else if (Type.Contains("硬") || Type.Contains("坚"))
            {
                // 硬塑粘土
                K_Duncan = 600;
                n_Duncan = 0.6;
                Rf = 0.85;
                Kb = 300;
                m_Duncan = 0.4;
            }
            else if (Type.Contains("粉"))
            {
                // 粉土
                K_Duncan = 500;
                n_Duncan = 0.5;
                Rf = 0.80;
                Kb = 250;
                m_Duncan = 0.3;
            }
            else if (Type.Contains("砂"))
            {
                // 砂土
                K_Duncan = 800;
                n_Duncan = 0.6;
                Rf = 0.85;
                Kb = 400;
                m_Duncan = 0.4;
            }
            else
            {
                // 默认值（中等土）
                K_Duncan = 400;
                n_Duncan = 0.5;
                Rf = 0.80;
                Kb = 200;
                m_Duncan = 0.3;
            }
            
            // 如果有Es值，可以用来校正K值
            if (Es > 0)
            {
                // K ≈ Es / Pa × 经验系数
                double estimatedK = Es * 1000 / Pa * 4.0; // Es单位MPa转kPa
                // 使用估算值和默认值的平均
                K_Duncan = (K_Duncan + estimatedK) / 2.0;
            }
        }
    }
}
