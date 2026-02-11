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
    // 定义单元类  
    public class Element
    {
        public int No;
        public Node Left;                   // 左节点  
        public Node Right;                  // 右节点  
        // 壳单元额外需要的节点  
        public Node Third;                  // 第三个节点  
        public Node Fourth;                 // 第四个节点  
        public Material Material;           // 单元材料  
        public string ElementType;          // 单元类型  
        public RealConstant RealConstant;   // 单元实常数  
        public Matrix<double> Tr;           // 坐标转换矩阵  
        public bool isAlive;                // 标记单元生命  

        // 杆端力 - 两节点单元  
        public double iFx;
        public double iFy;
        public double iMom;
        public double jFx;
        public double jFy;
        public double jMom;

        // 等效节点荷载 - 两节点单元  
        public double iFx0 = 0;
        public double iFy0 = 0;
        public double iMom0 = 0;
        public double jFx0 = 0;
        public double jFy0 = 0;
        public double jMom0 = 0;

        // 壳单元节点力和等效节点荷载 - 声明所有需要的变量  
        // 节点1(i)的力和等效荷载  
        public double iFz = 0;
        public double iMomx = 0;
        public double iMomy = 0;
        public double iFz0 = 0;
        public double iMomx0 = 0;
        public double iMomy0 = 0;

        // 节点2(j)的力和等效荷载  
        public double jFz = 0;
        public double jMomx = 0;
        public double jMomy = 0;
        public double jFz0 = 0;
        public double jMomx0 = 0;
        public double jMomy0 = 0;

        // 节点3(k)的力和等效荷载  
        public double kFx = 0;
        public double kFy = 0;
        public double kFz = 0;
        public double kMomx = 0;
        public double kMomy = 0;
        public double kFx0 = 0;
        public double kFy0 = 0;
        public double kFz0 = 0;
        public double kMomx0 = 0;
        public double kMomy0 = 0;

        // 节点4(l)的力和等效荷载  
        public double lFx = 0;
        public double lFy = 0;
        public double lFz = 0;
        public double lMomx = 0;
        public double lMomy = 0;
        public double lFx0 = 0;
        public double lFy0 = 0;
        public double lFz0 = 0;
        public double lMomx0 = 0;
        public double lMomy0 = 0;

        // 构造函数 - 两节点单元  
        public Element(int no, Node left, Node right, Material mat, string elementtype, RealConstant real)
        {
            No = no;
            Left = left;
            Right = right;
            Material = mat;
            ElementType = elementtype;
            RealConstant = real;

            // 计算坐标转换矩阵  
            double L = Math.Sqrt(Math.Pow(Left.Nx - Right.Nx, 2) + Math.Pow(Left.Ny - Right.Ny, 2));
            double s = (Right.Ny - Left.Ny) / L;
            double c = (Right.Nx - Left.Nx) / L;
            double[,] tempTr = {
                    {  c, s, 0,  0, 0, 0 },
                    { -s, c, 0,  0, 0, 0 },
                    {  0, 0, 1,  0, 0, 0 },
                    {  0, 0, 0,  c, s, 0 },
                    {  0, 0, 0, -s, c, 0 },
                    {  0, 0, 0,  0, 0, 1 } };
            Tr = Matrix<double>.Build.DenseOfArray(tempTr);

            isAlive = true;
        }

        // 新增构造函数 - 四节点壳单元  
        public Element(int no, Node n1, Node n2, Node n3, Node n4, Material mat, string elementtype, RealConstant real)
        {
            No = no;
            Left = n1;    // 对应于壳的第一个节点  
            Right = n2;   // 对应于壳的第二个节点  
            Third = n3;   // 对应于壳的第三个节点  
            Fourth = n4;  // 对应于壳的第四个节点  
            Material = mat;
            ElementType = elementtype;
            RealConstant = real;

            // 壳单元的坐标转换矩阵在需要时再计算  
            isAlive = true;
        }

        public Matrix<double> Ke()               // 【方法】计算单元刚度矩阵  
        {
            if (!isAlive)
                return ElementType == "Shell" ?
                       Matrix<double>.Build.Dense(20, 20, 0.0) :
                       Matrix<double>.Build.Dense(6, 6, 0.0);

            if (ElementType == "Beam")
            {
                return KeBeam();
            }
            else if (ElementType == "Link")
            {
                return KeLink();
            }
            else if (ElementType == "Shell")
            {
                return KeShell();
            }
            else
            {
                MessageBox.Show("单元类型输入错误，请检查！", "错误");
                return null;
            }
        }

        private Matrix<double> KeBeam()  // 梁单元刚度矩阵  
        {
            double E = Material.Emodulus;
            double A = RealConstant.Area;
            double I = RealConstant.Iz;
            double L = Math.Sqrt(Math.Pow(Left.Nx - Right.Nx, 2) + Math.Pow(Left.Ny - Right.Ny, 2));
            double EA = E * A;
            double EI = E * I;
            if (L < 1e-10)
                MessageBox.Show("单元两端节点重合，请检查输入！", "错误");

            double[,] tempKe = {
                {  EA/L,            0,         0, -EA/L,            0,         0 },
                {     0,  12*EI/L/L/L,  6*EI/L/L,     0, -12*EI/L/L/L,  6*EI/L/L },
                {     0,     6*EI/L/L,    4*EI/L,     0,    -6*EI/L/L,    2*EI/L },
                { -EA/L,            0,         0,  EA/L,            0,         0 },
                {     0, -12*EI/L/L/L, -6*EI/L/L,     0,  12*EI/L/L/L, -6*EI/L/L },
                {     0,     6*EI/L/L,    2*EI/L,     0,    -6*EI/L/L,    4*EI/L } };
            return Matrix<double>.Build.DenseOfArray(tempKe);
        }

        private Matrix<double> KeLink()  // 连杆单元刚度矩阵  
        {
            double E = Material.Emodulus;
            double A = RealConstant.Area;
            double L = Math.Sqrt(Math.Pow(Left.Nx - Right.Nx, 2) + Math.Pow(Left.Ny - Right.Ny, 2));
            double EA = E * A;
            if (L < 1e-10)
                MessageBox.Show("单元两端节点重合，请检查输入！", "错误");

            double[,] tempKe = {
                {  EA/L, 0, 0, -EA/L, 0, 0 },
                {     0, 0, 0,     0, 0, 0 },
                {     0, 0, 0,     0, 0, 0 },
                { -EA/L, 0, 0,  EA/L, 0, 0 },
                {     0, 0, 0,     0, 0, 0 },
                {     0, 0, 0,     0, 0, 0 } };
            return Matrix<double>.Build.DenseOfArray(tempKe);
        }

        private Matrix<double> KeShell()  // 壳单元刚度矩阵  
        {
            double E = Material.Emodulus;
            double nu = Material.PoissonRatio;
            double t = RealConstant.Thickness;
            double G = E / (2 * (1 + nu));

            // 创建四节点壳单元20×20刚度矩阵  
            Matrix<double> K = Matrix<double>.Build.Dense(20, 20);

            // 收集四个节点的坐标信息  
            double[,] nodeCoords = {
                { Left.Nx, Left.Ny, 0 },
                { Right.Nx, Right.Ny, 0 },
                { Third.Nx, Third.Ny, 0 },
                { Fourth.Nx, Fourth.Ny, 0 }
            };

            // 高斯积分点和权重  
            double[] gaussPoints = { -0.57735026919, 0.57735026919 };
            double[] gaussWeights = { 1.0, 1.0 };

            // 计算材料矩阵  
            // 膜部分  
            double[,] Dm = {
                { E*t/(1-nu*nu), E*t*nu/(1-nu*nu), 0 },
                { E*t*nu/(1-nu*nu), E*t/(1-nu*nu), 0 },
                { 0, 0, E*t*(1-nu)/(2*(1-nu*nu)) }
            };
            Matrix<double> Dm_matrix = Matrix<double>.Build.DenseOfArray(Dm);

            // 弯曲部分  
            double[,] Db = {
                { E*t*t*t/(12*(1-nu*nu)), E*t*t*t*nu/(12*(1-nu*nu)), 0 },
                { E*t*t*t*nu/(12*(1-nu*nu)), E*t*t*t/(12*(1-nu*nu)), 0 },
                { 0, 0, E*t*t*t*(1-nu)/(24*(1-nu*nu)) }
            };
            Matrix<double> Db_matrix = Matrix<double>.Build.DenseOfArray(Db);

            // 剪切部分  
            double kappa = 5.0 / 6.0;  // 剪切修正系数  
            double[,] Ds = {
                { kappa*G*t, 0 },
                { 0, kappa*G*t }
            };
            Matrix<double> Ds_matrix = Matrix<double>.Build.DenseOfArray(Ds);

            // 通过高斯积分计算刚度矩阵  
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    double xi = gaussPoints[i];
                    double eta = gaussPoints[j];
                    double weight = gaussWeights[i] * gaussWeights[j];

                    // 计算形函数值  
                    double[] N = {
                        0.25 * (1 - xi) * (1 - eta),
                        0.25 * (1 + xi) * (1 - eta),
                        0.25 * (1 + xi) * (1 + eta),
                        0.25 * (1 - xi) * (1 + eta)
                    };

                    // 计算形函数导数  
                    double[,] dNdxi = {
                        { -0.25 * (1 - eta), -0.25 * (1 - xi) },
                        {  0.25 * (1 - eta), -0.25 * (1 + xi) },
                        {  0.25 * (1 + eta),  0.25 * (1 + xi) },
                        { -0.25 * (1 + eta),  0.25 * (1 - xi) }
                    };

                    // 计算雅可比矩阵  
                    double[,] J = new double[2, 2];
                    for (int k = 0; k < 4; k++)
                    {
                        J[0, 0] += dNdxi[k, 0] * nodeCoords[k, 0];
                        J[0, 1] += dNdxi[k, 0] * nodeCoords[k, 1];
                        J[1, 0] += dNdxi[k, 1] * nodeCoords[k, 0];
                        J[1, 1] += dNdxi[k, 1] * nodeCoords[k, 1];
                    }

                    // 计算雅可比行列式和逆矩阵  
                    double detJ = J[0, 0] * J[1, 1] - J[0, 1] * J[1, 0];
                    double[,] invJ = {
                        {  J[1, 1] / detJ, -J[0, 1] / detJ },
                        { -J[1, 0] / detJ,  J[0, 0] / detJ }
                    };

                    // 计算形函数在全局坐标系下的导数  
                    double[,] dNdx = new double[4, 2];
                    for (int k = 0; k < 4; k++)
                    {
                        dNdx[k, 0] = dNdxi[k, 0] * invJ[0, 0] + dNdxi[k, 1] * invJ[1, 0];
                        dNdx[k, 1] = dNdxi[k, 0] * invJ[0, 1] + dNdxi[k, 1] * invJ[1, 1];
                    }

                    // 计算B矩阵 - 膜部分  
                    Matrix<double> Bm = Matrix<double>.Build.Dense(3, 20);
                    for (int k = 0; k < 4; k++)
                    {
                        Bm[0, k * 5 + 0] = dNdx[k, 0];  // ε_xx = du/dx  
                        Bm[1, k * 5 + 1] = dNdx[k, 1];  // ε_yy = dv/dy  
                        Bm[2, k * 5 + 0] = dNdx[k, 1];  // γ_xy = du/dy + dv/dx  
                        Bm[2, k * 5 + 1] = dNdx[k, 0];
                    }

                    // 计算B矩阵 - 弯曲部分  
                    Matrix<double> Bb = Matrix<double>.Build.Dense(3, 20);
                    for (int k = 0; k < 4; k++)
                    {
                        Bb[0, k * 5 + 3] = dNdx[k, 0];  // κ_xx = dθ_x/dx  
                        Bb[1, k * 5 + 4] = dNdx[k, 1];  // κ_yy = dθ_y/dy  
                        Bb[2, k * 5 + 3] = dNdx[k, 1];  // κ_xy = dθ_x/dy + dθ_y/dx  
                        Bb[2, k * 5 + 4] = dNdx[k, 0];
                    }

                    // 计算B矩阵 - 剪切部分  
                    Matrix<double> Bs = Matrix<double>.Build.Dense(2, 20);
                    for (int k = 0; k < 4; k++)
                    {
                        Bs[0, k * 5 + 2] = dNdx[k, 0];  // γ_xz = dw/dx + θ_x  
                        Bs[0, k * 5 + 3] = N[k];
                        Bs[1, k * 5 + 2] = dNdx[k, 1];  // γ_yz = dw/dy + θ_y  
                        Bs[1, k * 5 + 4] = N[k];
                    }

                    // 计算分部刚度矩阵并累加  
                    Matrix<double> Km = Bm.Transpose() * Dm_matrix * Bm * weight * detJ;
                    Matrix<double> Kb = Bb.Transpose() * Db_matrix * Bb * weight * detJ;
                    Matrix<double> Ks = Bs.Transpose() * Ds_matrix * Bs * weight * detJ;

                    K = K + Km + Kb + Ks;
                }
            }

            return K;
        }

        public void getNodalForce()              // 【方法】计算杆端力  
        {
            if (ElementType == "Beam" || ElementType == "Link")
            {
                // 单元变形列向量  
                Vector<double> gDisp = Vector<double>.Build.Dense(6);
                gDisp[0] = Left.Ux;
                gDisp[1] = Left.Uy;
                gDisp[2] = Left.Rot;
                gDisp[3] = Right.Ux;
                gDisp[4] = Right.Uy;
                gDisp[5] = Right.Rot;

                // 计算杆端力，注意考虑初应变和等效节点力  
                Vector<double> nodalForce = Ke() * Tr * gDisp;
                iFx = nodalForce[0] - iFx0 + RealConstant.IniStrn * Material.Emodulus * RealConstant.Area;
                iFy = nodalForce[1] - iFy0;
                iMom = nodalForce[2] - iMom0;
                jFx = nodalForce[3] - jFx0 - RealConstant.IniStrn * Material.Emodulus * RealConstant.Area;
                jFy = nodalForce[4] - jFy0;
                jMom = nodalForce[5] - jMom0;
            }
            else if (ElementType == "Shell")
            {
                // 对于壳单元，获取20个自由度的位移向量  
                Vector<double> gDisp = Vector<double>.Build.Dense(20);

                // 节点1 (Left)  
                gDisp[0] = Left.Ux;
                gDisp[1] = Left.Uy;
                gDisp[2] = Left.Uz;
                gDisp[3] = Left.Rotx;
                gDisp[4] = Left.Roty;

                // 节点2 (Right)  
                gDisp[5] = Right.Ux;
                gDisp[6] = Right.Uy;
                gDisp[7] = Right.Uz;
                gDisp[8] = Right.Rotx;
                gDisp[9] = Right.Roty;

                // 节点3 (Third)  
                gDisp[10] = Third.Ux;
                gDisp[11] = Third.Uy;
                gDisp[12] = Third.Uz;
                gDisp[13] = Third.Rotx;
                gDisp[14] = Third.Roty;

                // 节点4 (Fourth)  
                gDisp[15] = Fourth.Ux;
                gDisp[16] = Fourth.Uy;
                gDisp[17] = Fourth.Uz;
                gDisp[18] = Fourth.Rotx;
                gDisp[19] = Fourth.Roty;

                // 计算壳单元节点力  
                Vector<double> nodalForce = Ke() * gDisp;

                // 节点1力  
                iFx = nodalForce[0] - iFx0;
                iFy = nodalForce[1] - iFy0;
                iFz = nodalForce[2] - iFz0;
                iMomx = nodalForce[3] - iMomx0;
                iMomy = nodalForce[4] - iMomy0;

                // 节点2力  
                jFx = nodalForce[5] - jFx0;
                jFy = nodalForce[6] - jFy0;
                jFz = nodalForce[7] - jFz0;
                jMomx = nodalForce[8] - jMomx0;
                jMomy = nodalForce[9] - jMomy0;

                // 节点3力  
                kFx = nodalForce[10] - kFx0;
                kFy = nodalForce[11] - kFy0;
                kFz = nodalForce[12] - kFz0;
                kMomx = nodalForce[13] - kMomx0;
                kMomy = nodalForce[14] - kMomy0;

                // 节点4力  
                lFx = nodalForce[15] - lFx0;
                lFy = nodalForce[16] - lFy0;
                lFz = nodalForce[17] - lFz0;
                lMomx = nodalForce[18] - lMomx0;
                lMomy = nodalForce[19] - lMomy0;
            }
        }

        public static List<Element> SelectElementsFromLocation(List<Element> Elements, char loc, double min, double max)    // 【方法】根据位置选取单元  
        {
            List<Element> SelectedElements = new List<Element>();
            if (loc == 'x')
            {
                foreach (Element i in Elements)
                {
                    if (i.ElementType == "Shell")
                    {
                        // 对于壳单元，取四个节点的平均x坐标  
                        double x0 = (i.Left.Nx + i.Right.Nx + i.Third.Nx + i.Fourth.Nx) / 4;
                        if (x0 <= max && x0 >= min)
                            SelectedElements.Add(i);
                    }
                    else
                    {
                        // 对于梁和连杆，取两节点的平均  
                        double x0 = (i.Left.Nx + i.Right.Nx) / 2;
                        if (x0 <= max && x0 >= min)
                            SelectedElements.Add(i);
                    }
                }
            }
            else if (loc == 'y')
            {
                foreach (Element i in Elements)
                {
                    if (i.ElementType == "Shell")
                    {
                        // 对于壳单元，取四个节点的平均y坐标  
                        double y0 = (i.Left.Ny + i.Right.Ny + i.Third.Ny + i.Fourth.Ny) / 4;
                        if (y0 <= max && y0 >= min)
                            SelectedElements.Add(i);
                    }
                    else
                    {
                        // 对于梁和连杆，取两节点的平均  
                        double y0 = (i.Left.Ny + i.Right.Ny) / 2;
                        if (y0 <= max && y0 >= min)
                            SelectedElements.Add(i);
                    }
                }
            }
            else
            {
                MessageBox.Show("'SelectElementsFromLocation函数参数输入错误'！", "错误");
            }
            return SelectedElements;
        }
    }

    // 需要在Node类中添加对z方向位移和x,y方向转角的支持  
    public partial class Node
    {
        // 新增属性用于壳单元  
        public double Uz = 0;       // z方向位移  
        public double Rotx = 0;     // x轴转角  
        public double Roty = 0;     // y轴转角  
    }

    // 需要在RealConstant类中添加壳厚度属性  
    public partial class RealConstant
    {
        // 新增壳单元厚度属性  
        public double Thickness { get; set; }
    }

    // 需要在Material类中添加泊松比属性  
    public partial class Material
    {
        // 新增泊松比属性  
        public double PoissonRatio { get; set; }
    }
}