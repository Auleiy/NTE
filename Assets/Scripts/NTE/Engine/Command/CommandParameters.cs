using System;
using System.Collections.Generic;

using NTE.Core.Utils;
using NTE.Engine.Exceptions.Command;

#nullable enable

namespace NTE.Engine.Command
{
    public class CommandParameters
    {
        public Command BindedCommand;
        public List<Parameter> Parameters;
        public readonly int RequiredParametersCount;

        private CommandParameters(List<Parameter> paras, int reqCount, Command bind)
        {
            Parameters = paras;
            RequiredParametersCount = reqCount;
            BindedCommand = bind;
        }

        public string[] Validate(string[] raw)
        {
            List<string> buf = new(raw);

            if (raw.Length < RequiredParametersCount)
                throw new CommandParametersCountException(CommandRegistry.GetKey(BindedCommand), RequiredParametersCount, raw.Length);
            if (raw.Length > Parameters.Count)
                throw new CommandParametersCountException(CommandRegistry.GetKey(BindedCommand), Parameters.Count, raw.Length);

            for (int i = 0; i < Parameters.Count; i++)
            {
                if (i >= RequiredParametersCount && buf.Count < i - 1)
                    // 可选参数不可能没有默认值
#pragma warning disable CS8602 // 解引用可能出现空引用。
                    buf.Add(Parameters[i].GetDefault().ToString());
#pragma warning restore CS8602 // 解引用可能出现空引用。

                string errtext = Parameters[i].CheckValid(buf[i]);
                if (!errtext.IsNullOrEmpty())
                    throw new CommandParametersTypeException(CommandRegistry.GetKey(BindedCommand), Parameters[i].TypeString, errtext, i);
            }
            return buf.ToArray();
        }

        public T Get<T>(string[] args, int index)
        {
            return (T)Parameters[index].Get(args[index]);
        }

        public class Builder
        {
            private readonly List<Parameter> Parameters = new();

            private int RequiredParametersCount = 0;

            private bool IsOptionalParameter = false;

            public Builder() { }

            public Builder String(string? def = null)
            {
                Parameters.Add(new StringParameter(def));
                if (def != null)
                    IsOptionalParameter = true;
                else
                {
                    if (IsOptionalParameter)
                        throw new Exception("非可选参数应该在所有可选参数之前。");
                    RequiredParametersCount++;
                }
                return this;
            }

            public Builder Int(int? def = null)
            {
                Parameters.Add(new IntParameter(def));
                if (def != null)
                    IsOptionalParameter = true;
                else
                {
                    if (IsOptionalParameter)
                        throw new Exception("非可选参数应该在所有可选参数之前。");
                    RequiredParametersCount++;
                }
                return this;
            }

            public Builder Float(float? def = null)
            {
                Parameters.Add(new FloatParameter(def));
                if (def != null)
                    IsOptionalParameter = true;
                else
                {
                    if (IsOptionalParameter)
                        throw new Exception("非可选参数应该在所有可选参数之前。");
                    RequiredParametersCount++;
                }
                return this;
            }

            public CommandParameters Build(Command bind)
            {
                return new CommandParameters(Parameters, RequiredParametersCount, bind);
            }
        }

        public abstract class Parameter
        {
            public abstract string TypeString { get; }

            public abstract string CheckValid(string raw);

            public abstract object Get(string raw);

            public abstract object? GetDefault();
        }

        public abstract class Parameter<T> : Parameter
        {
            public T? DefaultValue;

            public Parameter(T? def)
            {
                DefaultValue = def;
            }

            public override string CheckValid(string raw) => string.Empty;

            public override object? GetDefault() => DefaultValue;
        }

        public class IntParameter : Parameter<int?>
        {
            public IntParameter(int? def) : base(def) { }

            public override string TypeString => "整数";

            public override string CheckValid(string raw)
            {
                return int.TryParse(raw, out _) ? string.Empty : "参数不是整数";
            }

            public override object Get(string raw)
            {
                return int.Parse(raw);
            }
        }
        public class FloatParameter : Parameter<float?>
        {
            public FloatParameter(float? def) : base(def) { }

            public override string TypeString => "浮点数";

            public override string CheckValid(string raw)
            {
                return float.TryParse(raw, out _) ? string.Empty : "参数不是浮点数";
            }

            public override object Get(string raw)
            {
                return float.Parse(raw);
            }
        }
        public class StringParameter : Parameter<string?>
        {
            public StringParameter(string? def) : base(def) { }

            public override string TypeString => "字符串";

            public override object Get(string raw) => raw;
        }
    }
}