using System.Collections.Generic;

using NTE.Core.Project.Resource;
using NTE.Core.Project.Resource.Configs;
using NTE.Core.Scenario.CG;
using NTE.Core.UI;
using NTE.Core.Utils;

using UnityEngine;

namespace NTE.Engine.Scenario.CG
{
    public class CGManager : RectBehaviour
    {
        public CGObject Current;

        public Dictionary<string, string> StartValues = new();

        public void Set(CGConfig cfg)
        {
            Set_E(cfg);
        }

        public void Set(string key)
        {
            if (key.IsNullOrEmpty())
                Set();
            else
                Set(ResourceManager.GetCG(key));
        }

        public void Set()
        {
            Set_E(null);
        }

        private void Set_E(CGConfig? nullableCfg)
        {
            if (Current != null)
                Current.Dispose(nullableCfg.HasValue);
            if (nullableCfg.HasValue)
            {
                CGConfig cfg = nullableCfg.Value;
                GameObject cgo = new(cfg.Name,
                    typeof(RectTransform),
                    typeof(Hidable),
                    typeof(CGObject));

                Hidable h = cgo.GetComponent<Hidable>();
                h.Duration = 0.5f;
                h.InitNonInteractiveHiding();

                RectTransform to = cgo.GetComponent<RectTransform>();
                to.FullRectTransform();
                to.SetParent(transform);

                CGObject obj = cgo.GetComponent<CGObject>();
                obj.CreateLayers(cfg, StartValues);
                obj.Hidable.HideImmediate();
                obj.Hidable.Show();

                Current = obj;
            }
            StartValues.Clear();
        }

        public void SetOn(string layer, string variant)
        {
            Current.SetOn(layer, variant);
        }

        public void AddStartValue(string layer, string variant)
        {
            StartValues[layer] = variant;
        }
    }
}
