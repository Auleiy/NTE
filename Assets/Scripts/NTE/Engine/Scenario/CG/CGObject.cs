using System.Collections;
using System.Collections.Generic;

using NTE.Core.Project.Resource.Configs;
using NTE.Core.UI;
using NTE.Core.Utils;
using NTE.Engine.Scenario.CG;

using UnityEngine;

namespace NTE.Core.Scenario.CG
{
    [RequireComponent(typeof(Hidable))]
    public class CGObject : MonoBehaviour
    {
        public Hidable Hidable => GetComponent<Hidable>();
        public CGConfig Config;

        private readonly Dictionary<string, CGLayerObject> Layers = new();

        public void CreateLayers(CGConfig cfg, Dictionary<string, string> startValues)
        {
            Config = cfg;
            foreach (CGConfig.Layer layer in cfg.Layers)
            {
                if (startValues.TryGetValue(layer.Name, out string variant))
                    AddLayer(layer, variant);
                else
                    AddLayer(layer);
            }
        }

        public void SetDefault()
        {
            foreach (CGLayerObject l in Layers.Values)
                l.SetOn("default");
        }

        public void Dispose(bool replace) // 真的要抛弃我吗……（泣）
        {
            if (replace)
                StartCoroutine(AwaitOff());
            else
                Hidable.HideDestroy();
        }

        public void AddLayer(CGConfig.Layer layer, string startVariant = "default")
        {
            GameObject lyr = new(layer.Name,
                    typeof(RectTransform),
                    typeof(CGLayerObject));

            CGLayerObject l = lyr.GetComponent<CGLayerObject>();

            RectTransform tl = lyr.GetComponent<RectTransform>();
            tl.FullRectTransform();
            tl.SetParent(transform);

            l.CreateVariants(layer, startVariant);

            Layers.Add(layer.Name, l);
        }

        /// <summary>
        /// 切换变种显示状态
        /// </summary>
        /// <param name="layer">层名称</param>
        /// <param name="variant">变种名称</param>
        public void SetOn(string layer, string variant)
        {
            if (Layers.TryGetValue(layer, out CGLayerObject obj))
                obj.SetOn(variant);
            else
                Debug.LogWarning($"不存在{layer}层。");
        }

        private IEnumerator AwaitOff()
        {
            yield return new WaitForSeconds(Hidable.Duration);
            Hidable.HideDestroy();
        }
    }
}