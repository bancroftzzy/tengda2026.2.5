using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    /// <summary>
    /// 邓肯-张非线性弹性本构模型
    /// 用于计算土弹簧的非线性刚度
    /// </summary>
    public class DuncanChangModel
    {
        // 模型参数
        private double K;           // 模量系数（无量纲）
        private double n;           // 模量指数（无量纲）
        private double Rf;          // 破坏比（无量纲，通常0.7~0.95）
        private double c;           // 粘聚力（kPa）
        private double phi;         // 内摩擦角（度）
        private double Kb;          // 体积模量系数（无量纲）
        private double m;           // 体积模量指数（无量纲）
        
        private const double Pa = 100.0;  // 大气压力（kPa）

        /// <summary>
        /// 构造函数
        /// </summary>
        public DuncanChangModel(double k, double n, double rf, double c, double phi, double kb, double m)
        {
            this.K = k;
            this.n = n;
            this.Rf = rf;
            this.c = c;
            this.phi = phi;
            this.Kb = kb;
            this.m = m;
        }

        /// <summary>
        /// 从土层创建邓肯-张模型
        /// </summary>
        public static DuncanChangModel FromSoilLayer(SoilLayer soil)
        {
            return new DuncanChangModel(
                soil.K_Duncan,
                soil.n_Duncan,
                soil.Rf,
                soil.C,
                soil.Phi,
                soil.Kb,
                soil.m_Duncan
            );
        }

        /// <summary>
        /// 计算初始切线模量 Ei
        /// Ei = K × Pa × (σ₃/Pa)^n
        /// </summary>
        /// <param name="sigma3">围压（kPa）</param>
        /// <returns>初始切线模量（kPa）</returns>
        public double GetInitialModulus(double sigma3)
        {
            // 确保围压不为负
            if (sigma3 < 1.0)
                sigma3 = 1.0;
            
            return K * Pa * Math.Pow(sigma3 / Pa, n);
        }

        /// <summary>
        /// 计算破坏偏应力 (σ₁-σ₃)f
        /// 基于莫尔-库仑准则
        /// </summary>
        /// <param name="sigma3">围压（kPa）</param>
        /// <returns>破坏偏应力（kPa）</returns>
        public double GetFailureStress(double sigma3)
        {
            double phi_rad = phi * Math.PI / 180.0;
            double sinPhi = Math.Sin(phi_rad);
            double cosPhi = Math.Cos(phi_rad);
            
            // (σ₁-σ₃)f = (2c·cosφ + 2σ₃·sinφ) / (1 - sinφ)
            double numerator = 2.0 * c * cosPhi + 2.0 * sigma3 * sinPhi;
            double denominator = 1.0 - sinPhi;
            
            if (denominator < 0.01)
                denominator = 0.01;
            
            return numerator / denominator;
        }

        /// <summary>
        /// 计算切线模量 Et
        /// Et = Ei × [1 - Rf × (σ₁-σ₃)/(σ₁-σ₃)f]²
        /// </summary>
        /// <param name="sigma3">围压（kPa）</param>
        /// <param name="deviatorStress">当前偏应力 σ₁-σ₃（kPa）</param>
        /// <returns>切线模量（kPa）</returns>
        public double GetTangentModulus(double sigma3, double deviatorStress)
        {
            double Ei = GetInitialModulus(sigma3);
            double sigmaf = GetFailureStress(sigma3);
            
            // 计算应力比
            double ratio = deviatorStress / sigmaf;
            
            // 限制应力比不超过0.95（防止接近破坏时模量过小）
            if (ratio >= 0.95)
                ratio = 0.95;
            if (ratio < 0)
                ratio = 0;
            
            // Et = Ei × [1 - Rf × ratio]²
            double factor = 1.0 - Rf * ratio;
            if (factor < 0.1)
                factor = 0.1;  // 限制最小值，防止刚度过小
            
            return Ei * factor * factor;
        }

        /// <summary>
        /// 计算土弹簧的切线刚度
        /// k = Et × A / L
        /// </summary>
        /// <param name="sigma3">围压（kPa）</param>
        /// <param name="deviatorStress">当前偏应力（kPa）</param>
        /// <param name="area">作用面积（m²）</param>
        /// <param name="length">土弹簧长度（m）</param>
        /// <returns>土弹簧刚度（kN/m）</returns>
        public double GetSpringStiffness(double sigma3, double deviatorStress, double area, double length)
        {
            if (length < 1e-6)
                length = 1e-6;
            
            double Et = GetTangentModulus(sigma3, deviatorStress);
            return Et * area / length;
        }

        /// <summary>
        /// 计算体积模量 Bt
        /// Bt = Kb × Pa × (σ₃/Pa)^m
        /// </summary>
        /// <param name="sigma3">围压（kPa）</param>
        /// <returns>体积模量（kPa）</returns>
        public double GetBulkModulus(double sigma3)
        {
            if (sigma3 < 1.0)
                sigma3 = 1.0;
            
            return Kb * Pa * Math.Pow(sigma3 / Pa, m);
        }

        /// <summary>
        /// 根据土弹簧位移估算围压
        /// 这是一个简化方法，用于迭代计算
        /// </summary>
        /// <param name="depth">深度（m，正值）</param>
        /// <param name="gamma">土重度（kN/m³）</param>
        /// <param name="k0">静止土压力系数</param>
        /// <returns>估算的水平围压（kPa）</returns>
        public static double EstimateConfiningStress(double depth, double gamma, double k0)
        {
            // σv = γ × z（竖向应力）
            double sigmaV = gamma * depth;
            
            // σh = K0 × σv（水平应力，即围压）
            double sigmaH = k0 * sigmaV;
            
            // 确保最小值
            if (sigmaH < 10.0)
                sigmaH = 10.0;
            
            return sigmaH;
        }

        /// <summary>
        /// 根据位移估算偏应力
        /// 这是一个简化方法，用于迭代计算
        /// </summary>
        /// <param name="displacement">位移（m）</param>
        /// <param name="currentStiffness">当前刚度（kN/m）</param>
        /// <param name="area">作用面积（m²）</param>
        /// <returns>估算的偏应力（kPa）</returns>
        public static double EstimateDeviatorStress(double displacement, double currentStiffness, double area)
        {
            if (area < 1e-6)
                area = 1e-6;
            
            // F = k × δ
            double force = currentStiffness * Math.Abs(displacement);
            
            // σ = F / A
            double stress = force / area;
            
            return stress;
        }
    }
}
