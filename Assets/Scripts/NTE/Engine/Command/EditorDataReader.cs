using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NTE.Core.Utils;

using UnityEngine;

namespace NTE.Engine.Command
{
    public class EditorDataReader : MonoBehaviour
    {
        public Dictionary<string, Command> Commands = new();

        public void Start()
        {
            string text = Resources.Load<TextAsset>("Datas/commands").text;
            JObject jt = JToken.Parse(text).Value<JObject>();
            Commands = Command.ParseAll(jt);
            NTEUtils.Breakpoint();
        }

        public class Command
        {
            public string Name;
            public string Display;
            public string Documentation;
            public List<EditorParameter> Parameters = new();

            public Command(JProperty p)
            {
                Name = p.Name;
                JToken tk = p.Value;

                Display = tk["display"].Value<string>();
                Documentation = tk["doc"].Value<string>();
                foreach (JToken param in tk["params"].AsJEnumerable())
                    Parameters.Add(EditorParameter.Parse(param));
            }

            public static Dictionary<string, Command> ParseAll(JObject obj)
            {
                Dictionary<string, Command> col = new();
                foreach (JProperty p in obj.Properties())
                    col.Add(p.Name, new(p));
                return col;
            }
        }

        public enum ParameterType
        {
            String,
            Integer,
            Float,
            Boolean
        }

        public static ParameterType ParseType(string text)
        {
            return text switch
            {
                "string" => ParameterType.String,
                "integer" => ParameterType.Integer,
                "float" => ParameterType.Float,
                "boolean" => ParameterType.Boolean,
                _ => ParameterType.String
            };
        }

        public class EditorParameter
        {
            public string Display;
            public ParameterType Type;
            public object Default;
            
            public EditorParameter(string display, ParameterType type, object def)
            {
                Display = display;
                Type = type;
                Default = def;
            }

            public static EditorParameter Parse(JToken jt)
            {
                string dis = jt["display"].Value<string>();
                ParameterType type = ParseType(jt["type"].Value<string>());
                if (jt["default"].Type == JTokenType.Null)
                    return new(dis, type, null);
                object def = type switch
                {
                    ParameterType.String => jt["default"].Value<string>(),
                    ParameterType.Integer => jt["default"].Value<int>(),
                    ParameterType.Float => jt["default"].Value<float>(),
                    ParameterType.Boolean => jt["default"].Value<bool>(),
                    _ => throw new Exception("类型错误")
                };
                return new(dis, type, def);
            }

            public T GetDefault<T>()
            {
                switch (Type)
                {
                    case ParameterType.String: if (typeof(T) == typeof(string)) return (T)Default; else throw new Exception("类型不正确");
                    case ParameterType.Integer: if (typeof(T) == typeof(int)) return (T)Default; else throw new Exception("类型不正确");
                    case ParameterType.Float: if (typeof(T) == typeof(float)) return (T)Default; else throw new Exception("类型不正确");
                    case ParameterType.Boolean: if (typeof(T) == typeof(bool)) return (T)Default; else throw new Exception("类型不正确");
                    default: throw new Exception("类型错误");
                }
            }
        }
    }
}