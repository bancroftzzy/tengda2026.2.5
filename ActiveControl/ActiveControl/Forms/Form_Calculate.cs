using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;

#pragma warning disable CS0197 // 将引用封送类的字段用作 ref 或 out 值或获取其地址可能导致运行时异常

namespace ActiveControl.Forms
{
    public partial class Form_Calculate : Form
    {
        readonly Form_Main mf;

        public List<Material> Materials = new List<Material>();
        public List<RealConstant> RealConstants = new List<RealConstant>();
        public List<int> ConstrainedDOFIndex;
        public List<int> CM_Node_ECS;
        public List<int> CM_Elem_ECS;
        public List<int> CM_Elem_SoilSpring;
        public List<int> CM_Elem_Supports;
        public Vector<double> Fs, RForce, Disp;

        public int Iter = 0;

        public Form_Calculate(Form_Main mf)
        {
            InitializeComponent();
            this.mf = mf;
            btnPause.Visible = false;
            btnContinue.Visible = false;
            rbtDirect.Checked = true;
            FormClosing += CalculationFormClosing;
        }

        private void btnStartOnce_Click(object sender, EventArgs e)
        {
            StartCalculation(false);
        }

        private void btnStartAll_Click(object sender, EventArgs e)
        {
            StartCalculation(true);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            mf.mre.Set();
            btnContinue.Visible = false;
            btnPause.Visible = true;
            mf.PrintString("继续计算……");
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            mf.PrintString("计算暂停……");
            mf.mre.Reset();
            btnPause.Visible = false;
            btnContinue.Visible = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (mf.Calculate == null || !mf.Calculate.IsAlive) return;
            mf.mre.Set();
            mf.Calculate.Abort();
        }

        private void Init()                      // 模型初始化
        {
            #region 初始化

            mf.PrintString("计算程序初始化……");

            mf.Nodes.Clear();
            mf.Elements.Clear();
            Materials.Clear();
            RealConstants.Clear();
            if (mf.SoilLayers.Count() * mf.Supports.Count() * mf.Loadcases.Count() == 0)      // 校核数据是否完整
            {
                mf.PrintString("初始数据缺失，请检查输入！");
                return;
            }
            if (mf.SoilLayers.Sum(o => o.Thick) < mf.LengthOfECS)                             // 校核土层厚度之和是否大于围护长度
            {
                mf.PrintString("土层厚度之和小于围护长度，请检查输入！");
                return;
            }
            if (mf.ElevOfCollar < mf.ElevOfGround)                                            // 校核钻孔孔口标高是否高于开挖地面标高
            {
                mf.SoilLayers[0].Thick += mf.ElevOfGround - mf.ElevOfCollar;
                mf.ElevOfCollar = mf.ElevOfGround;
                mf.PrintString("提示：钻孔孔口标高低于开挖地面标高，缺失部分按第一层土计算。");
            }
            mf.PrintString("计算程序初始化完成！");

            mf.cbStage.Items.Clear();
            mf.cbStage.Items.Add("包络");
            for (int i = 0; i < mf.Loadcases.Count(); i++)
                mf.cbStage.Items.Add((i + 1).ToString("0"));

            #endregion


            #region 生成节点

            // 生成围护结构关键节点     
            mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, mf.ElevOfGround));                    // 围护结构顶端
            mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, mf.ElevOfGround - mf.LengthOfECS));   // 围护结构底端
            double tempElev = mf.ElevOfCollar;
            foreach (SoilLayer i in mf.SoilLayers)                                       // 土层分界节点
            {
                tempElev -= i.Thick;
                if (tempElev > mf.ElevOfGround - mf.LengthOfECS)
                    mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, tempElev));
                else
                    break;
            }
            tempElev = mf.ElevOfGround;
            foreach (Loadcase i in mf.Loadcases)                                         // 开挖分界节点
            {
                tempElev -= i.ExcavationDepth;
                mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, tempElev));
            }
            foreach (Support i in mf.Supports)                                           // 支撑位置节点
                mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, mf.ElevOfGround - i.DistToGround));
            
            // ========== 新增：局部荷载影响位置节点 ==========
            if (mf.LocalLoads.Count > 0)                                                 // 判断是否有局部荷载
            {
                mf.PrintString($"开始计算局部荷载影响位置，共 {mf.LocalLoads.Count} 个局部荷载");
                
                foreach (LocalLoad load in mf.LocalLoads)                                // 局部荷载影响位置节点
                {
                   
                    double localLoadEcsBottom = mf.ElevOfGround - mf.LengthOfECS;
                    
                    // 计算近端影响深度 z1
                    double a_near = load.DistToECS;
                    double cElev = mf.ElevOfCollar;
                    double z1 = -999;
                    bool z1_valid = true;
                    
                    foreach (SoilLayer soil in mf.SoilLayers)
                    {
                        double phi = soil.Phi;
                        double beta = (45 + phi / 2) * Math.PI / 180;
                        double layerThick = soil.Thick;
                        double delta_a = layerThick / Math.Tan(beta);
                        
                        if (a_near < delta_a)
                        {
                            double depth_in_layer = a_near * Math.Tan(beta);
                            z1 = cElev - depth_in_layer;
                            break;
                        }
                        else
                        {
                            a_near -= delta_a;
                            cElev -= layerThick;
                            
                            if (cElev <= localLoadEcsBottom)
                            {
                                z1_valid = false;
                                break;
                            }
                        }
                    }
                    
                    // 如果z1有效且在围护结构范围内，添加节点
                    if (z1_valid && z1 > localLoadEcsBottom && z1 < mf.ElevOfGround)
                    {
                        load.Z1 = z1; //保存z1
                        mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, z1));
                        mf.PrintString($"    近端影响深度 z1 = {z1:F3}m，已添加节点");
                    }
                    else if (!z1_valid || z1 <= localLoadEcsBottom)
                    {
                        mf.PrintString($"    警告：局部荷载 {load.No} 近端距离太远，影响深度超出围护结构范围");
                    }
                    
                    // 计算远端影响深度 z2
                    double a_far = load.DistToECS + load.Width;
                    cElev = mf.ElevOfCollar;
                    double z2 = -999;
                    bool z2_valid = true;
                    
                    foreach (SoilLayer soil in mf.SoilLayers)
                    {
                        double phi = soil.Phi;
                        double beta = (45 + phi / 2) * Math.PI / 180;
                        double layerThick = soil.Thick;
                        double delta_a = layerThick / Math.Tan(beta);
                        
                        if (a_far < delta_a)
                        {
                            double depth_in_layer = a_far * Math.Tan(beta);
                            z2 = cElev - depth_in_layer;
                            break;
                        }
                        else
                        {
                            a_far -= delta_a;
                            cElev -= layerThick;
                            
                            if (cElev <= localLoadEcsBottom)
                            {
                                z2_valid = false;
                                break;
                            }
                        }
                    }
                    
                    // 如果z2有效且在围护结构范围内，添加节点
                    if (z2_valid && z2 > localLoadEcsBottom && z2 < mf.ElevOfGround)
                    {
                        load.Z2 = z2;
                        mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, z2));
                        mf.PrintString($"    远端影响深度 z2 = {z2:F3}m，已添加节点");
                    }
                    else if (!z2_valid || z2 <= localLoadEcsBottom)
                    {
                        z2 = localLoadEcsBottom;
                        load.Z2 = z2;
                        mf.PrintString($"    提示：局部荷载 {load.No} 远端超出围护结构，影响范围截断到底部 {z2:F3}m");
                    }

                    // 输出影响范围总结
                    if (z1_valid && z1 > localLoadEcsBottom)
                    {
                        mf.PrintString($"    局部荷载 {load.No} 影响标高范围: {z1:F3}m ~ {z2:F3}m");
                    }
                }

                mf.PrintString("局部荷载影响位置节点添加完成");
            }
            // ========== 局部荷载影响位置节点添加完成 ==========

            // ========== 新增：地下水位线节点 ==========
            if (Loadcase.EnableWater && Loadcase.WaterTableElev > -9999)
            {
                double waterElev = Loadcase.WaterTableElev;
                double waterNodeEcsTop = mf.ElevOfGround;
                double waterNodeEcsBottom = mf.ElevOfGround - mf.LengthOfECS;

                // 如果水位线在围护结构范围内，添加节点
                if (waterElev > waterNodeEcsBottom && waterElev < waterNodeEcsTop)
                {
                    mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0.0, waterElev));
                    mf.PrintString($"地下水位标高 {waterElev:F3}m，已添加水位线节点");
                }
            }
            // ========== 地下水位线节点添加完成 ==========

            mf.Nodes = mf.NodesDistinct(mf.Nodes);                                       // 节点列表去重
            mf.Nodes = mf.Nodes.OrderByDescending(o => o.Ny).ToList();                   // 按降序排序

            // 围护结构节点内插
            double nodeDist = 0.2;
            int tempInt = mf.Nodes.Count() - 1;
            for (int i = 0; i < tempInt; i++)              // 由于 Nodes.Count() 在变化，因此不可以直接用 for (int i = 0; i < Nodes.Count() - 1; i++)
            {
                for (double tempCoordy = mf.Nodes[i].Ny - nodeDist; tempCoordy > mf.Nodes[i + 1].Ny + 1e-5; tempCoordy -= nodeDist)
                    mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, 0, tempCoordy));
            }
            mf.Nodes = mf.Nodes.OrderByDescending(o => o.Ny).ToList();
            int NodeNumOfECS = mf.Nodes.Count();
            CM_Node_ECS = new List<int>();
            for (int i = 0; i < mf.Nodes.Count(); i++)     // 整理节点序号
                mf.Nodes[i].No = i + 1;
            for (int i = 0; i < NodeNumOfECS; i++)
                CM_Node_ECS.Add(i);

            // 偏移得到土弹簧另一端的节点
            for (int i = 1; i < NodeNumOfECS; i++)         // 顶端节点不偏移，i 从 1 开始
                mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, -1, mf.Nodes[i].Ny));

            // 建立支撑的另一端的节点
            foreach (Support i in mf.Supports)
                mf.Nodes.Add(new Node(mf.Nodes.Count() + 1, -mf.LengthOfSupports, mf.ElevOfGround - i.DistToGround));

            #endregion


            #region 生成单元

            // 定义材料
            Material Soil = new Material("Soil", 1, 0);
            Material Steel = new Material("Steel", 206e9, 7850);    // 弹性模量单位是N/m²，密度单位是kg/m³
            Material Concrete = new Material("Concrete", 31.5e9, 2400);

            // 生成围护结构单元
            CM_Elem_ECS = new List<int>();
            RealConstants.Add(new RealConstant(RealConstants.Count() + 1, 1.0 * mf.ThickOfECS, 1.0 * Math.Pow(mf.ThickOfECS, 3) / 12.0));  // 定义围护结构实常数
            for (int i = 1; i < NodeNumOfECS; i++)
            {
                mf.Elements.Add(new Element(mf.Elements.Count() + 1, mf.Nodes[i - 1], mf.Nodes[i], Concrete, "Beam", RealConstants.Last())); ;
                CM_Elem_ECS.Add(mf.Elements.Count() - 1);
            }

            //生成土弹簧单元
            CM_Elem_SoilSpring = new List<int>();
            for (int i = 1; i < NodeNumOfECS; i++)
            {
                Node iNode = Node.SelectNodeByLoc(mf.Nodes, -1, mf.Nodes[i].Ny);
                Node jNode = mf.Nodes[i];
                double ks = 0, cElev = mf.ElevOfCollar;
                foreach (SoilLayer j in mf.SoilLayers)     // 获取土层
                {
                    cElev -= j.Thick;
                    if (mf.Nodes[i].Ny > cElev)
                    {
                        double elemLength = mf.Nodes[i - 1].Ny - mf.Nodes[i].Ny;  // 单元长度
                        double depth = mf.ElevOfGround - mf.Nodes[i].Ny;          // 深度

                        if (Loadcase.UseNonlinearSoilSpring)
                        {
                            // 非线性土弹簧：使用邓肯-张模型计算初始刚度
                            double area = elemLength * 1.0;  // 截面积 (m²)
                            double springLength = GetElementLength(iNode, jNode);
                            double sigma3 = DuncanChangModel.EstimateConfiningStress(depth, j.Gamma, j.K0);
                            double Ei = DuncanChangModel.FromSoilLayer(j).GetInitialModulus(sigma3);
                            ks = GetInitialDuncanChangSpringStiffness(j, depth, area, springLength);
                            Console.WriteLine($"[初始刚度-非线性] 节点{i}, 深度={depth:F2}m, σ3={sigma3:F2}kPa, Ei={Ei:F2}kPa, ks={ks:E3}kN/m");
                        }
                        else
                        {
                            // 线性土弹簧：使用m法计算刚度
                            // 刚度Ks = 土层厚度a × 水平宽度b1 × 水平抗力比例系数m × 土层到地面距离z，注意单位换算
                            ks = elemLength * 1.0 * j.M * 1e6 * depth;
                            Console.WriteLine($"[初始刚度-线性] 节点{i}, 深度={depth:F2}m, m={j.M:F2}MN/m⁴, ks={ks:E3}kN/m");
                        }
                        break;
                    }
                }
                RealConstants.Add(new RealConstant(RealConstants.Count() + 1, ks, 0.0));
                mf.Elements.Add(new Element(mf.Elements.Count() + 1, iNode, jNode, Soil, "Link", RealConstants.Last()));
                CM_Elem_SoilSpring.Add(mf.Elements.Count() - 1);
            }

            // 生成支撑单元
            CM_Elem_Supports = new List<int>();
            foreach (Support i in mf.Supports)
            {
                Node iNode = Node.SelectNodeByLoc(mf.Nodes, -mf.LengthOfSupports, mf.ElevOfGround - i.DistToGround);
                Node jNode = Node.SelectNodeByLoc(mf.Nodes, 0, mf.ElevOfGround - i.DistToGround);
                Material SupportMat;
                double AvgArea;
                if (i.Mat == "钢")
                {
                    SupportMat = Steel;
                    AvgArea = (Math.Pow(i.Size1, 2) - Math.Pow(i.Size1 - 2.0 * i.Size2, 2)) * Math.PI / 4 / i.HrzDist;
                }
                else
                {
                    SupportMat = Concrete;
                    AvgArea = i.Size1 * i.Size2 / i.HrzDist;
                }
                RealConstants.Add(new RealConstant(RealConstants.Count() + 1, AvgArea, 0.0));
                mf.Elements.Add(new Element(mf.Elements.Count() + 1, iNode, jNode, SupportMat, "Link", RealConstants.Last()));
                CM_Elem_Supports.Add(mf.Elements.Count() - 1);
            }

            #endregion


            #region 施加约束

            // 标记约束自由度
            ConstrainedDOFIndex = new List<int> { (NodeNumOfECS - 1) * 3 + 1 };     // 约束围护结构底端竖向位移
            for (int i = NodeNumOfECS; i < mf.Nodes.Count(); i++)    // 约束围护结构底端以后的全部节点的全部自由度
            {
                ConstrainedDOFIndex.Add(i * 3);
                ConstrainedDOFIndex.Add(i * 3 + 1);
                ConstrainedDOFIndex.Add(i * 3 + 2);
            }

            #endregion


            #region 荷载列向量初始化

            Fs = Vector<double>.Build.Dense(mf.Nodes.Count() * 3);   // 土压力荷载向量
            
            // 施加局部荷载
            if (mf.LocalLoads.Count > 0)
            {
                mf.PrintString($"开始施加局部荷载，共 {mf.LocalLoads.Count} 个");
                
                foreach (LocalLoad load in mf.LocalLoads)
                {
                    // 检查局部荷载是否有效
                    if (load.Z1 == -999)
                    {
                        mf.PrintString($"  局部荷载 {load.No} 无效（影响深度超出范围），跳过");
                        continue;
                    }
                    
                    mf.PrintString($"  施加局部荷载 {load.No}，影响范围: {load.Z1:F3}m ~ {load.Z2:F3}m");
                    
                    // 遍历围护结构单元
                    int affectedElemCount = 0;
                    for (int i = 0; i < CM_Elem_ECS.Count; i++)
                    {
                        double node_i_elev = mf.Nodes[i].Ny;
                        double node_j_elev = mf.Nodes[i + 1].Ny;
                        
                        // 判断单元是否在影响范围内
                        if (node_i_elev <= load.Z1 && node_j_elev >= load.Z2)
                        {
                            // 判断单元在哪个土层
                            double cElev = mf.ElevOfCollar;
                            foreach (SoilLayer j in mf.SoilLayers)
                            {
                                cElev -= j.Thick;
                                if (mf.Nodes[i].Ny > cElev)
                                {
                                    // 计算主动土压力系数
                                    double Ka = Math.Pow(Math.Tan((45 - j.Phi / 2) * Math.PI / 180), 2);
                                    
                                    // 计算附加水平应力（均布荷载）
                                    double q = Ka * load.LocalGroundLoad * 1e3;  // kPa转Pa
                                    
                                    // 单元长度
                                    double elem_length = mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny;
                                    
                                    // 均布荷载转化为节点等效荷载
                                    double tempFyi = -q * elem_length / 2;
                                    double tempFyj = -q * elem_length / 2;
                                    double tempMomi = q / 12 * Math.Pow(elem_length, 2);
                                    double tempMomj = -q / 12 * Math.Pow(elem_length, 2);
                                    
                                    // 累加到荷载向量
                                    Fs[i * 3] += tempFyi;
                                    Fs[i * 3 + 2] += tempMomi;
                                    Fs[(i + 1) * 3] += tempFyj;
                                    Fs[(i + 1) * 3 + 2] += tempMomj;
                                    
                                    affectedElemCount++;
                                    break;
                                }
                            }
                        }
                    }
                    
                    mf.PrintString($"    影响单元数: {affectedElemCount}");
                }
                
                mf.PrintString("局部荷载施加完成");
            }

            double Sy = mf.GroundLoad * 1e3;



            if (!(Loadcase.WaterTableElev > mf.ElevOfGround - mf.LengthOfECS && Loadcase.WaterTableElev < mf.ElevOfGround && Loadcase.EnableWater))
            {
                // ==========不考虑地下水影响========== //
                for (int i = 0; i < CM_Elem_ECS.Count(); i++)            // 土压力
                {
                    double cElev = mf.ElevOfCollar;                      // 获取土层
                    foreach (SoilLayer j in mf.SoilLayers)
                    {
                        cElev -= j.Thick;
                        if (mf.Nodes[i].Ny > cElev)
                        {
                            // 计算单元两端土压力荷载集度
                            double Ka = Math.Pow(Math.Tan((45 - j.Phi / 2) * Math.PI / 180), 2);       // 主动土压力系数
                            double qi = Math.Max(Sy * Ka - 2 * j.C * Math.Pow(Ka, 0.5) * 1e3, 0);
                            Sy += (mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny) * j.Gamma * 1e3;
                            double qj = Math.Max(Sy * Ka - 2 * j.C * Math.Pow(Ka, 0.5) * 1e3, 0);

                            // 将节间荷载转化为节点荷载
                            double tempFyi = -(7.0 * qi + 3.0 * qj) / 20.0 * (mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny);
                            double tempMomi = (3.0 * qi + 2.0 * qj) / 60.0 * Math.Pow(mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny, 2);
                            double tempFyj = -(3.0 * qi + 7.0 * qj) / 20.0 * (mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny);
                            double tempMomj = -(2.0 * qi + 3.0 * qj) / 60.0 * Math.Pow(mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny, 2);

                            mf.Elements[CM_Elem_ECS[i]].iFy0 = tempFyi;
                            mf.Elements[CM_Elem_ECS[i]].jFy0 = tempFyj;
                            mf.Elements[CM_Elem_ECS[i]].iMom0 = tempMomi;
                            mf.Elements[CM_Elem_ECS[i]].jMom0 = tempMomj;

                            Fs[i * 3] += tempFyi;
                            Fs[i * 3 + 2] += tempMomi;
                            Fs[(i + 1) * 3] += tempFyj;
                            Fs[(i + 1) * 3 + 2] += tempMomj;
                            break;
                        }
                    }
                }
            }
            else
            {
                    // ========== 考虑地下水影响：根据水土模式计算 ==========
                for (int i = 0; i < CM_Elem_ECS.Count(); i++)
                {
                    double cElev = mf.ElevOfCollar;
                    foreach (SoilLayer j in mf.SoilLayers)
                    {
                        cElev -= j.Thick;
                        if (mf.Nodes[i].Ny > cElev)
                        {
                            // 确定使用的重度和水土模式
                            double gamma_to_use = j.Gamma;
                            string mode = j.WaterSoilMode;
                            
                            // 判断单元的上节点是否在水位以下
                            if (mf.Nodes[i].Ny < Loadcase.WaterTableElev)
                            {
                                // 处理"自动"模式
                                if (mode == "自动" || mode == "自动（根据土性）")
                                {
                                    if (j.Type == "杂填土" || j.Type == "砂质粉土" || j.Type == "粉砂")
                                        mode = "水土分算";
                                    else
                                        mode = "水土合算";
                                }
                                
                                // 根据水土模式选择重度
                                if (mode == "水土分算")
                                    gamma_to_use = j.GammaEff;
                                else if (mode == "水土合算")
                                    gamma_to_use = j.GammaSat;
                            }
                            
                            // 计算土压力
                            double Ka = Math.Pow(Math.Tan((45 - j.Phi / 2) * Math.PI / 180), 2);
                            double qi = Math.Max(Sy * Ka - 2 * j.C * Math.Pow(Ka, 0.5) * 1e3, 0);
                            Sy += (mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny) * gamma_to_use * 1e3;
                            double qj = Math.Max(Sy * Ka - 2 * j.C * Math.Pow(Ka, 0.5) * 1e3, 0);
                            
                            // 水土分算时，额外计算水压力
                            double qi_water = 0, qj_water = 0;
                            if (mode == "水土分算")
                            {
                                if (mf.Nodes[i].Ny < Loadcase.WaterTableElev)
                                    qi_water = 9.81 * (Loadcase.WaterTableElev - mf.Nodes[i].Ny) * 1e3;
                                if (mf.Nodes[i + 1].Ny < Loadcase.WaterTableElev)
                                    qj_water = 9.81 * (Loadcase.WaterTableElev - mf.Nodes[i + 1].Ny) * 1e3;
                            }
                            
                            // 总压力 = 土压力 + 水压力
                            double qi_total = qi + qi_water;
                            double qj_total = qj + qj_water;
                            
                            // 将节间荷载转化为节点荷载
                            double tempFyi = -(7.0 * qi_total + 3.0 * qj_total) / 20.0 * (mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny);
                            double tempMomi = (3.0 * qi_total + 2.0 * qj_total) / 60.0 * Math.Pow(mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny, 2);
                            double tempFyj = -(3.0 * qi_total + 7.0 * qj_total) / 20.0 * (mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny);
                            double tempMomj = -(2.0 * qi_total + 3.0 * qj_total) / 60.0 * Math.Pow(mf.Nodes[i].Ny - mf.Nodes[i + 1].Ny, 2);

                            mf.Elements[CM_Elem_ECS[i]].iFy0 = tempFyi;
                            mf.Elements[CM_Elem_ECS[i]].jFy0 = tempFyj;
                            mf.Elements[CM_Elem_ECS[i]].iMom0 = tempMomi;
                            mf.Elements[CM_Elem_ECS[i]].jMom0 = tempMomj;

                            Fs[i * 3] += tempFyi;
                            Fs[i * 3 + 2] += tempMomi;
                            Fs[(i + 1) * 3] += tempFyj;
                            Fs[(i + 1) * 3 + 2] += tempMomj;
                            break;
            }
        }
    }
            }
            // // 局部荷载（滑动面传递法）
            // if (mf.LocalLoads.Count > 0)
            // {
            //     mf.PrintString($"开始计算局部荷载，共 {mf.LocalLoads.Count} 个");
                
            //     foreach (LocalLoad load in mf.LocalLoads)
            //     {
            //         mf.PrintString($"计算局部荷载 {load.No}");
                    
            //         double ecsBottom = mf.ElevOfGround - mf.LengthOfECS;
                    
            //         // 追踪荷载近端滑动面，确定 z1
            //         double a_near = load.DistToECS;
            //         double cElev = mf.ElevOfCollar;
            //         double z1 = -999;
            //         bool z1_out_of_range = false;
                    
            //         foreach (SoilLayer soil in mf.SoilLayers)
            //         {
            //             double phi = soil.Phi;
            //             double beta = (45 + phi / 2) * Math.PI / 180;
            //             double layerThick = soil.Thick;
            //             double delta_a = layerThick / Math.Tan(beta);
                        
            //             if (a_near < delta_a)
            //             {
            //                 double depth_in_layer = a_near * Math.Tan(beta);
            //                 z1 = cElev - depth_in_layer;
            //                 break;
            //             }
            //             else
            //             {
            //                 a_near -= delta_a;
            //                 cElev -= layerThick;
                            
            //                 if (cElev <= ecsBottom)
            //                 {
            //                     z1_out_of_range = true;
            //                     break;
            //                 }
            //             }
            //         }
                    
            //         if (z1_out_of_range || z1 < ecsBottom)
            //         {
            //             mf.PrintString($"  警告：局部荷载 {load.No} 距离太远，影响深度超出围护结构范围");
            //             continue;
            //         }
                    
            //         // 追踪荷载远端滑动面，确定 z2
            //         double a_far = load.DistToECS + load.Width;
            //         cElev = mf.ElevOfCollar;
            //         double z2 = -999;
            //         bool z2_out_of_range = false;
                    
            //         foreach (SoilLayer soil in mf.SoilLayers)
            //         {
            //             double phi = soil.Phi;
            //             double beta = (45 + phi / 2) * Math.PI / 180;
            //             double layerThick = soil.Thick;
            //             double delta_a = layerThick / Math.Tan(beta);
                        
            //             if (a_far < delta_a)
            //             {
            //                 double depth_in_layer = a_far * Math.Tan(beta);
            //                 z2 = cElev - depth_in_layer;
            //                 break;
            //             }
            //             else
            //             {
            //                 a_far -= delta_a;
            //                 cElev -= layerThick;
                            
            //                 if (cElev <= ecsBottom)
            //                 {
            //                     z2_out_of_range = true;
            //                     break;
            //                 }
            //             }
            //         }
                    
            //         if (z2_out_of_range || z2 < ecsBottom)
            //         {
            //             z2 = ecsBottom;
            //             mf.PrintString($"  提示：局部荷载 {load.No} 远端超出围护结构，影响范围截断到底部");
            //         }
                    
            //         mf.PrintString($"  影响标高: {z1:F2}m ~ {z2:F2}m");
                    
            //         // 遍历围护结构单元，施加荷载
            //         for (int i = 0; i < CM_Elem_ECS.Count; i++)
            //         {
            //             double node_i_elev = mf.Nodes[i].Ny;
            //             double node_j_elev = mf.Nodes[i + 1].Ny;
                        
            //             // 判断单元是否在影响范围内
            //             if (node_i_elev <= z1 && node_j_elev >= z2)
            //             {
            //                 // 找到单元所在的土层
            //                 double cElev_temp = mf.ElevOfCollar;
            //                 double elem_mid_elev = (node_i_elev + node_j_elev) / 2;
                            
            //                 foreach (SoilLayer soil in mf.SoilLayers)
            //                 {
            //                     double layer_top = cElev_temp;
            //                     double layer_bottom = cElev_temp - soil.Thick;
                                
            //                     if (elem_mid_elev <= layer_top && elem_mid_elev > layer_bottom)
            //                     {
            //                         double phi = soil.Phi;
            //                         double Ka = Math.Pow(Math.Tan((45 - phi / 2) * Math.PI / 180), 2);
            //                         double delta_sigma_h = Ka * load.LocalGroundLoad * 1e3;
                                    
            //                         double elem_height = node_i_elev - node_j_elev;
            //                         double force_i = delta_sigma_h * elem_height / 2;
            //                         double force_j = delta_sigma_h * elem_height / 2;
                                    
            //                         Fs[i * 3] += force_i;
            //                         Fs[(i + 1) * 3] += force_j;
                                    
            //                         break;
            //                     }
                                
            //                     cElev_temp -= soil.Thick;
            //                 }
            //             }
            //         }
            //     }
            // }



            Vector<double> Fg = FEM.GetFg(mf.Elements, Fs);          // 生成总荷载向量

            //// 在列表中显示荷载信息
            //rtbOutputWindow.Text += ">> " + System.DateTime.Now.ToString() + "  输出荷载信息：\r\n自由度编号\t节点荷载\r\n";
            //for (int i = 0; i < Fg.Count; i++)
            //    rtbOutputWindow.Text += (i + 1).ToString("0") + "\t" + Fg[i].ToString("0") + "\r\n";
            //rtbOutputWindow.SelectionStart = rtbOutputWindow.Text.Length;
            //rtbOutputWindow.ScrollToCaret();

            #endregion


            #region 计算准备

            // 计算初始，激活土弹簧单元和围护结构单元，杀死支撑
            FEM.AliveElements(ref mf.Elements, CM_Elem_ECS);
            FEM.AliveElements(ref mf.Elements, CM_Elem_SoilSpring);
            FEM.KillElements(ref mf.Elements, CM_Elem_Supports);

            // 计算初始，获得土弹簧初应变
            foreach (int index in CM_Elem_SoilSpring)                               // 土弹簧刚度放大 1e20 倍
                mf.Elements[index].RealConstant.Area *= 1e20;
            FEM.Solve(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
            foreach (int index in CM_Elem_SoilSpring)                               // 土弹簧赋初应变
            {
                mf.Elements[index].RealConstant.Area /= 1e20;                       // 土弹簧刚度复原
                double tRForce = RForce[(index - CM_Elem_SoilSpring[0]) * 3 + 1];   // 取出土弹簧轴力，并对土弹簧赋初应变
                mf.Elements[index].RealConstant.IniStrn = tRForce / mf.Elements[index].Material.Emodulus / mf.Elements[index].RealConstant.Area;
            }

            // 数据初始化
            Loadcase.CurLCNo = 0;
            Loadcase.CurElev = mf.ElevOfGround;
            Loadcase.ExcCount = 0;                         // 开挖次数
            Loadcase.ActSupCount = 0;                      // 已激活支撑数
            Loadcase.AdjSupIndex = new List<int>();        // 可调节支撑索引

            // 包络数据初始化
            mf.MomMax = Vector<double>.Build.Dense(CM_Elem_ECS.Count());       // 围护结构弯矩
            mf.MomMin = Vector<double>.Build.Dense(CM_Elem_ECS.Count());
            mf.FyMax  = Vector<double>.Build.Dense(CM_Elem_ECS.Count());       // 围护结构剪力
            mf.FyMin  = Vector<double>.Build.Dense(CM_Elem_ECS.Count());
            mf.UxMax  = Vector<double>.Build.Dense(CM_Node_ECS.Count());       // 围护结构位移
            mf.UxMin  = Vector<double>.Build.Dense(CM_Node_ECS.Count());
            mf.FxMax  = Vector<double>.Build.Dense(CM_Elem_Supports.Count());  // 支撑轴力
            mf.FxMin  = Vector<double>.Build.Dense(CM_Elem_Supports.Count());

            // *Sum 数据初始化
            mf.DispSum = Matrix<double>.Build.Dense(mf.Nodes.Count() * 3, mf.Loadcases.Count(), 0.0);
            mf.InistrnSum = Matrix<double>.Build.Dense(mf.Elements.Count(), mf.Loadcases.Count(), 0.0);
            mf.AliveSum = Matrix<double>.Build.Dense(mf.Elements.Count(), mf.Loadcases.Count());
            mf.IniForceSum = Matrix<double>.Build.Dense(mf.Supports.Count(), mf.Loadcases.Count(), 0.0);
            mf.JackStrokeCurrent = Vector<double>.Build.Dense(mf.Supports.Count(), 0.0);
            mf.JackStrokeSum = Matrix<double>.Build.Dense(mf.Supports.Count(), mf.Loadcases.Count(), 0.0);

            #endregion
        }
        private void Construction()
        {
            // 1. 开挖操作
            if (mf.Loadcases[Loadcase.CurLCNo].ExcavationDepth > 1e-6)
            {
                Loadcase.CurElev -= mf.Loadcases[Loadcase.CurLCNo].ExcavationDepth;            // 当前地面标高
                foreach (int index in CM_Elem_SoilSpring)
                {
                    if (mf.Elements[index].Left.Ny >= Loadcase.CurElev - 1e-6)              // 杀死挖去部分及开挖面处弹簧单元
                    {
                        mf.Elements[index].isAlive = false;
                        mf.Elements[index].RealConstant.IniStrn = 0;
                    }
                    else                                                        // 修改下部土弹簧刚度
                    {
                        double cElev = mf.ElevOfCollar;
                        foreach (SoilLayer j in mf.SoilLayers)                     // 获取土层
                        {
                            cElev -= j.Thick;
                            if (mf.Elements[index].Left.Ny > cElev)
                            {
                                // 刚度Ks = 土层厚度a × 水平宽度b1 × 水平抗力比例系数m × 土层到地面距离z，注意单位换算
                                // 找到围护墙上当前节点的上方相邻节点,计算单元长度
                                double currentY = mf.Elements[index].Right.Ny;
                                double upperY = currentY;
                                // 在围护墙节点中找到紧邻上方的节点
                                foreach (int ecsIndex in CM_Node_ECS)
                                {
                                    if (mf.Nodes[ecsIndex].Ny > currentY)
                                    {
                                        if (upperY == currentY || mf.Nodes[ecsIndex].Ny < upperY)
                                        {
                                            upperY = mf.Nodes[ecsIndex].Ny;
                                        }
                                    }
                                }
                                double elemLength = upperY - currentY;
                                double depth = Loadcase.CurElev - mf.Elements[index].Left.Ny;

                                if (Loadcase.UseNonlinearSoilSpring)
                                {
                                    // 非线性土弹簧：使用邓肯-张模型计算刚度
                                    double area = elemLength * 1.0;  // 截面积 (m²)
                                    double springLength = GetElementLength(mf.Elements[index].Left, mf.Elements[index].Right);
                                    mf.Elements[index].RealConstant.Area = GetInitialDuncanChangSpringStiffness(j, depth, area, springLength);
                                }
                                else
                                {
                                    // 线性土弹簧：使用m法计算刚度
                                    mf.Elements[index].RealConstant.Area = elemLength * 1.0 * j.M * 1e6 * depth;
                                }
                                break;
                            }
                        }
                    }
                }
                Loadcase.ExcCount++;
            }

            // 2. 激活支撑并设置初应变
            if (mf.Loadcases[Loadcase.CurLCNo].IsActiveSupport == true)
            {
                mf.Elements[CM_Elem_Supports[Loadcase.ActSupCount]].isAlive = true;
                mf.Elements[CM_Elem_Supports[Loadcase.ActSupCount]].RealConstant.IniStrn =
                    (mf.Elements[CM_Elem_Supports[Loadcase.ActSupCount]].Right.Ux -
                     mf.Elements[CM_Elem_Supports[Loadcase.ActSupCount]].Left.Ux) /
                    mf.LengthOfSupports;
                Loadcase.ActSupCount++;
                if (mf.Supports[Loadcase.ActSupCount - 1].AdjAble == true)
                    Loadcase.AdjSupIndex.Add(Loadcase.ActSupCount - 1);
            }
            
            // 3. 浇筑顶板（新增功能）
            if (mf.Loadcases[Loadcase.CurLCNo].IsAddSlab == true)
            {
                double slabElev = mf.Loadcases[Loadcase.CurLCNo].SlabElevation;
                double slabThick = mf.Loadcases[Loadcase.CurLCNo].SlabThickness;
                
                // 创建顶板梁单元
                Node leftNode = Node.SelectNodeByLoc(mf.Nodes, 0, slabElev);
                Node rightNode = Node.SelectNodeByLoc(mf.Nodes, -mf.LengthOfSupports, slabElev);
                
                if (leftNode != null && rightNode != null)
                {
                    // 定义顶板实常数
                    RealConstant slabRC = new RealConstant(
                        RealConstants.Count() + 1,
                        1.0 * slabThick,                        // 面积
                        1.0 * Math.Pow(slabThick, 3) / 12.0    // 惯性矩
                    );
                    RealConstants.Add(slabRC);
                    
                    // 创建顶板梁单元
                    Material Concrete = new Material("Concrete", 31.5e9, 2400);
                    Element slabElem = new Element(
                        mf.Elements.Count() + 1,
                        leftNode,
                        rightNode,
                        Concrete,
                        "Beam",
                        slabRC
                    );
                    mf.Elements.Add(slabElem);
                    Loadcase.SlabElemIndex.Add(mf.Elements.Count() - 1);
                    
                    mf.PrintString($"浇筑顶板：标高{slabElev:F2}m，厚度{slabThick:F2}m");
                }
                else
                {
                    mf.PrintString($"警告：无法在标高{slabElev:F2}m处找到节点，顶板浇筑失败！");
                }
            }
            
            // 4. 拆除支撑（新增功能 - 增量法）
            if (mf.Loadcases[Loadcase.CurLCNo].IsRemoveSupport == true)
            {
                int removeIndex = mf.Loadcases[Loadcase.CurLCNo].RemoveSupportIndex;
                
                if (removeIndex >= 0 && removeIndex < CM_Elem_Supports.Count())
                {
                    int supportElemIndex = CM_Elem_Supports[removeIndex];
                    
                    // 检查支撑是否激活
                    if (mf.Elements[supportElemIndex].isAlive)
                    {
                        // 4.1 先求解一次，获取当前支撑抗力
                        SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                        mf.Elements[supportElemIndex].getNodalForce();
                        double supportForce = -mf.Elements[supportElemIndex].jFx;  // 支撑轴力（正值为压力）
                        
                        // 4.2 钝化支撑单元
                        mf.Elements[supportElemIndex].isAlive = false;
                        mf.Elements[supportElemIndex].RealConstant.IniStrn = 0;
                        
                        // 4.3 在围护墙对应位置施加反向荷载（关键步骤）
                        int nodeIndex = mf.Elements[supportElemIndex].Right.No - 1;
                        Fs[nodeIndex * 3] += -supportForce;  // X方向反向荷载
                        
                        mf.PrintString($"拆除第{removeIndex + 1}道支撑，支撑抗力{supportForce / 1e3:F2} kN，施加反向荷载{-supportForce / 1e3:F2} kN");
                    }
                    else
                    {
                        mf.PrintString($"警告：第{removeIndex + 1}道支撑未激活，无法拆除！");
                    }
                }
                else
                {
                    mf.PrintString($"错误：支撑索引{removeIndex}超出范围！");
                }
            }
            
            // 5. 回筑操作（新增功能 - 增量法）
            if (mf.Loadcases[Loadcase.CurLCNo].BackfillDepth < -1e-6)
            {
                double backfillDepth = -mf.Loadcases[Loadcase.CurLCNo].BackfillDepth;  // 转为正值
                Loadcase.CurElev += backfillDepth;  // 开挖面标高上升
                
                mf.PrintString($"回筑{backfillDepth:F2}m，当前开挖面标高{Loadcase.CurElev:F2}m");
                
                // 激活被回填区域的土弹簧
                foreach (int index in CM_Elem_SoilSpring)
                {
                    if (mf.Elements[index].Left.Ny < Loadcase.CurElev - 1e-6 && !mf.Elements[index].isAlive)
                    {
                        // 激活土弹簧
                        mf.Elements[index].isAlive = true;
                        
                        // 重新计算土弹簧刚度（考虑回填土特性）
                        double cElev = mf.ElevOfCollar;
                        foreach (SoilLayer j in mf.SoilLayers)
                        {
                            cElev -= j.Thick;
                            if (mf.Elements[index].Left.Ny > cElev)
                            {
                                // 找到围护墙上当前节点的上方相邻节点,计算单元长度
                                double currentY = mf.Elements[index].Right.Ny;
                                double upperY = currentY;
                                foreach (int ecsIndex in CM_Node_ECS)
                                {
                                    if (mf.Nodes[ecsIndex].Ny > currentY)
                                    {
                                        if (upperY == currentY || mf.Nodes[ecsIndex].Ny < upperY)
                                        {
                                            upperY = mf.Nodes[ecsIndex].Ny;
                                        }
                                    }
                                }
                                double elemLength = upperY - currentY;
                                double depth = Loadcase.CurElev - mf.Elements[index].Left.Ny;

                                // 回填土刚度 = 原状土刚度 × 压实系数（这里假设压实系数为0.8）
                                double compactionFactor = 0.8;
                                double ks;

                                if (Loadcase.UseNonlinearSoilSpring)
                                {
                                    // 非线性土弹簧：使用邓肯-张模型计算刚度
                                    double area = elemLength * 1.0;  // 截面积 (m²)
                                    double springLength = GetElementLength(mf.Elements[index].Left, mf.Elements[index].Right);
                                    ks = GetInitialDuncanChangSpringStiffness(j, depth, area, springLength) * compactionFactor;
                                }
                                else
                                {
                                    // 线性土弹簧：使用m法计算刚度
                                    ks = elemLength * 1.0 * j.M * 1e6 * depth * compactionFactor;
                                }

                                mf.Elements[index].RealConstant.Area = ks;

                                // 计算回填土压力增量（主动土压力）
                                double Ka = Math.Pow(Math.Tan((45 - j.Phi / 2) * Math.PI / 180), 2);
                                double P_backfill = Ka * j.Gamma * 1e3 * depth;
                                
                                // 通过初应变施加回填土压力
                                // 注意：这里是增量，不是总量
                                if (mf.Elements[index].RealConstant.Area > 1e-6)
                                {
                                    mf.Elements[index].RealConstant.IniStrn = P_backfill / 
                                        (mf.Elements[index].Material.Emodulus * mf.Elements[index].RealConstant.Area);
                                }
                                
                                break;
                            }
                        }
                    }
                }
            }
            
            for (int i = 0; i < Loadcase.ActSupCount; i++)
                mf.IniForceSum[i, Loadcase.CurLCNo] = mf.Elements[CM_Elem_Supports[i]].RealConstant.IniStrn;

            Loadcase.CurLCNo++;
        }
        private void CopyToSum()                 // 将当前工况计算结果复制到 *Sum 数组
        {
            // 浇筑顶板等阶段会新增单元，历史结果表随模型扩展，已有阶段数据保留。
            if (mf.Elements.Count > mf.InistrnSum.RowCount)
            {
                var strain = Matrix<double>.Build.Dense(mf.Elements.Count, mf.Loadcases.Count);
                var alive = Matrix<double>.Build.Dense(mf.Elements.Count, mf.Loadcases.Count);
                strain.SetSubMatrix(0, 0, mf.InistrnSum);
                alive.SetSubMatrix(0, 0, mf.AliveSum);
                mf.InistrnSum = strain;
                mf.AliveSum = alive;
            }
            for (int i = 0; i < mf.Nodes.Count() * 3; i++)      // 导出位移数据
                mf.DispSum[i, Loadcase.CurLCNo - 1] = Disp[i];
            for (int i = 0; i < mf.Elements.Count(); i++)       // 导出初应变数据
                mf.InistrnSum[i, Loadcase.CurLCNo - 1] = mf.Elements[i].RealConstant.IniStrn;
            for (int i = 0; i < mf.Elements.Count(); i++)       // 导出单元存活情况
                mf.AliveSum[i, Loadcase.CurLCNo - 1] = Convert.ToInt32(mf.Elements[i].isAlive);
            for (int i = 0; i < mf.Supports.Count(); i++)       // 导出千斤顶累计行程
                mf.JackStrokeSum[i, Loadcase.CurLCNo - 1] = mf.JackStrokeCurrent[i];
        }
        private void GlobalPSO()                 // 全局粒子群优化算法
        {
            if (Loadcase.AdjSupIndex.Count() != 0)
            {
                Matrix<double> ForceCohMat = GetCohMat();

                // 打印影响矩阵
                Console.WriteLine("=== 影响矩阵 ForceCohMat ===");
                for (int i = 0; i < ForceCohMat.RowCount; i++)
                {
                    Console.Write($"行{i}: [");
                    for (int j = 0; j < ForceCohMat.ColumnCount; j++)
                    {
                        Console.Write($"{ForceCohMat[i, j]:F2}");
                        if (j < ForceCohMat.ColumnCount - 1) Console.Write(", ");
                    }
                    Console.WriteLine("]");
                }

                    // 计算当前支撑轴力及优化上下限
                    Vector<double> ForceMax = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());         // 设计变量（可调支撑轴力）上下限
                    Vector<double> ForceMin = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                    Vector<double> Force0 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                    for (int i = 0; i < Loadcase.AdjSupIndex.Count(); i++)
                    {
                        mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[i]]].getNodalForce();
                        Force0[i] = -mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[i]]].jFx;  // 单位：N

                        // 最大轴力（受压），单位：N
                        ForceMax[i] = -Force0[i] + mf.Supports[Loadcase.AdjSupIndex[i]].MaxFC / mf.Supports[Loadcase.AdjSupIndex[i]].HrzDist;

                        // 最小轴力（受压）：20kN 或 Force0绝对值的10%，取较大者
                        double minForce1 = 50 * 1e3;  // 50kN = 50000N
                        double minForce2 = Math.Abs(Force0[i]) * 0.2;  // Force0绝对值的20%
                        double minForceRequired = Math.Max(minForce1, minForce2);  // 单位：N

                        ForceMin[i] = -Force0[i] + minForceRequired;  // 调整量，单位：N

                        // 数值保护：检查是否有异常值
                        if (double.IsNaN(Force0[i]) || double.IsInfinity(Force0[i]) || 
                            double.IsNaN(ForceMax[i]) || double.IsInfinity(ForceMax[i]) ||
                            double.IsNaN(ForceMin[i]) || double.IsInfinity(ForceMin[i]))
                        {
                            stageAdjustmentSucceeded = false;
                            mf.PrintString("全局粒子群算法计算出现数值异常，请检查模型参数！");
                            return;
                        }
                    }

                    // 打印Force0、ForceMax、ForceMin 
                    Console.WriteLine("=== 支撑轴力范围 ===");
                    Console.Write("Force0 (当前轴力): [");
                    for (int i = 0; i < Force0.Count; i++)
                    {
                        Console.Write($"{Force0[i]:F5}");
                        if (i < Force0.Count - 1) Console.Write(", ");
                    }
                    Console.WriteLine("]");

                    Console.Write("ForceMin (最小调整量): [");
                    for (int i = 0; i < ForceMin.Count; i++)
                    {
                        Console.Write($"{ForceMin[i]:F2}");
                        if (i < ForceMin.Count - 1) Console.Write(", ");
                    }
                    Console.WriteLine("]");

                    Console.Write("ForceMax (最大调整量): [");
                    for (int i = 0; i < ForceMax.Count; i++)
                    {
                        Console.Write($"{ForceMax[i]:F2}");
                        if (i < ForceMax.Count - 1) Console.Write(", ");
                    }
                    Console.WriteLine("]");

                    // 粒子群参数
                int PtCount = Loadcase.AdjSupIndex.Count() * 3;           // 粒子数
                int IterCount = Loadcase.AdjSupIndex.Count() * 30;        // 迭代次数
                double OmegaMax = 0.9;           // 最大惯性权重
                double OmegaMin = 0.4;           // 最小惯性权重
                double Omega;                    // 惯性权重，按照线性动态权重计算
                double c1 = 2.0;                 // 粒子对个人最佳位置的信心参数
                double c2 = 2.0;                 // 粒子对群体最佳位置的信心参数
                double dt = 1.0;                 // 时间步长
                double FunGbest;                 // 粒子全局最优解对应的函数值
                Random rd = new Random();        // 定义随机类，用于后续生成随机数
                Matrix<double> Force = Matrix<double>.Build.Dense(Loadcase.AdjSupIndex.Count(), PtCount);   // 粒子位置（可调支撑轴力）
                Matrix<double> Pbest = Matrix<double>.Build.Dense(Loadcase.AdjSupIndex.Count(), PtCount);   // 粒子个体最优解
                Matrix<double> Vel = Matrix<double>.Build.Dense(Loadcase.AdjSupIndex.Count(), PtCount);     // 粒子当前速度
                Vector<double> FunPbest = Vector<double>.Build.Dense(PtCount);                     // 各个粒子个体最优解对应的函数值
                Vector<double> Gbest = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());            // 粒子全局最优解
                Vector<double> VelMax = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());           // 粒子速度上限
                Vector<double> VelMin = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());           // 粒子速度下限

                for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)                       // 计算粒子速度上下限
                {
                    VelMax[iForce] = ForceMax[iForce] - ForceMin[iForce];
                    VelMin[iForce] = -VelMax[iForce];
                }

                for (int iPt = 0; iPt < PtCount; iPt++)                                            // 粒子位置和速度初始化
                {
                    Vector<double> ForceIni1 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                    Vector<double> ForceIni2 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                    
                    int maxAttempts = 1000;  // 最大尝试次数，防止死循环
                    int attemptCount = 0; //当前尝试次数
                    bool foundFeasible = false; // 是否找到可行解的标志
                    
                    while (attemptCount < maxAttempts)
                    {
                        attemptCount++;

                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)      // 对中法生成一对粒子初始位置
                        {
                            ForceIni1[iForce] = rd.NextDouble() * (ForceMax[iForce] - ForceMin[iForce]) + ForceMin[iForce];
                            ForceIni2[iForce] = ForceMax[iForce] + ForceMin[iForce] - ForceIni1[iForce];
                        }

                        // 随机初始值冲突检查 & 个体最优初始化
                        double f1, f2;
                        if (FEM.Check(ForceIni1, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, Loadcase.AdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                        {
                            f1 = FEM.GetDispNormInf(mf.Nodes);
                            if (FEM.Check(ForceIni2, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, Loadcase.AdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                            {
                                f2 = FEM.GetDispNormInf(mf.Nodes);
                                if (f1 < f2)
                                {
                                    for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                                    {
                                        Force[iForce, iPt] = ForceIni1[iForce];
                                        Pbest[iForce, iPt] = ForceIni1[iForce];
                                    }
                                    FunPbest[iPt] = f1;
                                }
                                else
                                {
                                    for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                                    {
                                        Force[iForce, iPt] = ForceIni2[iForce];
                                        Pbest[iForce, iPt] = ForceIni2[iForce];
                                    }
                                    FunPbest[iPt] = f2;
                                }
                            }
                            else
                            {
                                for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                                {
                                    Force[iForce, iPt] = ForceIni1[iForce];
                                    Pbest[iForce, iPt] = ForceIni1[iForce];
                                }
                                FunPbest[iPt] = f1;
                            }
                            foundFeasible = true;
                            break;
                        }
                        else if (FEM.Check(ForceIni2, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, Loadcase.AdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                        {
                            f2 = FEM.GetDispNormInf(mf.Nodes);
                            for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                            {
                                Force[iForce, iPt] = ForceIni2[iForce];
                                Pbest[iForce, iPt] = ForceIni2[iForce];
                            }
                            FunPbest[iPt] = f2;
                            foundFeasible = true;
                            break;
                        }
                    }
                    // 数值保护：检查是否成功找到可行解
                    if (!foundFeasible)
                    {
                        stageAdjustmentSucceeded = false;
                        mf.PrintString($"警告：粒子 {iPt + 1} 在 {maxAttempts} 次尝试后未找到可行初始位置，算法终止！");
                        return;
                    }
                    
                    for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)         // 粒子速度初始化
                        Vel[iForce, iPt] = rd.NextDouble() * (VelMax[iForce] - VelMin[iForce]) + VelMin[iForce];
                }

                FunGbest = FunPbest[0];                                                  // 粒子全局最优初始化
                for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                    Gbest[iForce] = Pbest[iForce, 0];
                for (int iPt = 0; iPt < PtCount; iPt++)
                {
                    if (FunPbest[iPt] < FunGbest)
                    {
                        FunGbest = FunPbest[iPt];
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                            Gbest[iForce] = Pbest[iForce, iPt];
                    }
                }

                for (int IterStep = 0; IterStep < IterCount; IterStep++)                 // 粒子更新过程
                {
                    Omega = OmegaMax - (OmegaMax - OmegaMin) / IterCount * IterStep;     // 按照线性动态权重计算惯性权重
                    for (int iPt = 0; iPt < PtCount; iPt++)
                    {
                        Vector<double> r1 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                        Vector<double> r2 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)     // 生成随机方向向量
                        {
                            r1[iForce] = rd.NextDouble();
                            r2[iForce] = rd.NextDouble();
                        }
                        //for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)     // 方向向量归一化
                        //{
                        //    r1[iForce] /= r1.Norm(2);
                        //    r2[iForce] /= r2.Norm(2);
                        //}
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)     // 更新速度
                        {
                            double newVel = Omega * Vel[iForce, iPt] + c1 * r1[iForce] * (Pbest[iForce, iPt] - Force[iForce, iPt]) / dt + c2 * r2[iForce] * (Gbest[iForce] - Force[iForce, iPt]) / dt;
                            
                            // 数值保护：检查速度更新是否产生异常值
                            if (double.IsNaN(newVel) || double.IsInfinity(newVel))
                            {
                                stageAdjustmentSucceeded = false;
                                mf.PrintString("粒子群算法速度更新出现数值异常，算法终止！");
                                return;
                            }
                            
                            Vel[iForce, iPt] = newVel;
                            if (Vel[iForce, iPt] > VelMax[iForce])
                                Vel[iForce, iPt] = VelMax[iForce];
                            else if (Vel[iForce, iPt] < VelMin[iForce])
                                Vel[iForce, iPt] = VelMin[iForce];
                        }
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)     // 更新位置
                        {
                            double newForce = Force[iForce, iPt] + Vel[iForce, iPt] * dt;
                            
                            // 数值保护：检查位置更新是否产生异常值
                            if (double.IsNaN(newForce) || double.IsInfinity(newForce))
                            {
                                stageAdjustmentSucceeded = false;
                                mf.PrintString("粒子群算法位置更新出现数值异常，算法终止！");
                                return;
                            }
                            
                            Force[iForce, iPt] = newForce;
                            if (Force[iForce, iPt] > ForceMax[iForce])
                            {
                                Force[iForce, iPt] = ForceMax[iForce];
                                Vel[iForce, iPt] = (ForceMax[iForce] - Force[iForce, iPt]) / dt;
                            }
                            else if (Force[iForce, iPt] < ForceMin[iForce])
                            {
                                Force[iForce, iPt] = ForceMin[iForce];
                                Vel[iForce, iPt] = (ForceMin[iForce] - Force[iForce, iPt]) / dt;
                            }
                        }

                        // 拷贝得到当前粒子各轴力
                        Vector<double> ForceIni = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                            ForceIni[iForce] = Force[iForce, iPt];

                        // 当前轴力冲突检验
                        double f;
                        if (FEM.Check(ForceIni, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, Loadcase.AdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                        {
                            f = FEM.GetDispNormInf(mf.Nodes);
                            if (f < FunPbest[iPt])     // 校核是否优于个体历史最优
                            {
                                FunPbest[iPt] = f;
                                for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                                    Pbest[iForce, iPt] = ForceIni[iForce];
                            }
                            if (f < FunGbest)          // 校核是否优于全局历史最优
                            {
                                FunGbest = f;
                                for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                                    Gbest[iForce] = ForceIni[iForce];
                            }
                        }
                        else
                        {
                            for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)     // 更新位置
                                Force[iForce, iPt] -= Vel[iForce, iPt] * dt;
                        }
                    }


                    Vector<double> tempJackStrokeIncrement = ForceCohMat.Solve(Gbest);
                    Vector<double> tempJackStrokeCandidate = mf.JackStrokeCurrent.Clone();
                    for (int i = 0; i < Loadcase.AdjSupIndex.Count(); i++)
                        tempJackStrokeCandidate[Loadcase.AdjSupIndex[i]] += tempJackStrokeIncrement[i];
                    Vector<double> tempPrescribedDisp = FEM.GetJackPrescribedDisp(
                        mf.Elements, CM_Elem_Supports, tempJackStrokeCandidate,
                        mf.Nodes.Count * 3);
                    SolveWithMode(
                        mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                        tempPrescribedDisp, out Disp, out RForce);
                    for (int i = 0; i < mf.Elements.Count(); i++)
                        mf.Elements[i].getNodalForce();


                    Iter++;
                    StreamWriter sw;
                    if (Iter == 1)
                    {
                        sw = new StreamWriter(Application.StartupPath + "\\支撑轴力迭代中间数据.txt", false, Encoding.UTF8);      // 第一次迭代，覆盖数据文件
                        sw.WriteLine("==============================================");
                        sw.WriteLine("        支撑轴力迭代中间数据");
                        sw.WriteLine("        导出时间：" + DateTime.Now.ToString());
                        sw.WriteLine("==============================================\r\n\r\n");
                        sw.WriteLine("LC  Iter   Obj(mm)     F1 (N)        F2 (N)        F3 (N)        F4 (N)        F5 (N)        F6 (N)      maxM (N*m)    minM (N*m)     maxQ (N)      minQ (N)");
                        sw.WriteLine("--  ----   -------  ------------  ------------  ------------  ------------  ------------  ------------  ------------  ------------  ------------  ------------");
                    }
                    else
                        sw = new StreamWriter(Application.StartupPath + "\\支撑轴力迭代中间数据.txt", true, Encoding.UTF8);       // 之后追加行

                    // sw.WriteLine("{0,2}{1,6}{2,10:f4}{3,14:e4}{4,14:e4}{5,14:e4}{6,14:e4}{7,14:e4}{8,14:e4}{9,14:e4}{10,14:e4}{11,14:e4}{12,14:e4}",
                    //     Loadcase.CurLCNo, Iter, FunGbest * 1e3, mf.Elements[CM_Elem_Supports[0]].iFx, mf.Elements[CM_Elem_Supports[1]].iFx, mf.Elements[CM_Elem_Supports[2]].iFx, mf.Elements[CM_Elem_Supports[3]].iFx, mf.Elements[CM_Elem_Supports[4]].iFx, mf.Elements[CM_Elem_Supports[5]].iFx,
                    //     mf.Elements.Max(o => o.iMom), mf.Elements.Min(o => o.iMom), mf.Elements.Max(o => o.iFy), mf.Elements.Min(o => o.iFy));

                    // 动态处理支撑数量的版本
                    string line = string.Format("{0,2}{1,6}{2,10:f4}", Loadcase.CurLCNo, Iter, FunGbest * 1e3);

                    // 输出支撑轴力（最多6个，不足的用空格填充）
                    for (int i = 0; i < 6; i++)
                    {
                        if (i < CM_Elem_Supports.Count)
                        {
                            line += string.Format("{0,14:e4}", mf.Elements[CM_Elem_Supports[i]].iFx);
                        }
                        else
                        {
                            line += string.Format("{0,14}", "");  // 不足6个支撑时用空格填充
                        }
                    }

                    // 输出弯矩和剪力的最大最小值
                    line += string.Format("{0,14:e4}{1,14:e4}{2,14:e4}{3,14:e4}",
                        mf.Elements.Max(o => o.iMom), mf.Elements.Min(o => o.iMom),
                        mf.Elements.Max(o => o.iFy), mf.Elements.Min(o => o.iFy));

                    sw.WriteLine(line);

                    sw.Close();

                }

                // 搜索结束，检查搜索结果：若满足条件，给出各个轴力；若不满足，在屏幕上打出提示
                //if (MinDef < EpsDefor * (ElevOfGround - curElev) && flag)
                Vector<double> JackStrokeCandidate;
                if (FEM.Check(Gbest, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, Loadcase.AdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out JackStrokeCandidate, out Disp, out RForce))
                {
                    Vector<double> JackStrokeIncrement = ForceCohMat.Solve(Gbest);
                    Vector<double> JackStrokeBefore = mf.JackStrokeCurrent.Clone();
                    // 合理轴力结果输出
                    mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 轴力主动调节完成！");
                    
                    Invoke(new Action(() =>
                    {
                        int[] summaryWidths = { 8, 14, 14, 14, 12, 12, 12, 12, 16, 16 };
                        mf.rtbOutputWindow.Text += FormatOutputTableRow(
                            new[] { "支撑编号", "初始轴力(kN/m)", "理想轴力(kN/m)", "轴力增量(kN/m)",
                                    "行程增量(mm)", "调前累计(mm)", "调后累计(mm)", "行程上限(mm)",
                                    "接触点位移(mm)", "支撑压缩量(mm)" },
                            summaryWidths, false);
                        mf.rtbOutputWindow.Text += FormatOutputSeparator(summaryWidths);
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                        {
                            int supportIndex = Loadcase.AdjSupIndex[iForce];
                            mf.rtbOutputWindow.Text += FormatOutputTableRow(
                                new[]
                                {
                                    (supportIndex + 1).ToString("0"),
                                    (Force0[iForce] * 1e-3).ToString("F2"),
                                    ((Force0[iForce] + Gbest[iForce]) * 1e-3).ToString("F2"),
                                    (Gbest[iForce] * 1e-3).ToString("F2"),
                                    (JackStrokeIncrement[iForce] * 1e3).ToString("F3"),
                                    (JackStrokeBefore[supportIndex] * 1e3).ToString("F3"),
                                    (JackStrokeCandidate[supportIndex] * 1e3).ToString("F3"),
                                    (mf.Supports[supportIndex].JackStrokeMax * 1e3).ToString("F3"),
                                    GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                                    GetSupportCompressionMm(supportIndex).ToString("F3")
                                },
                                summaryWidths, true);
                        }

                        // 轴力调整过程计算及输出
                        int[] processWidths = { 4, 8, 18, 12, 12, 16, 16 };
                        mf.rtbOutputWindow.Text += "\r\n千斤顶逐根调节过程：\r\n";
                        mf.rtbOutputWindow.Text += FormatOutputTableRow(
                            new[] { "次序", "支撑编号", "轴力变化(kN/m)", "行程增量(mm)", "累计行程(mm)",
                                    "接触点位移(mm)", "支撑压缩量(mm)" },
                            processWidths, false);
                        mf.rtbOutputWindow.Text += FormatOutputSeparator(processWidths);

                        Vector<double> Force1 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                        Vector<double> Force2 = Vector<double>.Build.Dense(Loadcase.AdjSupIndex.Count());
                        SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                        {
                            mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[iForce]]].getNodalForce();
                            Force1[iForce] = -mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[iForce]]].jFx;
                        }
                        Vector<double> JackStrokeWorking = JackStrokeBefore.Clone();
                        for (int iForce = 0; iForce < Loadcase.AdjSupIndex.Count(); iForce++)
                        {
                            int j = Loadcase.AdjSupIndex.Count() - iForce - 1;
                            int supportIndex = Loadcase.AdjSupIndex[j];
                            JackStrokeWorking[supportIndex] = JackStrokeCandidate[supportIndex];
                            Vector<double> PrescribedDisp = FEM.GetJackPrescribedDisp(
                                mf.Elements, CM_Elem_Supports, JackStrokeWorking,
                                mf.Nodes.Count * 3);
                            SolveWithMode(
                                mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                                PrescribedDisp, out Disp, out RForce);
                            UpdateEnvData();
                            for (int i = 0; i < Loadcase.AdjSupIndex.Count(); i++)
                            {
                                mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[i]]].getNodalForce();
                                Force2[i] = -mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[i]]].jFx;
                            }

                            mf.rtbOutputWindow.Text += FormatOutputTableRow(
                                new[]
                                {
                                    (iForce + 1).ToString("0"),
                                    (supportIndex + 1).ToString("0"),
                                    $"{Force1[j] * 1e-3:F2} => {Force2[j] * 1e-3:F2}",
                                    (JackStrokeIncrement[j] * 1e3).ToString("F3"),
                                    (JackStrokeWorking[supportIndex] * 1e3).ToString("F3"),
                                    GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                                    GetSupportCompressionMm(supportIndex).ToString("F3")
                                },
                                processWidths, true);

                            for (int i = 0; i < Loadcase.AdjSupIndex.Count(); i++)
                                Force1[i] = Force2[i];
                        }

                        mf.JackStrokeCurrent = JackStrokeCandidate.Clone();
                        SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

                        double MinDef = FEM.GetDispNormInf(mf.Nodes);
                        if (MinDef < mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev))
                            mf.PrintString("围护结构最大位移" + (MinDef * 1e3).ToString("0.00") + "mm，小于限值" + (mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev) * 1e3).ToString("0.00") + "mm。");
                        else
                            mf.PrintString("围护结构最大位移" + (MinDef * 1e3).ToString("0.00") + "mm，超过限值" + (mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev) * 1e3).ToString("0.00") + "mm。");

                        mf.rtbOutputWindow.Text += "\r\n\r\n";
                        mf.rtbOutputWindow.SelectionStart = mf.rtbOutputWindow.Text.Length;
                        mf.rtbOutputWindow.ScrollToCaret();
                    }));

                }
                else
                {
                    stageAdjustmentSucceeded = false;
                    mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 轴力主动调节失败，请检查输入。");
                }
            }
            else
            {
                SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 计算完成，无需进行轴力主动调节！");
            }
        }
        private void PartitionPSO()              // 分区粒子群优化算法
        {
            if (Loadcase.AdjSupIndex.Count() != 0)
            {
                // 获取分区支撑索引，该索引集合为原可调支撑（钢支撑）索引集合的子集。分区以混凝土支撑为界。
                List<int> PartAdjSupIndex = new List<int>();
                int tempInt = Loadcase.AdjSupIndex.Last();
                while (Loadcase.AdjSupIndex.Contains(tempInt))
                {
                    PartAdjSupIndex = PartAdjSupIndex.Prepend(tempInt).ToList();
                    tempInt--;
                    if (tempInt < 0)
                        break;
                }

                // 获取影响矩阵
                Matrix<double> tempForceCohMat = GetCohMat();
                Matrix<double> ForceCohMat = Matrix<double>.Build.Dense(PartAdjSupIndex.Count(), PartAdjSupIndex.Count(), 0.0);
                for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                    for (int j = 0; j < PartAdjSupIndex.Count(); j++)
                        ForceCohMat[PartAdjSupIndex.Count() - i - 1, PartAdjSupIndex.Count() - j - 1] = tempForceCohMat[Loadcase.AdjSupIndex.Count() - i - 1, Loadcase.AdjSupIndex.Count() - j - 1];

                // 打印影响矩阵
                Console.WriteLine("=== 影响矩阵 ForceCohMat (分区) ===");
                for (int i = 0; i < ForceCohMat.RowCount; i++)
                {
                    Console.Write($"行{i}: [");
                    for (int j = 0; j < ForceCohMat.ColumnCount; j++)
                    {
                        Console.Write($"{ForceCohMat[i, j]:F2}");
                        if (j < ForceCohMat.ColumnCount - 1) Console.Write(", ");
                    }
                    Console.WriteLine("]");
                }

                // 计算当前支撑轴力及优化上下限
                Vector<double> ForceMax = Vector<double>.Build.Dense(PartAdjSupIndex.Count());         // 设计变量（可调支撑轴力）上下限
                Vector<double> ForceMin = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                Vector<double> Force0 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                {
                    mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].getNodalForce();
                    Force0[i] = -mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].jFx;  // 单位：N

                    // 最大轴力（受压），单位：N
                    ForceMax[i] = -Force0[i] + mf.Supports[PartAdjSupIndex[i]].MaxFC / mf.Supports[PartAdjSupIndex[i]].HrzDist;

                    // 最小轴力（受压）：20kN 或 Force0绝对值的10%，取较大者
                    double minForce1 = 20 * 1e3;  // 20kN = 20000N
                    double minForce2 = Math.Abs(Force0[i]) * 0.1;  // Force0绝对值的10%
                    double minForceRequired = Math.Max(minForce1, minForce2);  // 单位：N

                    ForceMin[i] = -Force0[i] + minForceRequired;  // 调整量，单位：N

                    // 旧的计算方法（已废弃）
                    //ForceMin[i] = -Force0[i] - mf.Supports[PartAdjSupIndex[i]].MaxFT / mf.Supports[PartAdjSupIndex[i]].HrzDist;
                    //ForceMin[i] = -0.9 * Force0[i];     // 这里假设最小轴力为当前轴力的10%，实际工程中应根据支撑特性合理设置

                    // 数值保护：检查是否有异常值
                    if (double.IsNaN(Force0[i]) || double.IsInfinity(Force0[i]) ||
                        double.IsNaN(ForceMax[i]) || double.IsInfinity(ForceMax[i]) ||
                        double.IsNaN(ForceMin[i]) || double.IsInfinity(ForceMin[i]))
                    {
                        stageAdjustmentSucceeded = false;
                        mf.PrintString("分区粒子群算法计算出现数值异常，请检查模型参数！");
                        return;
                    }
                }

                // 打印Force0、ForceMax、ForceMin
                Console.WriteLine("=== 支撑轴力范围 (分区) ===");
                Console.Write("Force0 (当前轴力): [");
                for (int i = 0; i < Force0.Count; i++)
                {
                    Console.Write($"{Force0[i]:F5}");
                    if (i < Force0.Count - 1) Console.Write(", ");
                }
                Console.WriteLine("]");

                Console.Write("ForceMin (最小调整量): [");
                for (int i = 0; i < ForceMin.Count; i++)
                {
                    Console.Write($"{ForceMin[i]:F2}");
                    if (i < ForceMin.Count - 1) Console.Write(", ");
                }
                Console.WriteLine("]");

                Console.Write("ForceMax (最大调整量): [");
                for (int i = 0; i < ForceMax.Count; i++)
                {
                    Console.Write($"{ForceMax[i]:F2}");
                    if (i < ForceMax.Count - 1) Console.Write(", ");
                }
                Console.WriteLine("]");

                // 粒子群参数
                int PtCount = PartAdjSupIndex.Count() * 3;           // 粒子数
                int IterCount = PartAdjSupIndex.Count() * 20;        // 迭代次数
                double OmegaMax = 0.9;           // 最大惯性权重
                double OmegaMin = 0.4;           // 最小惯性权重
                double Omega;                    // 惯性权重，按照线性动态权重计算
                double c1 = 2.0;                 // 粒子对个人最佳位置的信心参数
                double c2 = 2.0;                 // 粒子对群体最佳位置的信心参数
                double dt = 1.0;                 // 时间步长
                double FunGbest;                 // 粒子全局最优解对应的函数值
                Random rd = new Random();        // 定义随机类，用于后续生成随机数
                Matrix<double> Force = Matrix<double>.Build.Dense(PartAdjSupIndex.Count(), PtCount);   // 粒子位置（可调支撑轴力）
                Matrix<double> Pbest = Matrix<double>.Build.Dense(PartAdjSupIndex.Count(), PtCount);   // 粒子个体最优解
                Matrix<double> Vel = Matrix<double>.Build.Dense(PartAdjSupIndex.Count(), PtCount);     // 粒子当前速度
                Vector<double> FunPbest = Vector<double>.Build.Dense(PtCount);                         // 各个粒子个体最优解对应的函数值
                Vector<double> Gbest = Vector<double>.Build.Dense(PartAdjSupIndex.Count());            // 粒子全局最优解
                Vector<double> VelMax = Vector<double>.Build.Dense(PartAdjSupIndex.Count());           // 粒子速度上限
                Vector<double> VelMin = Vector<double>.Build.Dense(PartAdjSupIndex.Count());           // 粒子速度下限

                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)                       // 计算粒子速度上下限
                {
                    VelMax[iForce] = ForceMax[iForce] - ForceMin[iForce];
                    VelMin[iForce] = -VelMax[iForce];
                }

                for (int iPt = 0; iPt < PtCount; iPt++)                                            // 粒子位置和速度初始化
                {
                    Vector<double> ForceIni1 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                    Vector<double> ForceIni2 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());

                    int maxAttempts = 1000;  // 最大尝试次数，防止死循环
                    int attemptCount = 0; //当前尝试次数
                    bool foundFeasible = false; // 是否找到可行解的标志

                    while (attemptCount < maxAttempts)
                    {
                        attemptCount++;

                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)      // 对中法生成一对粒子初始位置
                        {
                            ForceIni1[iForce] = rd.NextDouble() * (ForceMax[iForce] - ForceMin[iForce]) + ForceMin[iForce];
                            ForceIni2[iForce] = ForceMax[iForce] + ForceMin[iForce] - ForceIni1[iForce];
                        }

                        // 随机初始值冲突检查 & 个体最优初始化
                        double f1, f2;
                        if (FEM.Check(ForceIni1, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, PartAdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                        {
                            f1 = FEM.GetDispNormInf(mf.Nodes);
                            if (FEM.Check(ForceIni2, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, PartAdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                            {
                                f2 = FEM.GetDispNormInf(mf.Nodes);
                                if (f1 < f2)
                                {
                                    for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                                    {
                                        Force[iForce, iPt] = ForceIni1[iForce];
                                        Pbest[iForce, iPt] = ForceIni1[iForce];
                                    }
                                    FunPbest[iPt] = f1;
                                }
                                else
                                {
                                    for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                                    {
                                        Force[iForce, iPt] = ForceIni2[iForce];
                                        Pbest[iForce, iPt] = ForceIni2[iForce];
                                    }
                                    FunPbest[iPt] = f2;
                                }
                            }
                            else
                            {
                                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                                {
                                    Force[iForce, iPt] = ForceIni1[iForce];
                                    Pbest[iForce, iPt] = ForceIni1[iForce];
                                }
                                FunPbest[iPt] = f1;
                            }
                            foundFeasible = true;
                            break;
                        }
                        else if (FEM.Check(ForceIni2, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, PartAdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                        {
                            f2 = FEM.GetDispNormInf(mf.Nodes);
                            for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                            {
                                Force[iForce, iPt] = ForceIni2[iForce];
                                Pbest[iForce, iPt] = ForceIni2[iForce];
                            }
                            FunPbest[iPt] = f2;
                            foundFeasible = true;
                            break;
                        }
                    }

                    if (!foundFeasible)
                    {
                        mf.PrintString($"警告：粒子 {iPt + 1} 初始化未找到可行解，已尝试 {maxAttempts} 次。可能需要调整约束条件或增加搜索范围。");
                        // 这里可以选择使用最后一次尝试的值，或者跳过该粒子
                    }

                    for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)     // 粒子速度初始化
                        Vel[iForce, iPt] = rd.NextDouble() * (VelMax[iForce] - VelMin[iForce]) + VelMin[iForce];
                }

                FunGbest = FunPbest[0];                                                  // 粒子全局最优初始化
                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                    Gbest[iForce] = Pbest[iForce, 0];
                for (int iPt = 0; iPt < PtCount; iPt++)
                {
                    if (FunPbest[iPt] < FunGbest)
                    {
                        FunGbest = FunPbest[iPt];
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                            Gbest[iForce] = Pbest[iForce, iPt];
                    }
                }

                for (int IterStep = 0; IterStep < IterCount; IterStep++)                 // 粒子更新过程
                {
                    Omega = OmegaMax - (OmegaMax - OmegaMin) / IterCount * IterStep;     // 按照线性动态权重计算惯性权重
                    for (int iPt = 0; iPt < PtCount; iPt++)
                    {
                        Vector<double> r1 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                        Vector<double> r2 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)     // 生成随机方向向量
                        {
                            r1[iForce] = rd.NextDouble();
                            r2[iForce] = rd.NextDouble();
                        }
                        //for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)     // 方向向量归一化
                        //{
                        //    r1[iForce] /= r1.Norm(2);
                        //    r2[iForce] /= r2.Norm(2);
                        //}
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)     // 更新速度
                        {
                            Vel[iForce, iPt] = Omega * Vel[iForce, iPt] + c1 * r1[iForce] * (Pbest[iForce, iPt] - Force[iForce, iPt]) / dt + c2 * r2[iForce] * (Gbest[iForce] - Force[iForce, iPt]) / dt;
                            if (Vel[iForce, iPt] > VelMax[iForce])
                                Vel[iForce, iPt] = VelMax[iForce];
                            else if (Vel[iForce, iPt] < VelMin[iForce])
                                Vel[iForce, iPt] = VelMin[iForce];
                        }
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)     // 更新位置
                        {
                            Force[iForce, iPt] += Vel[iForce, iPt] * dt;
                            if (Force[iForce, iPt] > ForceMax[iForce])
                            {
                                Force[iForce, iPt] = ForceMax[iForce];
                                Vel[iForce, iPt] = (ForceMax[iForce] - Force[iForce, iPt]) / dt;
                            }
                            else if (Force[iForce, iPt] < ForceMin[iForce])
                            {
                                Force[iForce, iPt] = ForceMin[iForce];
                                Vel[iForce, iPt] = (ForceMin[iForce] - Force[iForce, iPt]) / dt;
                            }
                        }

                        // 拷贝得到当前粒子各轴力
                        Vector<double> ForceIni = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                            ForceIni[iForce] = Force[iForce, iPt];

                        // 当前轴力冲突检验
                        double f;
                        if (FEM.Check(ForceIni, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, PartAdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out Disp, out RForce))
                        {
                            f = FEM.GetDispNormInf(mf.Nodes);
                            if (f < FunPbest[iPt])     // 校核是否优于个体历史最优
                            {
                                FunPbest[iPt] = f;
                                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                                    Pbest[iForce, iPt] = ForceIni[iForce];
                            }
                            if (f < FunGbest)          // 校核是否优于全局历史最优
                            {
                                FunGbest = f;
                                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                                    Gbest[iForce] = ForceIni[iForce];
                            }
                        }
                        else
                        {
                            for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)     // 更新位置
                                Force[iForce, iPt] -= Vel[iForce, iPt] * dt;
                        }
                    }
                }

                // 搜索结束，检查搜索结果：若满足条件，给出各个轴力；若不满足，在屏幕上打出提示
                //if (MinDef < EpsDefor * (ElevOfGround - curElev) && flag)
                Vector<double> PartJackStrokeCandidate;
                if (FEM.Check(Gbest, ForceCohMat, mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS, CM_Elem_ECS, CM_Elem_Supports, PartAdjSupIndex, mf.Elements, ref mf.Nodes, mf.Supports, Fs, ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent, out PartJackStrokeCandidate, out Disp, out RForce))
                {
                    Vector<double> JackStrokeIncrement = ForceCohMat.Solve(Gbest);
                    Vector<double> JackStrokeBefore = mf.JackStrokeCurrent.Clone();
                    // 合理轴力结果输出
                    mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 轴力主动调节完成！");
                    
                    Invoke(new Action(() =>
                    {
                        int[] summaryWidths = { 8, 14, 14, 14, 12, 12, 12, 12, 16, 16 };
                        mf.rtbOutputWindow.Text += FormatOutputTableRow(
                            new[] { "支撑编号", "初始轴力(kN/m)", "理想轴力(kN/m)", "轴力增量(kN/m)",
                                    "行程增量(mm)", "调前累计(mm)", "调后累计(mm)", "行程上限(mm)",
                                    "接触点位移(mm)", "支撑压缩量(mm)" },
                            summaryWidths, false);
                        mf.rtbOutputWindow.Text += FormatOutputSeparator(summaryWidths);
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                        {
                            int supportIndex = PartAdjSupIndex[iForce];
                            mf.rtbOutputWindow.Text += FormatOutputTableRow(
                                new[]
                                {
                                    (supportIndex + 1).ToString("0"),
                                    (Force0[iForce] * 1e-3).ToString("F2"),
                                    ((Force0[iForce] + Gbest[iForce]) * 1e-3).ToString("F2"),
                                    (Gbest[iForce] * 1e-3).ToString("F2"),
                                    (JackStrokeIncrement[iForce] * 1e3).ToString("F3"),
                                    (JackStrokeBefore[supportIndex] * 1e3).ToString("F3"),
                                    (PartJackStrokeCandidate[supportIndex] * 1e3).ToString("F3"),
                                    (mf.Supports[supportIndex].JackStrokeMax * 1e3).ToString("F3"),
                                    GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                                    GetSupportCompressionMm(supportIndex).ToString("F3")
                                },
                                summaryWidths, true);
                        }

                        // 轴力调整过程计算及输出
                        int[] processWidths = { 4, 8, 18, 12, 12, 16, 16 };
                        mf.rtbOutputWindow.Text += "\r\n千斤顶逐根调节过程：\r\n";
                        mf.rtbOutputWindow.Text += FormatOutputTableRow(
                            new[] { "次序", "支撑编号", "轴力变化(kN/m)", "行程增量(mm)", "累计行程(mm)",
                                    "接触点位移(mm)", "支撑压缩量(mm)" },
                            processWidths, false);
                        mf.rtbOutputWindow.Text += FormatOutputSeparator(processWidths);

                        Vector<double> Force1 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                        Vector<double> Force2 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                        SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                        {
                            mf.Elements[CM_Elem_Supports[PartAdjSupIndex[iForce]]].getNodalForce();
                            Force1[iForce] = -mf.Elements[CM_Elem_Supports[PartAdjSupIndex[iForce]]].jFx;
                        }
                        Vector<double> JackStrokeWorking = JackStrokeBefore.Clone();
                        for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                        {
                            int j = PartAdjSupIndex.Count() - iForce - 1;
                            int supportIndex = PartAdjSupIndex[j];
                            JackStrokeWorking[supportIndex] = PartJackStrokeCandidate[supportIndex];
                            Vector<double> PrescribedDisp = FEM.GetJackPrescribedDisp(
                                mf.Elements, CM_Elem_Supports, JackStrokeWorking,
                                mf.Nodes.Count * 3);
                            SolveWithMode(
                                mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                                PrescribedDisp, out Disp, out RForce);
                            UpdateEnvData();
                            for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                            {
                                mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].getNodalForce();
                                Force2[i] = -mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].jFx;
                            }

                            mf.rtbOutputWindow.Text += FormatOutputTableRow(
                                new[]
                                {
                                    (iForce + 1).ToString("0"),
                                    (supportIndex + 1).ToString("0"),
                                    $"{Force1[j] * 1e-3:F2} => {Force2[j] * 1e-3:F2}",
                                    (JackStrokeIncrement[j] * 1e3).ToString("F3"),
                                    (JackStrokeWorking[supportIndex] * 1e3).ToString("F3"),
                                    GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                                    GetSupportCompressionMm(supportIndex).ToString("F3")
                                },
                                processWidths, true);

                            for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                                Force1[i] = Force2[i];
                        }

                        mf.JackStrokeCurrent = PartJackStrokeCandidate.Clone();
                        SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

                        double MinDef = FEM.GetDispNormInf(mf.Nodes);
                        if (MinDef < mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev))
                            mf.PrintString("围护结构最大位移" + (MinDef * 1e3).ToString("0.00") + "mm，小于限值" + (mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev) * 1e3).ToString("0.00") + "mm。");
                        else
                            mf.PrintString("围护结构最大位移" + (MinDef * 1e3).ToString("0.00") + "mm，超过限值" + (mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev) * 1e3).ToString("0.00") + "mm。");

                        mf.rtbOutputWindow.Text += "\r\n\r\n";
                        mf.rtbOutputWindow.SelectionStart = mf.rtbOutputWindow.Text.Length;
                        mf.rtbOutputWindow.ScrollToCaret();
                    }));

                }
                else
                {
                    stageAdjustmentSucceeded = false;
                    mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 轴力主动调节失败，请检查输入。");
                }
            }
            else
            {
                SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 计算完成，无需进行轴力主动调节！");
            }
        }
        private void ZeroDisp()                  // 动态零位移法
        {
            if (Loadcase.AdjSupIndex.Count == 0)
            {
                SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                mf.PrintString("施工阶段 " + (Loadcase.CurLCNo).ToString("0") + " 计算完成，无需进行轴力主动调节！");
                return;
            }

            int adjustableCount = Loadcase.AdjSupIndex.Count;
            SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

            Vector<double> AdjustedVec = Vector<double>.Build.Dense(adjustableCount);
            Matrix<double> InfluenceMat = Matrix<double>.Build.Dense(
                adjustableCount, adjustableCount, 0.0);

            for (int i = 0; i < adjustableCount; i++)
            {
                int supportIndex = Loadcase.AdjSupIndex[i];
                AdjustedVec[i] = -mf.Elements[CM_Elem_Supports[supportIndex]].Right.Ux;
            }

            // 用1mm千斤顶试探行程建立“行程-墙体位移”影响矩阵。
            for (int i = 0; i < adjustableCount; i++)
            {
                int supportIndex = Loadcase.AdjSupIndex[i];
                double remainingStroke =
                    mf.Supports[supportIndex].JackStrokeMax - mf.JackStrokeCurrent[supportIndex];
                double probeStroke = remainingStroke > 1.0e-12
                    ? Math.Min(1.0e-3, remainingStroke)
                    : -Math.Min(1.0e-3, mf.JackStrokeCurrent[supportIndex]);

                if (Math.Abs(probeStroke) < 1.0e-12)
                {
                    stageAdjustmentSucceeded = false;
                    mf.PrintString($"动态零位移法失败：第{supportIndex + 1}根支撑没有可用千斤顶行程。");
                    return;
                }

                Vector<double> probeJackStroke = mf.JackStrokeCurrent.Clone();
                probeJackStroke[supportIndex] += probeStroke;
                Vector<double> PrescribedDisp = FEM.GetJackPrescribedDisp(
                    mf.Elements, CM_Elem_Supports, probeJackStroke,
                    mf.Nodes.Count * 3);
                SolveWithMode(
                    mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                    PrescribedDisp, out Disp, out RForce);

                for (int j = 0; j < adjustableCount; j++)
                {
                    int measuredSupportIndex = Loadcase.AdjSupIndex[j];
                    double probeUx =
                        mf.Elements[CM_Elem_Supports[measuredSupportIndex]].Right.Ux;
                    InfluenceMat[j, i] =
                        (probeUx + AdjustedVec[j]) / probeStroke;
                }
            }

            Vector<double> JackStrokeIncrement;
            try
            {
                JackStrokeIncrement = InfluenceMat.Solve(AdjustedVec);
            }
            catch (Exception ex)
            {
                stageAdjustmentSucceeded = false;
                mf.PrintString("动态零位移法反算千斤顶行程失败：" + ex.Message);
                return;
            }

            Vector<double> JackStrokeBefore = mf.JackStrokeCurrent.Clone();
            Vector<double> JackStrokeCandidate = mf.JackStrokeCurrent.Clone();
            for (int i = 0; i < adjustableCount; i++)
            {
                int supportIndex = Loadcase.AdjSupIndex[i];
                JackStrokeCandidate[supportIndex] += JackStrokeIncrement[i];

                if (JackStrokeCandidate[supportIndex] < -1.0e-9 ||
                    JackStrokeCandidate[supportIndex] >
                    mf.Supports[supportIndex].JackStrokeMax + 1.0e-9)
                {
                    stageAdjustmentSucceeded = false;
                    mf.PrintString(
                        $"动态零位移法失败：第{supportIndex + 1}根支撑累计行程" +
                        $"{JackStrokeCandidate[supportIndex] * 1e3:F3}mm超出允许范围。");
                    return;
                }

                JackStrokeCandidate[supportIndex] = Math.Max(
                    0.0,
                    Math.Min(
                        mf.Supports[supportIndex].JackStrokeMax,
                        JackStrokeCandidate[supportIndex]));
            }

            Vector<double> finalPrescribedDisp = FEM.GetJackPrescribedDisp(
                mf.Elements, CM_Elem_Supports, JackStrokeCandidate,
                mf.Nodes.Count * 3);
            SolveWithMode(
                mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                finalPrescribedDisp, out Disp, out RForce);
            mf.JackStrokeCurrent = JackStrokeCandidate.Clone();
            UpdateEnvData();

            mf.PrintString("施工阶段 " + Loadcase.CurLCNo.ToString("0") + " 动态零位移调节完成！");
            Invoke(new Action(() =>
            {
                int[] widths = { 8, 12, 12, 12, 12, 16, 16 };
                mf.rtbOutputWindow.Text += FormatOutputTableRow(
                    new[] { "支撑编号", "行程增量(mm)", "调前累计(mm)",
                            "调后累计(mm)", "行程上限(mm)",
                            "接触点位移(mm)", "支撑压缩量(mm)" },
                    widths, false);
                mf.rtbOutputWindow.Text += FormatOutputSeparator(widths);

                for (int i = 0; i < adjustableCount; i++)
                {
                    int supportIndex = Loadcase.AdjSupIndex[i];
                    mf.rtbOutputWindow.Text += FormatOutputTableRow(
                        new[]
                        {
                            (supportIndex + 1).ToString("0"),
                            (JackStrokeIncrement[i] * 1e3).ToString("F3"),
                            (JackStrokeBefore[supportIndex] * 1e3).ToString("F3"),
                            (JackStrokeCandidate[supportIndex] * 1e3).ToString("F3"),
                            (mf.Supports[supportIndex].JackStrokeMax * 1e3).ToString("F3"),
                            GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                            GetSupportCompressionMm(supportIndex).ToString("F3")
                        },
                        widths, true);
                }

                mf.rtbOutputWindow.Text += "\r\n";
                mf.rtbOutputWindow.SelectionStart = mf.rtbOutputWindow.Text.Length;
                mf.rtbOutputWindow.ScrollToCaret();
            }));
        }
        private void Manual(Vector<double> ManualForce)    // 手动赋值
        {
            List<int> PartAdjSupIndex = new List<int>();
            List<double> PartForce = new List<double>();
            for (int i = 0; i < ManualForce.Count(); i++)
            {
                if (ManualForce[i] < 1e11)
                {
                    PartAdjSupIndex.Add(Loadcase.AdjSupIndex[i]);
                    PartForce.Add(ManualForce[i]);
                }
            }

            if (PartAdjSupIndex.Count == 0)
                return;

            // 获取影响矩阵
            Matrix<double> tempForceCohMat = GetCohMat();
            Matrix<double> ForceCohMat = Matrix<double>.Build.Dense(
                PartAdjSupIndex.Count, PartAdjSupIndex.Count, 0.0);
            for (int i = 0; i < PartAdjSupIndex.Count(); i++)
            {
                int sourceRow = Loadcase.AdjSupIndex.IndexOf(PartAdjSupIndex[i]);
                for (int j = 0; j < PartAdjSupIndex.Count(); j++)
                {
                    int sourceColumn = Loadcase.AdjSupIndex.IndexOf(PartAdjSupIndex[j]);
                    ForceCohMat[i, j] = tempForceCohMat[sourceRow, sourceColumn];
                }
            }

            // 计算当前支撑轴力
            SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
            Vector<double> Force0 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
            for (int i = 0; i < PartAdjSupIndex.Count(); i++)
            {
                mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].getNodalForce();
                Force0[i] = -mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].jFx;
            }

            Vector<double> tempVec = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
            for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                tempVec[i] = PartForce[i] - Force0[i];

            Vector<double> ManualJackStrokeCandidate;
            if (!FEM.Check(
                tempVec, ForceCohMat,
                mf.MaxMommentOfECS1, mf.MaxMommentOfECS2, mf.MaxShearForceOfECS,
                CM_Elem_ECS, CM_Elem_Supports, PartAdjSupIndex,
                mf.Elements, ref mf.Nodes, mf.Supports, Fs,
                ConstrainedDOFIndex, Force0, mf.JackStrokeCurrent,
                out ManualJackStrokeCandidate, out Disp, out RForce))
            {
                stageAdjustmentSucceeded = false;
                mf.PrintString("手动轴力对应的千斤顶行程或结构内力不满足约束，未执行调节。");
                return;
            }

            Vector<double> JackStrokeIncrement = ForceCohMat.Solve(tempVec);
            Vector<double> JackStrokeBefore = mf.JackStrokeCurrent.Clone();

            Invoke(new Action(() =>
            {
                int[] summaryWidths = { 8, 14, 14, 14, 12, 12, 12, 12, 16, 16 };
                mf.rtbOutputWindow.Text += FormatOutputTableRow(
                    new[] { "支撑编号", "初始轴力(kN/m)", "理想轴力(kN/m)", "轴力增量(kN/m)",
                            "行程增量(mm)", "调前累计(mm)", "调后累计(mm)", "行程上限(mm)",
                            "接触点位移(mm)", "支撑压缩量(mm)" },
                    summaryWidths, false);
                mf.rtbOutputWindow.Text += FormatOutputSeparator(summaryWidths);
                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                {
                    int supportIndex = PartAdjSupIndex[iForce];
                    mf.rtbOutputWindow.Text += FormatOutputTableRow(
                        new[]
                        {
                            (supportIndex + 1).ToString("0"),
                            (Force0[iForce] * 1e-3).ToString("F2"),
                            (PartForce[iForce] * 1e-3).ToString("F2"),
                            (tempVec[iForce] * 1e-3).ToString("F2"),
                            (JackStrokeIncrement[iForce] * 1e3).ToString("F3"),
                            (JackStrokeBefore[supportIndex] * 1e3).ToString("F3"),
                            (ManualJackStrokeCandidate[supportIndex] * 1e3).ToString("F3"),
                            (mf.Supports[supportIndex].JackStrokeMax * 1e3).ToString("F3"),
                            GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                            GetSupportCompressionMm(supportIndex).ToString("F3")
                        },
                        summaryWidths, true);
                }

                int[] processWidths = { 4, 8, 18, 12, 12, 16, 16 };
                mf.rtbOutputWindow.Text += "\r\n千斤顶逐根调节过程：\r\n";
                mf.rtbOutputWindow.Text += FormatOutputTableRow(
                    new[] { "次序", "支撑编号", "轴力变化(kN/m)", "行程增量(mm)", "累计行程(mm)",
                            "接触点位移(mm)", "支撑压缩量(mm)" },
                    processWidths, false);
                mf.rtbOutputWindow.Text += FormatOutputSeparator(processWidths);

                Vector<double> Force1 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                Vector<double> Force2 = Vector<double>.Build.Dense(PartAdjSupIndex.Count());
                SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);
                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                {
                    mf.Elements[CM_Elem_Supports[PartAdjSupIndex[iForce]]].getNodalForce();
                    Force1[iForce] = -mf.Elements[CM_Elem_Supports[PartAdjSupIndex[iForce]]].jFx;
                }

                Vector<double> JackStrokeWorking = JackStrokeBefore.Clone();
                for (int iForce = 0; iForce < PartAdjSupIndex.Count(); iForce++)
                {
                    int j = PartAdjSupIndex.Count() - iForce - 1;
                    int supportIndex = PartAdjSupIndex[j];
                    JackStrokeWorking[supportIndex] = ManualJackStrokeCandidate[supportIndex];
                    Vector<double> PrescribedDisp = FEM.GetJackPrescribedDisp(
                        mf.Elements, CM_Elem_Supports, JackStrokeWorking,
                        mf.Nodes.Count * 3);
                    SolveWithMode(
                        mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                        PrescribedDisp, out Disp, out RForce);
                    UpdateEnvData();

                    for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                    {
                        mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].getNodalForce();
                        Force2[i] = -mf.Elements[CM_Elem_Supports[PartAdjSupIndex[i]]].jFx;
                    }

                    mf.rtbOutputWindow.Text += FormatOutputTableRow(
                        new[]
                        {
                            (iForce + 1).ToString("0"),
                            (supportIndex + 1).ToString("0"),
                            $"{Force1[j] * 1e-3:F2} => {Force2[j] * 1e-3:F2}",
                            (JackStrokeIncrement[j] * 1e3).ToString("F3"),
                            (JackStrokeWorking[supportIndex] * 1e3).ToString("F3"),
                            GetSupportContactDisplacementMm(supportIndex).ToString("F3"),
                            GetSupportCompressionMm(supportIndex).ToString("F3")
                        },
                        processWidths, true);

                    for (int i = 0; i < PartAdjSupIndex.Count(); i++)
                        Force1[i] = Force2[i];
                }

                mf.JackStrokeCurrent = ManualJackStrokeCandidate.Clone();
                SolveWithMode(mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex, out Disp, out RForce);

                double MinDef = FEM.GetDispNormInf(mf.Nodes);
                if (MinDef < mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev))
                    mf.PrintString("围护结构最大位移" + (MinDef * 1e3).ToString("0.00") + "mm，小于限值" + (mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev) * 1e3).ToString("0.00") + "mm。支撑轴力调整结果：");
                else
                    mf.PrintString("围护结构最大位移" + (MinDef * 1e3).ToString("0.00") + "mm，超过限值" + (mf.EpsDefor * (mf.ElevOfGround - Loadcase.CurElev) * 1e3).ToString("0.00") + "mm。支撑轴力调整结果：");

                mf.rtbOutputWindow.Text += "\r\n\r\n";
                mf.rtbOutputWindow.SelectionStart = mf.rtbOutputWindow.Text.Length;
                mf.rtbOutputWindow.ScrollToCaret();
            }));
        }
        private void UpdateEnvData()             // 更新包络数据
        {
            // 单元内力更新
            foreach (Element i in mf.Elements)
                i.getNodalForce();

            // 围护结构水平位移包络数据更新
            for (int iNode = 0; iNode < CM_Node_ECS.Count(); iNode++) 
            {
                if (mf.Nodes[CM_Node_ECS[iNode]].Ux > mf.UxMax[iNode])
                    mf.UxMax[iNode] = mf.Nodes[CM_Node_ECS[iNode]].Ux;
                if (mf.Nodes[CM_Node_ECS[iNode]].Ux < mf.UxMin[iNode])
                    mf.UxMin[iNode] = mf.Nodes[CM_Node_ECS[iNode]].Ux;
            }

            // 围护结构弯矩 & 剪力包络数据更新
            for (int iElem = 0; iElem < CM_Elem_ECS.Count(); iElem++) 
            {
                // 弯矩
                if (mf.Elements[CM_Elem_ECS[iElem]].iMom > mf.MomMax[iElem])
                    mf.MomMax[iElem] = mf.Elements[CM_Elem_ECS[iElem]].iMom;
                if (mf.Elements[CM_Elem_ECS[iElem]].iMom < mf.MomMin[iElem])
                    mf.MomMin[iElem] = mf.Elements[CM_Elem_ECS[iElem]].iMom;
                // 剪力
                if (mf.Elements[CM_Elem_ECS[iElem]].iFy > mf.FyMax[iElem])
                    mf.FyMax[iElem] = mf.Elements[CM_Elem_ECS[iElem]].iFy;
                if (mf.Elements[CM_Elem_ECS[iElem]].iFy < mf.FyMin[iElem])
                    mf.FyMin[iElem] = mf.Elements[CM_Elem_ECS[iElem]].iFy;
            }

            // 支撑轴力包络数据更新
            for (int iElem = 0; iElem < CM_Elem_Supports.Count(); iElem++)
            {
                if (mf.Elements[CM_Elem_Supports[iElem]].iFx > mf.FxMax[iElem])
                    mf.FxMax[iElem] = mf.Elements[CM_Elem_Supports[iElem]].iFx;
                if (mf.Elements[CM_Elem_Supports[iElem]].iFx < mf.FxMin[iElem])
                    mf.FxMin[iElem] = mf.Elements[CM_Elem_Supports[iElem]].iFx;
            }

        }

        /// <summary>
        /// 返回支撑与围护结构接触点的水平位移，单位mm。
        /// 支撑Right节点为围护结构一端，数值保留全局X方向的正负号。
        /// </summary>
        private double GetSupportContactDisplacementMm(int supportIndex)
        {
            Element supportElement = mf.Elements[CM_Elem_Supports[supportIndex]];
            return supportElement.Right.Ux * 1e3;
        }

        /// <summary>
        /// 返回支撑弹性压缩量，单位mm。
        /// 按 delta = N * L / (E * A) 计算；受压为正，受拉为负。
        /// </summary>
        private double GetSupportCompressionMm(int supportIndex)
        {
            Element supportElement = mf.Elements[CM_Elem_Supports[supportIndex]];
            supportElement.getNodalForce();

            double compressionForce = -supportElement.jFx;
            double elementLength = GetElementLength(
                supportElement.Left,
                supportElement.Right);
            double axialRigidity =
                supportElement.Material.Emodulus *
                supportElement.RealConstant.Area;

            if (Math.Abs(axialRigidity) < 1.0e-12)
                return 0.0;

            return compressionForce * elementLength / axialRigidity * 1e3;
        }

        /// <summary>
        /// 计算输出窗口中的文本显示宽度。
        /// 中文字符按2个半角字符计算，用于保证中英文混排时表格列对齐。
        /// </summary>
        private static int GetOutputDisplayWidth(string text)
        {
            int width = 0;
            foreach (char c in text ?? string.Empty)
                width += c > 255 ? 2 : 1;

            return width;
        }

        /// <summary>
        /// 按显示宽度填充单元格。
        /// </summary>
        private static string PadOutputCell(
            string text,
            int displayWidth,
            bool alignRight)
        {
            text = text ?? string.Empty;
            int padding = Math.Max(
                0,
                displayWidth - GetOutputDisplayWidth(text));

            return alignRight
                ? new string(' ', padding) + text
                : text + new string(' ', padding);
        }

        /// <summary>
        /// 生成一行固定显示宽度的输出表格。
        /// </summary>
        private static string FormatOutputTableRow(
            string[] cells,
            int[] widths,
            bool alignRight)
        {
            if (cells == null || widths == null || cells.Length != widths.Length)
                throw new ArgumentException("输出表格单元格数量与列宽数量不一致。");

            StringBuilder row = new StringBuilder("|");
            for (int i = 0; i < cells.Length; i++)
            {
                row.Append(PadOutputCell(cells[i], widths[i], alignRight));
                row.Append('|');
            }

            row.Append("\r\n");
            return row.ToString();
        }

        /// <summary>
        /// 生成与指定表格同宽的分隔线。
        /// </summary>
        private static string FormatOutputSeparator(int[] widths)
        {
            if (widths == null)
                throw new ArgumentNullException(nameof(widths));

            int length = widths.Sum() + widths.Length + 1;
            return new string('-', length) + "\r\n";
        }

        private Matrix<double> GetCohMat()       // 计算轴力相干性影响矩阵
        {
            int adjustableCount = Loadcase.AdjSupIndex.Count;
            Matrix<double> ForceCohMat = Matrix<double>.Build.Dense(
                adjustableCount, adjustableCount, 0.0);

            if (adjustableCount == 0)
                return ForceCohMat;

            for (int i = 0; i < adjustableCount; i++)
            {
                int supportIndex = Loadcase.AdjSupIndex[i];
                Element supportElement = mf.Elements[CM_Elem_Supports[supportIndex]];

                if (!supportElement.isAlive)
                {
                    mf.PrintString($"警告：第{supportIndex + 1}根可调支撑尚未激活，影响矩阵对应列置零。");
                    continue;
                }

                // 当前累计行程对应的基准轴力。
                Vector<double> basePrescribedDisp = FEM.GetJackPrescribedDisp(
                    mf.Elements, CM_Elem_Supports, mf.JackStrokeCurrent,
                    mf.Nodes.Count * 3);
                SolveWithMode(
                    mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                    basePrescribedDisp, out Disp, out RForce);

                Vector<double> jFx0 = Vector<double>.Build.Dense(adjustableCount);
                for (int j = 0; j < adjustableCount; j++)
                {
                    Element measuredElement =
                        mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[j]]];
                    measuredElement.getNodalForce();
                    jFx0[j] = measuredElement.jFx;
                }

                // 优先向顶出方向试探1mm；达到上限时改为回缩方向试探。
                double currentStroke = mf.JackStrokeCurrent[supportIndex];
                double remainingStroke =
                    mf.Supports[supportIndex].JackStrokeMax - currentStroke;
                double probeStroke;

                if (remainingStroke > 1.0e-12)
                {
                    probeStroke = Math.Min(1.0e-3, remainingStroke);
                }
                else if (currentStroke > 1.0e-12)
                {
                    probeStroke = -Math.Min(1.0e-3, currentStroke);
                }
                else
                {
                    mf.PrintString($"警告：第{supportIndex + 1}根支撑千斤顶没有可用行程，影响矩阵对应列置零。");
                    continue;
                }

                Vector<double> probeJackStroke = mf.JackStrokeCurrent.Clone();
                probeJackStroke[supportIndex] += probeStroke;
                Vector<double> probePrescribedDisp = FEM.GetJackPrescribedDisp(
                    mf.Elements, CM_Elem_Supports, probeJackStroke,
                    mf.Nodes.Count * 3);
                SolveWithMode(
                    mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                    probePrescribedDisp, out Disp, out RForce);

                for (int j = 0; j < adjustableCount; j++)
                {
                    Element measuredElement =
                        mf.Elements[CM_Elem_Supports[Loadcase.AdjSupIndex[j]]];
                    measuredElement.getNodalForce();
                    ForceCohMat[j, i] =
                        -(measuredElement.jFx - jFx0[j]) / probeStroke;
                }
            }

            // 试探结束后恢复到当前累计行程所对应的结构状态。
            Vector<double> restorePrescribedDisp = FEM.GetJackPrescribedDisp(
                mf.Elements, CM_Elem_Supports, mf.JackStrokeCurrent,
                mf.Nodes.Count * 3);
            SolveWithMode(
                mf.Elements, ref mf.Nodes, Fs, ConstrainedDOFIndex,
                restorePrescribedDisp, out Disp, out RForce);

            return ForceCohMat;
        }

        /// <summary>
        /// 计算两节点单元长度，土弹簧通常为水平 1m
        /// </summary>
        private static double GetElementLength(Node left, Node right)
        {
            double length = Math.Sqrt(Math.Pow(left.Nx - right.Nx, 2) + Math.Pow(left.Ny - right.Ny, 2));
            return length > 1e-6 ? length : 1.0;
        }

        /// <summary>
        /// 计算邓肯-张土弹簧物理初始刚度，返回值单位与有限元整体一致（N/m）
        /// </summary>
        private static double GetInitialDuncanChangSpringStiffness(SoilLayer soilLayer, double depth, double area, double springLength)
        {
            if (depth < 0.1)
                depth = 0.1;

            DuncanChangModel dcModel = DuncanChangModel.FromSoilLayer(soilLayer);
            double sigma3 = DuncanChangModel.EstimateConfiningStress(depth, soilLayer.Gamma, soilLayer.K0);
            return dcModel.GetSpringStiffness(sigma3, 0.0, area, springLength);
        }

        /// <summary>
        /// 统一的求解方法：根据Loadcase.UseNonlinearSoilSpring自动选择线性或非线性求解
        /// </summary>
        private void SolveWithMode(List<Element> Elements, ref List<Node> Nodes,
                                   Vector<double> Fs, List<int> ConstrainedDOFIndex,
                                   out Vector<double> Disp, out Vector<double> RForce)
        {
            Vector<double> PrescribedDisp = FEM.GetJackPrescribedDisp(
                Elements, CM_Elem_Supports, mf.JackStrokeCurrent,
                Nodes.Count * 3);

            SolveWithMode(
                Elements, ref Nodes, Fs, ConstrainedDOFIndex, PrescribedDisp,
                out Disp, out RForce);
        }

        /// <summary>
        /// 统一的求解方法，允许传入千斤顶端的非零规定位移。
        /// </summary>
        private void SolveWithMode(List<Element> Elements, ref List<Node> Nodes,
                                   Vector<double> Fs, List<int> ConstrainedDOFIndex,
                                   Vector<double> PrescribedDisp,
                                   out Vector<double> Disp, out Vector<double> RForce)
        {
            if (Loadcase.UseNonlinearSoilSpring)
            {
                // 非线性求解：使用邓肯-张模型迭代
                FEM.SolveNonlinear(Elements, ref Nodes, Fs, ConstrainedDOFIndex,
                                   CM_Elem_SoilSpring, mf.SoilLayers,
                                   Loadcase.CurElev, mf.ElevOfCollar, PrescribedDisp,
                                   out Disp, out RForce);
            }
            else
            {
                // 线性求解：刚度不变
                FEM.Solve(
                    Elements, ref Nodes, Fs, ConstrainedDOFIndex, PrescribedDisp,
                    out Disp, out RForce);
            }
        }

    }
}
