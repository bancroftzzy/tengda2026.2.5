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
    // 有限元计算中用到的各类静态函数
    static class FEM
    {
        public static Matrix<double> GetKg(List<Element> Elements, List<Node> Nodes)     // 【方法】生成结构刚度矩阵
        {
            Matrix<double> Kg = Matrix<double>.Build.Dense(Nodes.Count() * 3, Nodes.Count() * 3, 0.0);
            foreach(Element iElem in Elements)
            {
                // 单元刚度矩阵坐标变换
                Matrix<double> tempKg = iElem.Tr.Transpose() * iElem.Ke() * iElem.Tr;

                // 寻找插入位置
                int starti = 0, startj = 0;
                for (int i = 0; i < Nodes.Count(); i++)
                {
                    if (Nodes[i].No == iElem.Left.No)
                        starti = i;
                    if (Nodes[i].No == iElem.Right.No)
                        startj = i;
                }

                // 四块分别导入
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        Kg[starti * 3 + i, starti * 3 + j] += tempKg[i, j];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        Kg[starti * 3 + i, startj * 3 + j] += tempKg[i, j + 3];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        Kg[startj * 3 + i, starti * 3 + j] += tempKg[i + 3, j];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        Kg[startj * 3 + i, startj * 3 + j] += tempKg[i + 3, j + 3];

            }
            return Kg;
        }

        public static Vector<double> GetFg(List<Element> Elements, Vector<double> Fs)    // 【方法】生成荷载列向量
        {
            Vector<double> Fi = Vector<double>.Build.Dense(Fs.Count());                  // 单元初应变等效荷载向量
            foreach(Element i in Elements)                                               // 将单元初应变转化为节点荷载
            {
                double cForce = i.Material.Emodulus * i.RealConstant.Area * i.RealConstant.IniStrn;
                Fi[(i.Left.No - 1) * 3] += -cForce;
                Fi[(i.Right.No - 1) * 3] += cForce;
            }
            return Fs + Fi;
        }

        public static void KillElements(ref List<Element> Elements, List<int> index)     // 【方法】杀死单元
        {
            foreach (int i in index)
                Elements[i].isAlive = false;
        }

        public static void AliveElements(ref List<Element> Elements, List<int> index)    // 【方法】激活单元
        {
            foreach (int i in index)
                Elements[i].isAlive = true;
        }

        public static bool Check(Vector<double> Force, Matrix<double> ForceCohMat, double M1, double M2, double Q, List<int> CM_Elem_ECS, List<int> CM_Elem_Supports, List<int> AdjSupIndex, List<Element> Elements, ref List<Node> Nodes, List<Support> Supports, Vector<double> Fs, List<int> ConstrainedDOFIndex, out Vector<double> Disp, out Vector<double> RForce)
        {
            bool flag = true;
            Vector<double> ForceIni = ForceCohMat.Solve(Force);
            for (int i = 0; i < AdjSupIndex.Count(); i++)
                Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn += ForceIni[i] / Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus / Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;
            Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
            
            // 围护结构内力校核
            foreach (int i in CM_Elem_ECS) 
            {
                Elements[i].getNodalForce();
                if (Elements[i].iMom > M2 || Elements[i].iMom < -M1 || Elements[i].iFy > Q || Elements[i].iFy < -Q)
                    flag = false;
            }

            // 支撑轴力限值校核
            for(int i = 0; i < CM_Elem_Supports.Count(); i++)
            {
                if (Elements[CM_Elem_Supports[i]].isAlive) 
                {
                    Elements[CM_Elem_Supports[i]].getNodalForce();
                    if (Elements[CM_Elem_Supports[i]].jFx < -Supports[i].MaxFC / Supports[i].HrzDist || Elements[CM_Elem_Supports[i]].jFx > Supports[i].MaxFT / Supports[i].HrzDist)
                        flag = false;
                }
            }

            for (int i = 0; i < AdjSupIndex.Count(); i++)
                Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn -= ForceIni[i] / Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus / Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;
            //Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

            return flag;

        }

        public static bool FullyCheck(Vector<double> SupForce, Vector<double> IniForce0, Matrix<double> ForceCohMat, double M1, double M2, double Q, List<int> CM_Elem_ECS, List<int> CM_Elem_Supports, List<int> AdjSupIndex, List<Element> Elements, ref List<Node> Nodes, Vector<double> Fs, List<int> ConstrainedDOFIndex, out Vector<double> Disp, out Vector<double> RForce)
        {
            Vector<double> IniForce = ForceCohMat.Solve(SupForce);
            for (int i = 0; i < AdjSupIndex.Count(); i++) 
                Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn += IniForce0[i] / Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus / Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;
            Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);   //先进行一次有限元求解

            //Vector<double> ForceIni0 = Vector<double>.Build.Dense(AdjSupIndex.Count());
            //for (int i = 0; i < AdjSupIndex.Count(); i++)
            //    ForceIni0[i] = Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn * Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus * Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;
            //Vector<double> ForceByStep = ForceCohMat.Solve(IniForce - ForceIni0);

            bool flag = true;
            for (int i = AdjSupIndex.Count() - 1; i >= 0; i--)
            {
                Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn += (IniForce[i] - IniForce0[i]) / Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus / Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;

                Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

                List<double> Qs = new List<double>();
                List<double> Ms = new List<double>();
                foreach (int iElem in CM_Elem_ECS)
                {
                    Elements[iElem].getNodalForce();
                    Ms.Add(Elements[iElem].iMom);
                    Qs.Add(Elements[iElem].iFy);
                }

                if (Ms.Max() > M2 || Ms.Min() < -M1 || Qs.Max() > Q || Qs.Min() < -Q)
                    flag = false;
                    
            }

            // 单元初应变还原
            for (int i = AdjSupIndex.Count() - 1; i >= 0; i--)
                Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn -= IniForce[i] / Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus / Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;

            return flag;

        }

        public static double GetDispNormInf(List<Node> Nodes)                              // 【方法】返回围护结构位移无穷范数
        {
            double Ux = 0;
            foreach (Node i in Nodes)
            {
                if (i.Nx == 0 && Math.Abs(i.Ux) > Ux)
                    Ux = Math.Abs(i.Ux);
            }
            return Ux;
        }

        public static double GetDispNorm1(List<Node> Nodes)                              // 【方法】返回围护结构位移 1 范数
        {
            double Ux = 0;
            foreach (Node i in Nodes)
                Ux += Math.Abs(i.Ux);
            return Ux;
        }

        public static double GetDispNorm2(List<Node> Nodes)                              // 【方法】返回围护结构位移 2 范数
        {
            double Ux = 0;
            foreach (Node i in Nodes) 
            {
                if (i.Nx == 0)
                    Ux += Math.Pow(i.Ux, 2);
            }
            return Math.Sqrt(Ux);
        }

        // 【方法】有限元求解
        public static void Solve(List<Element> Elements, ref List<Node> Nodes, Vector<double> Fs, List<int> ConstrainedDOFIndex, out Vector<double> Disp,out Vector<double> RForce)
        {
            // 构造划行划列矩阵
            Matrix<double> TransK0 = Matrix<double>.Build.Dense((Nodes.Count() * 3) - ConstrainedDOFIndex.Count(), Nodes.Count() * 3, 0);
            Matrix<double> TransKc = Matrix<double>.Build.Dense(ConstrainedDOFIndex.Count(), Nodes.Count() * 3, 0);
            for (int i = 0, count = 0; i < Nodes.Count() * 3; i++)
            {
                if (!ConstrainedDOFIndex.Contains(i))
                {
                    count++;
                    TransK0[count - 1, i] = 1;
                }
                else
                    TransKc[i - count, i] = 1;
            }

            // 计算总刚矩阵 K0，计算约束反力所需矩阵 Kc（也即结力下 P16 式 8-34 中的 K_{aa} 和 K_{ba}）和划行后的荷载向量 F0
            Matrix<double> Kg = GetKg(Elements, Nodes);
            Vector<double> Fg = GetFg(Elements, Fs);
            Matrix<double> K0 = TransK0 * Kg * TransK0.Transpose();
            Matrix<double> Kc = TransKc * Kg * TransK0.Transpose();
            Vector<double> F0 = TransK0 * Fg;
            
            // 求解总刚方程得到结点位移
            Vector<double>  Disp0 = K0.Solve(F0);                         // 未约束结点的位移
            RForce = Kc * Disp0;                                          // 支座反力
            Disp = Vector<double>.Build.Dense(Nodes.Count() * 3, 0.0);    // 全部结点位移
            for (int i = 0, count = 0; i < Nodes.Count() * 3; i++)
            {
                if (!ConstrainedDOFIndex.Contains(i))
                {
                    count++;
                    Disp[i] = Disp0[count - 1];
                }
            }
            for (int i = 0; i < Nodes.Count(); i++)                       // 将位移数据赋予结点
                Nodes[i].LoadDisp(Disp[i * 3], Disp[i * 3 + 1], Disp[i * 3 + 2]);
        }
    }
}
