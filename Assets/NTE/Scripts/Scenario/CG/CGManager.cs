using System.Collections;
using System.Collections.Generic;
using System.IO;

using NTE.Project;
using NTE.UI;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Scenario.CG
{
    [RequireComponent(typeof(RawImage), typeof(Hidable))]
    public class CGManager : MonoBehaviour
    {
        public GameObject LayerTemplate;

        private readonly Dictionary<string, CGLayer> Layers = new();
        private CGConfig Config;

        private Hidable Hidable => GetComponent<Hidable>();
        private RawImage Image => GetComponent<RawImage>();

        private readonly Dictionary<string, CGConfig> LoadedConfigs = new();

        public void Set(CGConfig cfg)
        {
            StartCoroutine(Set_E(cfg));
        }

        public void Set(string filename)
        {
            if (!LoadedConfigs.TryGetValue(filename, out CGConfig cfg))
                cfg = new(Path.Combine(ProjectData.Current.DataPath, @"Configs\CGs", filename));
            Set(cfg);
        }

        private IEnumerator Set_E(CGConfig cfg)
        {
            foreach (KeyValuePair<string, CGLayer> l in Layers)
                Destroy(l.Value.gameObject);
            Layers.Clear();

            Config = cfg;
            Image.texture = Config.Base.GetProceedTexture();

            foreach (KeyValuePair<string, Utils.Image> kvp in Config.Layers)
            {
                CGLayer l = Instantiate(LayerTemplate, transform).GetComponent<CGLayer>();
                l.UI.texture = kvp.Value.GetProceedTexture();
                l.transform.anchoredPosition = new(kvp.Value.X, kvp.Value.Y);
                l.transform.sizeDelta = new(kvp.Value.Width, kvp.Value.Height);
                Layers.Add(kvp.Key, l);
            }

            Hidable cl = Instantiate(this, transform.parent).GetComponent<Hidable>();
            cl.HideDestroy();

            if (cfg == null)
                Hidable.HideImmediate();

            yield return new WaitForSeconds(0.5f);

            Hidable.Show();
        }

        public void ToggleLayer(string key)
        {
            Layers[key].Toggle();
        }

        public void SetLayer(string key, bool visible)
        {
            CGLayer l = Layers[key];
            l.IsOn = visible;
        }
    }
}
