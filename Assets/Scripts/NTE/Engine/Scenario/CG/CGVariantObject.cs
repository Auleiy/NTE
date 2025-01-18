using System.Collections;

using NTE.Core.Project.Resource.Configs;
using NTE.Core.UI;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Engine.Scenario.CG
{
    [RequireComponent(typeof(RawImage), typeof(Hidable))]
    public class CGVariantObject : RectBehaviour
    {
        public CGConfig.Variant Config;

        public Hidable Hidable => GetComponent<Hidable>();

        public bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;
                if (value) { Hidable.Show(); transform.SetAsLastSibling(); }
                else Hidable.Hide();
            }
        }

        private bool isOn;

        public void AwaitOff()
        {
            StartCoroutine(AwaitOff_());
        }

        private IEnumerator AwaitOff_()
        {
            yield return new WaitForSeconds(Hidable.Duration);
            Hidable.HideImmediate();
        }

        public void ImmediateOn()
        {
            Hidable.ShowImmediate();
            transform.SetAsLastSibling();
        }
    }
}