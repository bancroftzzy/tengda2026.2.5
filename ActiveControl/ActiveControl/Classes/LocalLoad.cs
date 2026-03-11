using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveControl
{
    /// <summary>
    /// 局部荷载类
    /// </summary>
    public class LocalLoad
    {
        public int No;                      // 荷载编号
        public double DistToECS;            // 距围护结构距离(m)
        public double Width;                // 荷载宽度(m)
        public double LocalGroundLoad;      // 局部地面荷载(kPa)
        public double Z1;                   //近端影响深度(m)
        public double Z2;                   //远端影响深度(m)

        public LocalLoad(double distToECS, double width, double localGroundLoad)
        {
            DistToECS = distToECS;
            Width = width;
            LocalGroundLoad = localGroundLoad;
            Z1 = -999;            //将影响深度初始化为无效值
            Z2 = -999;
        }
    }
}
