using System.Collections.Generic;

using NTE.Core.Project.Resource.Configs;
using NTE.Core.UI;
using NTE.Core.Utils;
using NTE.Engine.Debug;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Engine.Scenario.CG
{
    public class CGLayerObject : RectBehaviour
    {
        public CGConfig.Layer Config;

        public Dictionary<string, CGVariantObject> Variants = new();

        public void CreateVariants(CGConfig.Layer cfg, string defaultVariant)
        {
            Config = cfg;
            foreach (CGConfig.Variant var in cfg)
            {
                CGVariantObject o = AddVariant(var);
                if (o != null)
                {
                    if (var.Name.Equals(defaultVariant))
                    { 
                        o.ImmediateOn();
                        Current = o;
                    }
                }
            }
        }

        public CGVariantObject AddVariant(CGConfig.Variant var)
        {
            if (Variants.ContainsKey(var.Name))
            {
                Log.Warn($"{Config.Name}出现重复名称的变种{var.Name}，已跳过靠后的。");
                return null;
            }
            if (var.IsEmpty)
                return null;

            GameObject vo = new($"{var.Name}",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(CanvasGroup),
                typeof(RawImage),
                typeof(Hidable),
                typeof(CGVariantObject)
            );

            Texture2D img = var.Image.Get();

            Hidable h = vo.GetComponent<Hidable>();
            h.Duration = 0.5f;
            h.InitNonInteractiveHiding();

            CanvasGroup g = h.CanvasGroup;
            g.alpha = 0;
            g.interactable = false;
            g.blocksRaycasts = false;

            RawImage i = vo.GetComponent<RawImage>();
            i.texture = img;

            CGVariantObject v = vo.GetComponent<CGVariantObject>();
            v.Config = var;

            RectTransform t = vo.GetComponent<RectTransform>();
            t.anchoredPosition = var.Image.Offset;
            t.sizeDelta = new(img.width, img.height);
            t.pivot = Vector2.zero;
            t.SetParent(transform);

            Variants.Add(var.Name, v);

            return v;
        }

        private CGVariantObject Current;

        /// <summary>
        /// 改变变种。
        /// </summary>
        /// <param name="name">变种名</param>
        public void SetOn(string name)
        {
            void awaitOff(CGVariantObject obj)
            {
                if (Current != null && Current != obj)
                    Current.AwaitOff();
            }

            if (name.Equals("default"))
            {
                if (Variants.TryGetValue("default", out CGVariantObject def))
                {
                    awaitOff(def);
                    Current = def;
                    def.IsOn = true;
                }
                else if (Current != null)
                {
                    Current.IsOn = false;
                    Current = null;
                }
            }
            else
            {
                if (Variants.TryGetValue(name, out CGVariantObject obj))
                {
                    awaitOff(obj);
                    Current = obj;
                    obj.IsOn = true;
                }
                else
                {
                    if (Variants.TryGetValue("default", out CGVariantObject def))
                    {
                        awaitOff(def);
                        Current = def;
                        def.IsOn = true;
                    }
                    else if (Current != null)
                    {
                        Current.IsOn = false;
                        Current = null;
                    }
                    Log.Handle("找不到变种，切换到默认变种", LogType.Warning);
                }
            }
        }
    }
}