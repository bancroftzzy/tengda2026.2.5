using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    public class Support
    {
        // 支撑特性
        public string Mat;             // 材料
        public double DistToGround;    // 离地面的距离
        public double HrzDist;         // 支撑水平距离
        public double Size1;           // 特征尺寸1，对于矩形混凝土支撑为支撑宽度，对于空心钢管支撑为支撑外直径
        public double Size2;           // 特征尺寸2，对于矩形混凝土支撑为支撑高度，对于空心钢管支撑为支撑壁厚
        public double MaxFC;           // 支撑极限受压承载力
        public double MaxFT;           // 支撑极限受拉承载力
        public bool AdjAble;           // 标记轴力是否可调
        public double JackStrokeMax;   // 千斤顶累计行程上限，单位m

        // 构造函数
        public Support(string mat, double disttoground, double hrzdist, double size1, double size2, double maxfc, double maxft, bool adjbale)
            : this(mat, disttoground, hrzdist, size1, size2, maxfc, maxft, adjbale, 200.0)
        {
        }

        // 构造函数：界面和输入文件中的千斤顶行程单位为mm
        public Support(string mat, double disttoground, double hrzdist, double size1, double size2, double maxfc, double maxft, bool adjbale, double jackstrokemax)
        {
            Mat = mat;
            DistToGround = disttoground;
            HrzDist = hrzdist;
            Size1 = size1;
            Size2 = size2;
            MaxFC = maxfc * 1e3;
            MaxFT = maxft * 1e3;
            AdjAble = adjbale;
            JackStrokeMax = jackstrokemax * 1e-3;
        }
    }
}
