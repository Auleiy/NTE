using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace NTE.Core.Project.Resource.Configs
{
    public struct CGConfig
    {
        public struct Layer
        {
            public string Name { get; private set; }
            public bool OnlyOne { get; private set; }
            public Dictionary<string, Variant> Variants { get; private set; }

            public readonly Variant Default => Variants["default"];

            public static Layer Parse(JToken root)
            {
                JToken onlyOne = root["only_one"];
                return new()
                {
                    Name = root["name"].Value<string>(),
                    OnlyOne = onlyOne == null || onlyOne.Value<bool>(),
                    Variants = Variant.ParseMap(root["variants"])
                };
            }

            public static List<Layer> ParseList(JToken root)
            {
                List<Layer> layers = new();
                foreach (JToken t in root.AsJEnumerable())
                    layers.Add(Parse(t));
                return layers;
            }

            public readonly IEnumerator GetEnumerator()
            {
                return Variants.Values.GetEnumerator();
            }
        }

        public struct Variant
        {
            public string Name { get; private set; }
            public ImageConfig Image;

            public readonly bool IsEmpty => Image.IsEmpty;

            public static (string name, Variant obj) Parse(JToken root)
            {
                string name = root["name"].Value<string>();
                return (name, new()
                {
                    Name = name,
                    Image = ImageConfig.Parse(root["image"])
                });
            }

            public static Dictionary<string, Variant> ParseMap(JToken root)
            {
                Dictionary<string, Variant> layers = new();
                foreach (JToken t in root.AsJEnumerable())
                {
                    (string name, Variant obj) = Parse(t);
                    layers.Add(name, obj);
                }
                return layers;
            }
        }
        public string Name;
        public List<Layer> Layers;

        public static CGConfig Parse(JToken root)
        {
            return new()
            {
                Name = root["name"].Value<string>(),
                Layers = Layer.ParseList(root["layers"])
            };
        }
    }
}