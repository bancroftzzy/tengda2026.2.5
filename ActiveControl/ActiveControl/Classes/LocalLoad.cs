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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="distToECS">距围护结构距离(m)</param>
        /// <param name="width">荷载宽度(m)</param>
        /// <param name="localGroundLoad">局部地面荷载(kPa)</param>
        public LocalLoad(double distToECS, double width, double localGroundLoad)
        {
            DistToECS = distToECS;
            Width = width;
            LocalGroundLoad = localGroundLoad;
        }
    }
}
