using Newtonsoft.Json.Linq;

using UnityEngine;

namespace NTE.Core.Project.Resource.Configs
{
    public struct ImageConfig
    {
        public string Image;
        public Vector2 Offset;

        private ImageConfig(string img, Vector2 offset)
        {
            Image = img;
            Offset = offset;
        }

        public static readonly ImageConfig Empty = new("empty", new(0, 0));

        public static ImageConfig Parse(JToken root)
        {
            if (root["image"].Value<string>().Equals("empty"))
                return Empty;
            return new(root["image"].Value<string>(), new(root["offset"][0].Value<float>(), root["offset"][1].Value<float>()));
        }

        public readonly bool IsEmpty => Image.Equals("empty");

        /// <summary>
        /// 获取图片配置的素材
        /// </summary>
        /// <returns>素材，为Null时表示找不到或者为空</returns>
        public readonly Texture2D Get()
        {
            if (IsEmpty)
                return null;
            return ResourceManager.GetSprite(Image);
        }
    }
}