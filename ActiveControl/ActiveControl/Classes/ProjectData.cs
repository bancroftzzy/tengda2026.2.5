using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

namespace ActiveControl
{
    public enum CalculationMethod { Direct, GlobalPSO, PartitionPSO, ZeroDisp, Manual }

    public static class CalculationMethodNames
    {
        public static string Get(CalculationMethod method)
        {
            switch (method)
            {
                case CalculationMethod.Direct: return "直接计算";
                case CalculationMethod.GlobalPSO: return "全局粒子群";
                case CalculationMethod.PartitionPSO: return "分区粒子群";
                case CalculationMethod.ZeroDisp: return "零位移法";
                case CalculationMethod.Manual: return "手动赋值";
                default: throw new ArgumentOutOfRangeException(nameof(method));
            }
        }
    }

    // 输入以字段名保存，数字采用内部单位和往返精度，不经过TXT导出的舍入及单位换算。
    // 只允许现有输入类的公开标量字段，不反序列化任意运行时类型。
    [DataContract]
    public sealed class InputValue
    {
        [DataMember] public string Name;
        [DataMember] public string Value;
    }

    [DataContract]
    public sealed class InputRecord
    {
        [DataMember] public List<InputValue> Values = new List<InputValue>();

        public static InputRecord Capture(object source, params string[] names)
        {
            var record = new InputRecord();
            foreach (var field in source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => names.Length == 0 || names.Contains(f.Name)).OrderBy(f => f.Name, StringComparer.Ordinal))
            {
                object value = field.GetValue(source);
                record.Values.Add(new InputValue
                {
                    Name = field.Name,
                    Value = value is double ? ((double)value).ToString("R", CultureInfo.InvariantCulture)
                        : Convert.ToString(value, CultureInfo.InvariantCulture)
                });
            }
            return record;
        }

        public void Apply(object target, params string[] names)
        {
            if (Values == null || Values.Any(v => v == null || v.Name == null) ||
                Values.Select(v => v.Name).Distinct().Count() != Values.Count)
                throw new SerializationException("工程输入字段缺失或重复。");
            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => names.Length == 0 || names.Contains(f.Name)).ToArray();
            foreach (var field in fields)
            {
                var saved = Values.SingleOrDefault(v => v.Name == field.Name);
                if (saved == null) throw new SerializationException("工程输入缺少字段：" + field.Name);
                object value;
                if (field.FieldType == typeof(double))
                {
                    double number = double.Parse(saved.Value, CultureInfo.InvariantCulture);
                    if (double.IsNaN(number) || double.IsInfinity(number))
                        throw new SerializationException("工程输入包含无效数值：" + field.Name);
                    value = number;
                }
                else if (field.FieldType == typeof(int)) value = int.Parse(saved.Value, CultureInfo.InvariantCulture);
                else if (field.FieldType == typeof(bool)) value = bool.Parse(saved.Value);
                else if (field.FieldType == typeof(string)) value = saved.Value ?? "";
                else throw new SerializationException("不支持的工程输入字段：" + field.Name);
                field.SetValue(target, value);
            }
        }
    }

    [DataContract]
    public sealed class ProjectInputs
    {
        public static readonly string[] ParameterNames =
        {
            "LengthOfECS", "ThickOfECS", "LengthOfSupports", "ElevOfCollar", "ElevOfGround",
            "GroundLoad", "EpsDefor", "MaxMommentOfECS1", "MaxMommentOfECS2", "MaxShearForceOfECS"
        };
        public static readonly string[] LocalLoadNames = { "DistToECS", "Width", "LocalGroundLoad" };
        [DataMember] public InputRecord Parameters;
        [DataMember] public List<InputRecord> SoilLayers = new List<InputRecord>();
        [DataMember] public List<InputRecord> Supports = new List<InputRecord>();
        [DataMember] public List<InputRecord> Loadcases = new List<InputRecord>();
        [DataMember] public List<InputRecord> LocalLoads = new List<InputRecord>();
        [DataMember] public bool EnableWater;
        [DataMember] public double WaterTableElev;
        [DataMember] public bool UseNonlinearSoilSpring;

        public static ProjectInputs Capture(Form_Main form)
        {
            return new ProjectInputs
            {
                Parameters = InputRecord.Capture(form, ParameterNames),
                SoilLayers = form.SoilLayers.Select(s => InputRecord.Capture(s)).ToList(),
                Supports = form.Supports.Select(s => InputRecord.Capture(s)).ToList(),
                Loadcases = form.Loadcases.Select(s => InputRecord.Capture(s)).ToList(),
                LocalLoads = form.LocalLoads.Select(s => InputRecord.Capture(s, LocalLoadNames)).ToList(),
                EnableWater = Loadcase.EnableWater,
                WaterTableElev = Loadcase.WaterTableElev,
                UseNonlinearSoilSpring = Loadcase.UseNonlinearSoilSpring
            };
        }

        public List<SoilLayer> CreateSoils() => SoilLayers.Select(row =>
        {
            var value = new SoilLayer(1, 0, 0, 0.5, 1, 1, 18, "");
            row.Apply(value);
            return value;
        }).ToList();
        public List<Support> CreateSupports() => Supports.Select(row =>
        {
            var value = new Support("钢", 0, 1, 1, 0.1, 0, 0, false);
            row.Apply(value);
            if (value.JackStrokeMax < 0) throw new SerializationException("千斤顶行程上限不能小于0。");
            return value;
        }).ToList();
        public List<Loadcase> CreateLoadcases() => Loadcases.Select(row =>
        {
            var value = new Loadcase(0, false);
            row.Apply(value);
            return value;
        }).ToList();
        public List<LocalLoad> CreateLocalLoads() => LocalLoads.Select(row =>
        {
            var value = new LocalLoad(0, 0, 0);
            row.Apply(value, LocalLoadNames);
            return value;
        }).ToList();

        public string Signature()
        {
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(ProjectFile.Serialize(this)));
        }

        public void Validate()
        {
            if (Parameters == null || SoilLayers == null || Supports == null || Loadcases == null || LocalLoads == null)
                throw new SerializationException("工程输入数据不完整。");
            // 校验参数，但不创建或修改窗体。
            Parameters.Apply(new InputParameters(), ParameterNames);
            CreateSoils(); CreateSupports(); CreateLoadcases(); CreateLocalLoads();
            if (double.IsNaN(WaterTableElev) || double.IsInfinity(WaterTableElev))
                throw new SerializationException("地下水位数值无效。");
        }
    }

    public sealed class InputParameters
    {
        public double LengthOfECS, ThickOfECS, LengthOfSupports, ElevOfCollar, ElevOfGround,
            GroundLoad, EpsDefor, MaxMommentOfECS1, MaxMommentOfECS2, MaxShearForceOfECS;
    }

    [DataContract]
    public sealed class SavedNode
    {
        [DataMember] public int No;
        [DataMember] public double X, Y, Ux, Uy, Rot;
    }

    [DataContract]
    public sealed class SavedElement
    {
        [DataMember] public int No, LeftNo, RightNo;
        [DataMember] public string Type, Material;
        [DataMember] public double E, Area, Iz, InitialStrain;
        [DataMember] public bool Alive;
        // 顺序固定为iFx、iFy、iMom、jFx、jFy、jMom；保存实际阶段内力，查看时不重新求解。
        [DataMember] public double[] Forces;
        [DataMember] public double[] EquivalentLoads;
    }

    [DataContract]
    public sealed class SavedSupport
    {
        [DataMember] public int Index, ElementNo;
        [DataMember] public double Stroke, ContactUx, Compression;
    }

    [DataContract]
    public sealed class StageResults
    {
        [DataMember] public int Number;
        [DataMember] public string Status, ManualInput;
        [DataMember] public double ExcavationElevation;
        [DataMember] public List<SavedNode> Nodes;
        [DataMember] public List<SavedElement> Elements;
        [DataMember] public List<SavedSupport> Supports;
        [DataMember] public int[] WallNodeIndices, WallElementIndices;
        [DataMember] public double[] AppliedLoads, Reactions;
    }

    [DataContract]
    public sealed class ResultEnvelope
    {
        [DataMember] public double[] WallNodeY, WallElementY, SupportY;
        [DataMember] public double[] UxMax, UxMin, MomMax, MomMin, FyMax, FyMin, FxMax, FxMin;
    }

    [DataContract]
    public sealed class MethodResults
    {
        [DataMember] public CalculationMethod Method;
        [DataMember] public ProjectInputs Inputs;
        [DataMember] public string StartedAt, UpdatedAt, Status, Log = "";
        [DataMember] public List<StageResults> Stages = new List<StageResults>();
        [DataMember] public ResultEnvelope Envelope;

        public string DisplayStatus()
        {
            int total = Inputs.Loadcases.Count;
            return Status + "（" + Stages.Count + "/" + total + "阶段）";
        }
    }

    [DataContract]
    public sealed class ProjectDocument
    {
        [DataMember] public string Format = "ActiveControl.Project";
        [DataMember] public int Version = 1;
        [DataMember] public string Name = "未命名工程", SavedAt;
        [DataMember] public ProjectInputs Inputs;
        [DataMember] public List<MethodResults> Results = new List<MethodResults>();
        [DataMember] public CalculationMethod? SelectedMethod;
        [DataMember] public string SelectedStage, FigureType;

        public void Validate()
        {
            if (Format != "ActiveControl.Project" || Version != 1)
                throw new SerializationException("不是有效工程文件，或工程文件版本不受支持。");
            if (Inputs == null || Results == null) throw new SerializationException("工程数据不完整。");
            Inputs.Validate();
            if (Results.Any(r => r == null) || Results.Select(r => r.Method).Distinct().Count() != Results.Count)
                throw new SerializationException("工程计算结果重复或损坏。");
            foreach (var result in Results)
            {
                if (!Enum.IsDefined(typeof(CalculationMethod), result.Method) || result.Inputs == null || result.Stages == null)
                    throw new SerializationException("计算方法或结果数据无效。");
                result.Inputs.Validate();
                if (result.Stages.Count > result.Inputs.Loadcases.Count)
                    throw new SerializationException("结果阶段数超出施工阶段数。");
                for (int i = 0; i < result.Stages.Count; i++) ValidateStage(result.Stages[i], i + 1);
                if (result.Stages.Count > 0) ValidateEnvelope(result.Envelope);
            }
        }

        private static void ValidateStage(StageResults stage, int expected)
        {
            if (stage == null || stage.Number != expected || stage.Nodes == null || stage.Nodes.Count == 0 ||
                stage.Elements == null || stage.Supports == null || stage.WallNodeIndices == null || stage.WallElementIndices == null ||
                stage.AppliedLoads == null || stage.AppliedLoads.Length != stage.Nodes.Count * 3)
                throw new SerializationException("施工阶段结果不完整。");
            if (stage.Nodes.Where((n, i) => n == null || n.No != i + 1).Any() ||
                stage.Elements.Where((e, i) => e == null || e.No != i + 1 || e.LeftNo < 1 || e.RightNo < 1 ||
                    e.LeftNo > stage.Nodes.Count || e.RightNo > stage.Nodes.Count || e.Forces == null || e.Forces.Length != 6).Any() ||
                stage.WallNodeIndices.Any(i => i < 0 || i >= stage.Nodes.Count) ||
                stage.WallElementIndices.Any(i => i < 0 || i >= stage.Elements.Count) ||
                stage.Supports.Any(s => s == null || s.ElementNo < 1 || s.ElementNo > stage.Elements.Count))
                throw new SerializationException("施工阶段节点、单元索引或内力数据无效。");
            foreach (var node in stage.Nodes) CheckFinite(new[] { node.X, node.Y, node.Ux, node.Uy, node.Rot });
            foreach (var element in stage.Elements)
            {
                CheckFinite(element.Forces);
                CheckFinite(new[] { element.E, element.Area, element.Iz, element.InitialStrain });
                if (element.EquivalentLoads == null || element.EquivalentLoads.Length != 6)
                    throw new SerializationException("单元等效荷载数据不完整。");
                CheckFinite(element.EquivalentLoads);
            }
            foreach (var support in stage.Supports) CheckFinite(new[] { support.Stroke, support.ContactUx, support.Compression });
            CheckFinite(stage.AppliedLoads);
            CheckFinite(stage.Reactions);
        }

        private static void ValidateEnvelope(ResultEnvelope envelope)
        {
            if (envelope == null) throw new SerializationException("缺少包络结果。");
            CheckPair(envelope.WallNodeY, envelope.UxMax, envelope.UxMin);
            CheckPair(envelope.WallElementY, envelope.MomMax, envelope.MomMin);
            CheckPair(envelope.WallElementY, envelope.FyMax, envelope.FyMin);
            CheckPair(envelope.SupportY, envelope.FxMax, envelope.FxMin);
        }
        private static void CheckPair(double[] y, double[] max, double[] min)
        {
            if (y == null || max == null || min == null || y.Length != max.Length || y.Length != min.Length)
                throw new SerializationException("包络数据长度不一致。");
            CheckFinite(y); CheckFinite(max); CheckFinite(min);
        }
        private static void CheckFinite(double[] values)
        {
            if (values == null || values.Any(x => double.IsNaN(x) || double.IsInfinity(x)))
                throw new SerializationException("结果包含无效数值或缺失数据。");
        }
    }

    public static class ProjectFile
    {
        public static byte[] Serialize<T>(T value)
        {
            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings
                { MaxItemsInObjectGraph = int.MaxValue }).WriteObject(stream, value);
                return stream.ToArray();
            }
        }

        public static ProjectDocument Read(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var document = (ProjectDocument)new DataContractJsonSerializer(typeof(ProjectDocument),
                    new DataContractJsonSerializerSettings { MaxItemsInObjectGraph = int.MaxValue }).ReadObject(stream);
                if (document == null) throw new SerializationException("工程文件为空。");
                document.Validate();
                return document;
            }
        }

        public static void Write(string path, ProjectDocument document)
        {
            document.Validate();
            byte[] bytes = Serialize(document);
            // 同目录临时文件写完后原子替换，写入失败不破坏已有工程。
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }
    }
}
