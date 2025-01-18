using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

using NTE.Core.Project.Resource.Configs;
using NTE.Core.Utils;
using NTE.Engine.Project;

using UnityEngine;

namespace NTE.Core.Project.Resource
{
    public static class ResourceManager
    {
        private static Dictionary<string, Texture2D> LoadedSprites = new();

        private static Dictionary<string, CGConfig> CGConfigs = new();

        public static void LoadCGs()
        {
            string cgsPath = Path.Combine(ProjectData.Current.DataPath, "Resources", "Datas", "CGs");
            LoadCGs(cgsPath, "");
        }

        private static void LoadCGs(string path, string root)
        {
            foreach (string entry in Directory.GetFileSystemEntries(path))
            {
                if (entry.IsDirectory())
                    LoadCGs(entry, Path.Combine(root, Path.GetFileName(entry)));
                JToken obj = JObject.Parse(File.ReadAllText(entry));
                CGConfig cfg = CGConfig.Parse(obj);

                CGConfigs.Add(cfg.Name, cfg);

                Debug.Log($"加载CG配置 \"{cfg.Name}\"");
            }
        }

        public static CGConfig GetCG(string name)
        {
            if (CGConfigs.TryGetValue(name, out CGConfig c))
                return c;
            throw new Exception($"找不到CG \"{name}\"");
        }

        public static Texture2D GetSprite(string path)
        {
            if (LoadedSprites.TryGetValue(path, out Texture2D tex))
                return tex;
            tex = new(0, 0);
            tex.LoadImage(Load(Path.Combine("Sprites", path)));
            LoadedSprites.Add(path, tex);
            return tex;
        }

        public static byte[] Load(string path)
        {
            string fullPath = Path.Combine(ProjectData.Current.DataPath, "Resources", path);
            return File.ReadAllBytes(fullPath);
        }
    }
}
