using System;
using System.Collections.Generic;

#nullable enable

namespace NTE.Command
{
    public class CommandParameters
    {
        public List<Parameter> Parameters;
        public readonly int RequiredParametersCount;

        private CommandParameters(List<Parameter> paras, int reqCount)
        {
            Parameters = paras;
            RequiredParametersCount = reqCount;
        }

        public class Builder
        {
            private List<Parameter> Parameters = new();

            private int RequiredParametersCount = 0;

            private bool IsOptionalParameter = false;

            public Builder() { }

            public Builder Int(int? def = null)
            {
                Parameters.Add(new NumberParameter(def));
                if (def == null)
                {
                    RequiredParametersCount++;
                    IsOptionalParameter = true;
                }
                else if (IsOptionalParameter)
                    throw new Exception("非可选参数应该在所有可选参数之前。");
                return this;
            }

            public Builder String(string? def = null)
            {
                Parameters.Add(new StringParameter(def));
                if (def == null)
                {
                    RequiredParametersCount++;
                    IsOptionalParameter = true;
                }
                else if (IsOptionalParameter)
                    throw new Exception("非可选参数应该在所有可选参数之前。");
                return this;
            }

            public CommandParameters Build()
            {
                return new CommandParameters(Parameters, RequiredParametersCount);
            }
        }

        public class Parameter { }

        public class Parameter<T> : Parameter
        {
            public T? DefaultValue;

            public Parameter(T? def)
            {
                DefaultValue = def;
            }
        }

        public class NumberParameter : Parameter<float?> { public NumberParameter(float? def) : base(def) { } }
        public class StringParameter : Parameter<string?> { public StringParameter(string? def) : base(def) { } }
    }
}