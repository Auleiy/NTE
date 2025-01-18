using NTE.UI;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Scenario.CG
{
    [RequireComponent(typeof(RawImage), typeof(Hidable))]
    public class CGLayer : RectBehaviour
    {
        public RawImage UI => GetComponent<RawImage>();

        public Hidable Hidable => GetComponent<Hidable>();

        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
                if (value)
                    Hidable.Show();
                else
                    Hidable.Hide();
            }
        }

        private bool isOn;

        public void Toggle()
        {
            IsOn = !IsOn;
        }
    }
}