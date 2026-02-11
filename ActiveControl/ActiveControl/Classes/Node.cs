using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ActiveControl
{
    //定义节点类
    public partial class Node
    {
        // 节点坐标
        public int No;
        public double Nx;
        public double Ny;

        // 节点位移
        public double Ux;
        public double Uy;
        public double Rot;

        // 构造函数
        public Node(int no, double x, double y)
        {
            No = no;
            Nx = x;
            Ny = y;
        }

        // 方法：导入节点位移
        public void LoadDisp(double ux, double uy, double rot)
        {
            Ux = ux;
            Uy = uy;
            Rot = rot;
        }

        public static List<Node> SelectNodesByLoc(List<Node> Nodes, string loc, double min, double max)                               // 【方法】根据位置选取节点
        {
            List<Node> SelectedNodes = new List<Node>();
            if (loc == "x")
            {
                foreach (Node i in Nodes)
                {
                    if (i.Nx <= max && i.Nx >= min)
                        SelectedNodes.Add(i);
                }
            }
            else if (loc == "y")
            {
                foreach (Node i in Nodes)
                {
                    if (i.Ny <= max && i.Ny >= min)
                        SelectedNodes.Add(i);
                }
            }
            else
            {
                MessageBox.Show("'SelectNodesFromLocation'函数参数输入错误！", "错误");
            }
            return SelectedNodes;
        }
        public static List<Node> SelectNodesByLoc(List<Node> Nodes, string loc, double xmin, double xmax, double ymin, double ymax)   // 【方法】根据位置选取节点（重载）
        {
            List<Node> SelectedNodes = new List<Node>();
            if (loc == "xy")
            {
                foreach (Node i in Nodes)
                {
                    if (i.Nx <= xmax && i.Nx >= xmin && i.Ny <= ymax && i.Ny >= ymin)
                        SelectedNodes.Add(i);
                }
            }
            else
            {
                MessageBox.Show("'SelectNodesFromLocation'函数参数输入错误！", "错误");
            }
            return SelectedNodes;
        }
        public static Node SelectNodeByLoc(List<Node> Nodes, double xloc, double yloc)   // 【方法】根据位置选取节点（重载）
        {
            foreach (Node i in Nodes)
            {
                if (i.Nx <= xloc + 1e-3 && i.Nx >= xloc - 1e-3 && i.Ny <= yloc + 1e-3 && i.Ny >= yloc - 1e-3)
                    return i;
            }
            return null;
        }
    }
}
