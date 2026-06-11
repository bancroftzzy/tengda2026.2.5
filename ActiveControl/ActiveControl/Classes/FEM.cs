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

        public static bool Check(Vector<double> Force, Matrix<double> ForceCohMat, double M1, double M2, double Q, List<int> CM_Elem_ECS, List<int> CM_Elem_Supports, List<int> AdjSupIndex, List<Element> Elements, ref List<Node> Nodes, List<Support> Supports, Vector<double> Fs, List<int> ConstrainedDOFIndex, Vector<double> Force0, out Vector<double> Disp, out Vector<double> RForce)
        {
            bool flag = true;

            // 打印输入的Force向量
            Console.WriteLine($"[Check输入] Force = {Force}");

            Vector<double> ForceIni = ForceCohMat.Solve(Force);

            // 打印计算得到的ForceIni向量
            Console.WriteLine($"[Check计算] ForceIni = {ForceIni}");

            for (int i = 0; i < AdjSupIndex.Count(); i++)
                Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.IniStrn += ForceIni[i] / Elements[CM_Elem_Supports[AdjSupIndex[i]]].Material.Emodulus / Elements[CM_Elem_Supports[AdjSupIndex[i]]].RealConstant.Area;
            Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
            
            // 围护结构内力校核
            foreach (int i in CM_Elem_ECS)
            {
                Elements[i].getNodalForce();
                if (Elements[i].iMom > M2 * 1e3 || Elements[i].iMom < -M1 * 1e3 || Elements[i].iFy > Q * 1e3 || Elements[i].iFy < -Q * 1e3)
                {
                     Console.WriteLine($"[Check失败] 单元{i}: iMom={Elements[i].iMom:F2} (限值: {-M1 * 1e3:F2} ~ {M2 * 1e3:F2}), iFy={Elements[i].iFy:F2} (限值: ±{Q * 1e3:F2})");
                    flag = false;
                }
            }

            // 围护结构位移校核（不允许正向位移）
            foreach (int i in CM_Elem_ECS)
            {
                int leftNodeIndex = Elements[i].Left.No - 1;
                int rightNodeIndex = Elements[i].Right.No - 1;

                if (Nodes[leftNodeIndex].Ux > 1e-6 || Nodes[rightNodeIndex].Ux > 1e-6)
                {
                    Console.WriteLine($"[Check失败] 围护结构出现正向位移: 单元{i}, 左节点{Elements[i].Left.No} Ux={Nodes[leftNodeIndex].Ux:F6}, 右节点{Elements[i].Right.No} Ux={Nodes[rightNodeIndex].Ux:F6}");
                    flag = false;
                    break;
                }
            }

            // 支撑轴力限值校核
            // for (int i = 0; i < CM_Elem_Supports.Count(); i++)
            // {
            //     if (Elements[CM_Elem_Supports[i]].isAlive)
            //     {
            //         Elements[CM_Elem_Supports[i]].getNodalForce();
            //         //if (Elements[CM_Elem_Supports[i]].jFx < -Supports[i].MaxFC / Supports[i].HrzDist || Elements[CM_Elem_Supports[i]].jFx > Supports[i].MaxFT / Supports[i].HrzDist)
            //         // if(Elements[CM_Elem_Supports[i]].jFx < -Supports[i].MaxFC / Supports[i].HrzDist || Elements[CM_Elem_Supports[i]].jFx > -1 * 10 * 1e3 / Supports[i].HrzDist)   // 这里将拉力限值放宽到10kN，实际工程中应根据支撑特性合理设置
            //         double jFx = Elements[CM_Elem_Supports[i]].jFx;  // 支撑轴力（负值=受压，正值=受拉）

            //         // 最大受压力（负值，绝对值最大）
            //         double maxForce = -1 * Supports[i].MaxFC / Supports[i].HrzDist;

            //         // 最小受压力（负值，绝对值最小）:20kN或Force0绝对值的10%，取较大值
            //         double minForce = Math.Min(-1 * 20 * 1e3, Force0[i] * 0.1);

            //         if (jFx < maxForce || jFx > minForce)
            //         {
            //              Console.WriteLine($"[Check失败] 支撑{i}: jFx={jFx:F2} (限值: {maxForce:F2} ~ {minForce:F2})");
            //             flag = false;
            //         }
            //     }
            // }
            for (int i = 0; i < CM_Elem_Supports.Count(); i++)
            {
                if (Elements[CM_Elem_Supports[i]].isAlive)
                {
                    Elements[CM_Elem_Supports[i]].getNodalForce();
                    double jFx = Elements[CM_Elem_Supports[i]].jFx;

                    double maxForce = -1 * Supports[i].MaxFC / Supports[i].HrzDist;

                    // 找到当前支撑在AdjSupIndex中的位置
                    int adjIndex = AdjSupIndex.IndexOf(i);
                    
                    double minForce;
                    if (adjIndex >= 0)  // 如果是可调支撑
                    {
                        // 使用Force0[adjIndex]（这才是正确的索引）
                        double minForceAbs = Math.Max(20 * 1e3, Math.Abs(Force0[adjIndex]) * 0.1);
                        minForce = -minForceAbs;
                    }
                    else  // 如果不是可调支撑
                    {
                        minForce = Supports[i].MaxFT / Supports[i].HrzDist;
                    }

                    if (jFx < maxForce || jFx > minForce)
                    {
                        Console.WriteLine($"[Check失败] 支撑{i}: jFx={jFx:F2} (限值: {maxForce:F2} ~ {minForce:F2})");
                        flag = false;
                    }
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
                if (i.Nx == 0 && Math.Abs(i.Ux) > Ux)                     //考虑了围护结构最大位移的绝对值
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

        /// <summary>
        /// 【方法】非线性有限元求解（用于邓肯-张模型）
        /// 使用切线刚度法迭代求解
        /// </summary>
        public static void SolveNonlinear(List<Element> Elements, ref List<Node> Nodes, Vector<double> Fs,
            List<int> ConstrainedDOFIndex, List<int> CM_Elem_SoilSpring, List<SoilLayer> SoilLayers,
            double currentSurfaceElev, double elevOfCollar, out Vector<double> Disp, out Vector<double> RForce,
            int maxIterations = 100, double tolerance = 1e-3, double relaxationFactor = 0.2)
        {
            // 输出非线性求解开始信息
            Console.WriteLine("\n========== 开始非线性土弹簧迭代求解（邓肯-张模型） ==========");

            // 迭代参数
            int iteration = 0;
            double maxStiffnessChange = double.MaxValue;

            // 保存每个土弹簧的初始刚度和当前刚度
            Dictionary<int, double> initialStiffness = new Dictionary<int, double>();
            Dictionary<int, double> currentStiffness = new Dictionary<int, double>();

            // 记录初始刚度
            int activeSoilSpringCount = 0;
            foreach (int index in CM_Elem_SoilSpring)
            {
                if (Elements[index].isAlive)
                {
                    Element elem = Elements[index];
                    SoilLayer soilLayer = GetSoilLayerAtDepth(SoilLayers, elem.Left.Ny, elevOfCollar);
                    if (soilLayer == null)
                        continue;

                    double length = GetElementLength(elem);
                    double area = GetSoilSpringArea(Nodes, elem);
                    double depth = currentSurfaceElev - elem.Left.Ny;
                    double physicalInitialStiffness = GetInitialDuncanChangSpringStiffness(soilLayer, depth, area, length);

                    initialStiffness[index] = physicalInitialStiffness;
                    currentStiffness[index] = Elements[index].RealConstant.Area > 1e-9
                        ? Elements[index].RealConstant.Area
                        : physicalInitialStiffness;
                    Elements[index].RealConstant.Area = currentStiffness[index];
                    activeSoilSpringCount++;
                }
            }

            // 输出初始刚度统计
            if (activeSoilSpringCount > 0)
            {
                double avgStiffness = initialStiffness.Values.Average();
                double minStiffness = initialStiffness.Values.Min();
                double maxStiffness = initialStiffness.Values.Max();
                Console.WriteLine($"活动土弹簧数量: {activeSoilSpringCount}");
                Console.WriteLine($"初始刚度统计 - 平均: {avgStiffness:E3} kN/m, 最小: {minStiffness:E3} kN/m, 最大: {maxStiffness:E3} kN/m");
                Console.WriteLine($"迭代参数 - 最大迭代次数: {maxIterations}, 收敛容差: {tolerance:E3}, 松弛因子: {relaxationFactor}");
            }
            else
            {
                Console.WriteLine("警告：没有活动的土弹簧单元");
            }
            
            // 迭代求解
            while (iteration < maxIterations && maxStiffnessChange > tolerance)
            {
                iteration++;

                // 1. 用当前刚度求解
                Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

                // 2. 更新土弹簧刚度
                maxStiffnessChange = 0;
                
                foreach (int index in CM_Elem_SoilSpring)
                {
                    if (!Elements[index].isAlive)
                        continue;

                    Element elem = Elements[index];

                    // 获取土弹簧所在的土层
                    SoilLayer soilLayer = GetSoilLayerAtDepth(SoilLayers, elem.Left.Ny, elevOfCollar);

                    if (soilLayer == null)
                        continue;  // 如果找不到土层，跳过

                    // 创建邓肯-张模型
                    DuncanChangModel dcModel = DuncanChangModel.FromSoilLayer(soilLayer);
                    
                    // 计算深度（从当前开挖面或回筑面算起）
                    double depth = currentSurfaceElev - elem.Left.Ny;
                    if (depth < 0.1)
                        depth = 0.1;
                    
                    // 估算围压（水平应力）
                    double sigma3 = DuncanChangModel.EstimateConfiningStress(depth, soilLayer.Gamma, soilLayer.K0);
                    
                    // 计算位移
                    double displacement = elem.Right.Ux - elem.Left.Ux;
                    
                    // 计算单元长度和面积
                    double length = GetElementLength(elem);
                    
                    // 计算作用面积（单元高度 × 单位宽度）
                    double area = GetSoilSpringArea(Nodes, elem);
                    
                    // 估算偏应力
                    double deviatorStress = DuncanChangModel.EstimateDeviatorStress(
                        displacement, currentStiffness[index], area);
                    
                    // 计算新的切线刚度
                    double newStiffness = dcModel.GetSpringStiffness(sigma3, deviatorStress, area, length);
                    
                    // 使用松弛因子更新刚度（提高收敛性）
                    double oldStiffness = currentStiffness[index];
                    double updatedStiffness = relaxationFactor * newStiffness + (1 - relaxationFactor) * oldStiffness;
                    
                    // 限制刚度变化范围（防止过大波动）
                    double minStiffness = initialStiffness[index] * 0.1;   // 最小为初始刚度的10%
                    double maxStiffness = initialStiffness[index] * 2.0;   // 最大为初始刚度的200%
                    double maxStepRatio = 0.2;                             // 单次迭代最大变化20%
                    
                    if (updatedStiffness < minStiffness)
                        updatedStiffness = minStiffness;
                    if (updatedStiffness > maxStiffness)
                        updatedStiffness = maxStiffness;
                    if (oldStiffness > 1e-9)
                    {
                        double minStepStiffness = oldStiffness * (1.0 - maxStepRatio);
                        double maxStepStiffness = oldStiffness * (1.0 + maxStepRatio);
                        if (updatedStiffness < minStepStiffness)
                            updatedStiffness = minStepStiffness;
                        if (updatedStiffness > maxStepStiffness)
                            updatedStiffness = maxStepStiffness;
                    }
                    
                    // 更新刚度
                    elem.RealConstant.Area = updatedStiffness;
                    currentStiffness[index] = updatedStiffness;
                    
                    // 计算刚度变化率
                    double stiffnessChange = oldStiffness > 1e-9
                        ? Math.Abs(updatedStiffness - oldStiffness) / oldStiffness
                        : Math.Abs(updatedStiffness - oldStiffness);
                    if (stiffnessChange > maxStiffnessChange)
                        maxStiffnessChange = stiffnessChange;
                }

                // 输出每次迭代信息
                Console.WriteLine($"迭代 {iteration}: 最大刚度变化率 = {maxStiffnessChange:E3}");
            }
            
            // 最后一次求解，确保结果一致
            Solve(Elements, ref Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

            // 输出收敛信息
            if (maxStiffnessChange <= tolerance)
            {
                Console.WriteLine($"✓ 非线性求解收敛，总迭代次数: {iteration}");
            }
            else
            {
                Console.WriteLine($"✗ 警告：非线性求解未完全收敛，总迭代次数: {iteration}，最大变化率: {maxStiffnessChange:E3}");
            }

            // 输出最终刚度统计
            if (currentStiffness.Count > 0)
            {
                double finalAvgStiffness = currentStiffness.Values.Average();
                double finalMinStiffness = currentStiffness.Values.Min();
                double finalMaxStiffness = currentStiffness.Values.Max();
                Console.WriteLine($"最终刚度统计 - 平均: {finalAvgStiffness:E3} kN/m, 最小: {finalMinStiffness:E3} kN/m, 最大: {finalMaxStiffness:E3} kN/m");
            }
            Console.WriteLine("========== 非线性求解完成 ==========\n");
        }

        /// <summary>
        /// 【辅助方法】计算土弹簧水平长度
        /// </summary>
        private static double GetElementLength(Element elem)
        {
            double length = Math.Sqrt(Math.Pow(elem.Left.Nx - elem.Right.Nx, 2) +
                                      Math.Pow(elem.Left.Ny - elem.Right.Ny, 2));
            return length > 1e-6 ? length : 1.0;
        }

        /// <summary>
        /// 【辅助方法】计算土弹簧控制面积（墙体节点上方相邻节点间距 × 单位宽度）
        /// </summary>
        private static double GetSoilSpringArea(List<Node> nodes, Element elem)
        {
            double currentY = elem.Right.Ny;
            double upperY = double.MaxValue;

            foreach (Node node in nodes)
            {
                if (Math.Abs(node.Nx - elem.Right.Nx) < 1e-3 &&
                    node.Ny > currentY + 1e-6 &&
                    node.Ny < upperY)
                {
                    upperY = node.Ny;
                }
            }

            double height = upperY < double.MaxValue ? upperY - currentY : 0.2;
            if (height < 1e-6)
                height = 0.2;

            return height * 1.0;
        }

        /// <summary>
        /// 【辅助方法】计算邓肯-张土弹簧物理初始刚度
        /// </summary>
        private static double GetInitialDuncanChangSpringStiffness(SoilLayer soilLayer, double depth, double area, double length)
        {
            if (depth < 0.1)
                depth = 0.1;

            DuncanChangModel dcModel = DuncanChangModel.FromSoilLayer(soilLayer);
            double sigma3 = DuncanChangModel.EstimateConfiningStress(depth, soilLayer.Gamma, soilLayer.K0);
            return dcModel.GetSpringStiffness(sigma3, 0.0, area, length);
        }

        /// <summary>
        /// 【辅助方法】根据深度获取土层
        /// </summary>
        private static SoilLayer GetSoilLayerAtDepth(List<SoilLayer> soilLayers, double elevation, double elevOfCollar)
        {
            double currentElev = elevOfCollar;
            
            foreach (SoilLayer layer in soilLayers)
            {
                currentElev -= layer.Thick;
                if (elevation > currentElev)
                {
                    return layer;
                }
            }
            
            // 如果没找到，返回最后一层
            return soilLayers.LastOrDefault();
        }
    }
}
