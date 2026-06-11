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

                // 水土计算相关参数（新增）
        public double GammaSat;         // 饱和重度 (kN/m³)
        public double GammaEff;         // 有效重度 (kN/m³)
        public string WaterSoilMode;    // 水土计算模式："自动（根据土性）"、"水土分算"、"水土合算"

        // 邓肯-张模型参数
        public double K_Duncan;      // 模量系数（无量纲）
        public double n_Duncan;      // 模量指数（无量纲）
        public double Rf;            // 破坏比（无量纲）
        public double Kb;            // 体积模量系数（无量纲）
        public double m_Duncan;      // 体积模量指数（无量纲）

        private const double Pa = 100.0;                         // 大气压 (kPa)
        private const double MinConfiningStress = 10.0;           // 与 DuncanChangModel 中围压下限一致
        private const double DuncanKCalibrationDepth = 10.0;      // 用 m 法在 10m 参考深度反标定 K

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
            
            // 根据土性估算饱和重度
            if (type == "杂填土" || type == "砂质粉土" || type == "粉砂")
            {
                // 砂性土和杂填土：比较松散，饱和后重度增加较多
                GammaSat = gamma + 2.5;
            }
            else
            {
                // 粘性土：比较密实，饱和后重度增加较少
                GammaSat = gamma + 1.5;
            }
            
            GammaEff = GammaSat - 9.81;  // 自动计算有效重度
            
            // 根据土性自动设置默认水土模式
            WaterSoilMode = GetDefaultWaterSoilMode(type);

            // 根据土类设置默认邓肯-张参数
            SetDefaultDuncanChangParameters();
        }
        
        // 新增构造函数（支持饱和重度和水土模式）
        public SoilLayer(double thick, double c, double phi, double k0, double es, double m, 
                         double gamma, string type, string waterSoilMode)
        {
            Thick = thick;
            C = c;
            Phi = phi;
            K0 = k0;
            Es = es;
            M = m;
            Gamma = gamma;
            Type = type;
            
            // 根据土性估算饱和重度（和老构造函数一样的逻辑）
            if (type == "杂填土" || type == "砂质粉土" || type == "粉砂")
            {
                GammaSat = gamma + 2.5;
            }
            else
            {
                GammaSat = gamma + 1.5;
            }
            
            GammaEff = GammaSat - 9.81;  // 自动计算有效重度
            

            WaterSoilMode = waterSoilMode;

            // 根据土类设置默认邓肯-张参数
            SetDefaultDuncanChangParameters();
        }

        
        /// <summary>
        /// 根据土层类型设置默认的邓肯-张参数
        /// 参数来源：工程经验值和三轴试验统计数据
        /// </summary>
        private void SetDefaultDuncanChangParameters()
        {
            // 根据界面中的5种土层类型精确匹配
            switch (Type)
            {
                case "杂填土":
                    // 杂填土：松散，压缩性大，强度低
                    K_Duncan = 300;      // 模量系数较小
                    n_Duncan = 0.4;      // 模量指数较小（围压效应弱）
                    Rf = 0.75;           // 破坏比较小（应力-应变曲线较陡）
                    Kb = 150;            // 体积模量系数
                    m_Duncan = 0.2;      // 体积模量指数
                    break;

                case "粉质黏土":
                    // 粉质黏土：可塑~硬塑，中等强度
                    K_Duncan = 450;      // 模量系数中等
                    n_Duncan = 0.5;      // 模量指数中等
                    Rf = 0.80;           // 破坏比中等
                    Kb = 220;            // 体积模量系数
                    m_Duncan = 0.3;      // 体积模量指数
                    break;

                case "砂质粉土":
                    // 砂质粉土：稍密~中密，渗透性较好
                    K_Duncan = 550;      // 模量系数较大
                    n_Duncan = 0.55;     // 模量指数较大（围压效应明显）
                    Rf = 0.82;           // 破坏比较大
                    Kb = 280;            // 体积模量系数
                    m_Duncan = 0.35;     // 体积模量指数
                    break;

                case "粉砂":
                    // 粉砂：中密~密实，强度较高
                    K_Duncan = 700;      // 模量系数大
                    n_Duncan = 0.6;      // 模量指数大（围压效应显著）
                    Rf = 0.85;           // 破坏比大（应力-应变曲线平缓）
                    Kb = 350;            // 体积模量系数
                    m_Duncan = 0.4;      // 体积模量指数
                    break;

                case "淤泥质粘土":
                    // 淤泥质粘土：软塑~流塑，强度很低，压缩性很大
                    K_Duncan = 180;      // 模量系数很小
                    n_Duncan = 0.35;     // 模量指数很小
                    Rf = 0.70;           // 破坏比小
                    Kb = 90;             // 体积模量系数很小
                    m_Duncan = 0.15;     // 体积模量指数很小
                    break;

                default:
                    // 默认值（中等土）- 以粉质黏土为参考
                    K_Duncan = 450;
                    n_Duncan = 0.5;
                    Rf = 0.80;
                    Kb = 220;
                    m_Duncan = 0.3;
                    break;
            }

            CalibrateDuncanKFromAvailableData();
        }

        /// <summary>
        /// 优先用 m 法等效初始刚度反标定 K；缺少 m 法参数时再用 Es 经验关系修正。
        /// </summary>
        private void CalibrateDuncanKFromAvailableData()
        {
            if (M > 0 && Gamma > 0 && K0 > 0)
            {
                double sigma3Ref = Math.Max(MinConfiningStress, K0 * Gamma * DuncanKCalibrationDepth);
                double targetEi = M * 1000.0 * DuncanKCalibrationDepth; // kPa, 由 k_m = h*m*1e6*z 和 k = Ei*1e3*h/L 反推
                K_Duncan = targetEi / (Pa * Math.Pow(sigma3Ref / Pa, n_Duncan));
                return;
            }

            // 根据 Es 值进行校正（缺少 m 法参数时的备用估算）
            if (Es > 0)
            {
                // 根据压缩模量Es估算K值
                // 经验关系：Ei ≈ (2~5) × Es，取中间值3.5
                // Ei = K × Pa × (σ3/Pa)^n
                // 假设参考围压 σ3 = 100 kPa = Pa
                // 则 Ei = K × Pa，所以 K ≈ Ei / Pa = 3.5 × Es / Pa
                double estimatedK = 3.5 * Es * 1000 / Pa;  // Es单位MPa转kPa

                // 使用加权平均：70%经验值 + 30%估算值
                K_Duncan = 0.7 * K_Duncan + 0.3 * estimatedK;
            }
        }
        /// <summary>
        /// 根据土性推荐默认水土模式
        /// </summary>
        private string GetDefaultWaterSoilMode(string soilType)
        {
            // 水土分算：杂填土、砂质粉土、粉砂
            if (soilType == "杂填土" || soilType == "砂质粉土" || soilType == "粉砂")
            {
                return "水土分算";
            }
            else
            {
                // 水土合算：粉质黏土、淤泥质粘土
                return "水土合算";
            }
        }
    }
}



