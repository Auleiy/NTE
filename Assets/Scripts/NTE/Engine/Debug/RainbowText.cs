using TMPro;

using UnityEngine;

namespace NTE.Engine.Debug
{
    public class RainbowText : MonoBehaviour
    {
        public TextMeshProUGUI Text;

        public float LoopTime = 8, Alpha = 0.25f;

        private void Update() // 我又在干什么……
        {
            float h = Time.time / LoopTime % 1;
            Color c = Color.HSVToRGB(h, 1, 1);
            c.a = Alpha;
            Text.color = c;
        }
    }
}
