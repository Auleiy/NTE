using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Engine.Scenario.Effect
{
    public class ViewMosaic : MonoBehaviour
    {
        public RenderTexture Texture;
        public Camera Camera;
        public RawImage Displayer;

        public float StartMultiplier = 1, EndMultiplier = 128;
        public float ChangeTime;

        private float Current = 1;

        public void Transit()
        {
            StartCoroutine(TransitT());
        }

        private IEnumerator TransitT()
        {
            DOTween.To(() => Current, x => Current = x, EndMultiplier, ChangeTime).SetEase(Ease.InQuart);
            yield return new WaitForSeconds(ChangeTime);
            DOTween.To(() => Current, x => Current = x, StartMultiplier, ChangeTime).SetEase(Ease.OutQuart);
        }

        private void Start()
        {
            lastw = Mathf.RoundToInt(Camera.main.pixelWidth / Current);
            lasth = Mathf.RoundToInt(Camera.main.pixelHeight / Current);
            Texture = new(lastw, lasth, 32)
            {
                filterMode = FilterMode.Point,
                format = RenderTextureFormat.ARGBFloat
            };
            Camera.targetTexture = Texture;
            Camera.Render();
            Displayer.texture = Texture;

        }

        int lastw = 1920, lasth = 1080;
        private void Update()
        {
            int w = Mathf.RoundToInt(Camera.main.pixelWidth / Current), h = Mathf.RoundToInt(Camera.main.pixelHeight / Current);
            if (lastw != w || lasth != h) // 能省一点是一点 ：）
            {
                RenderTexture tex = new(w, h, 32)
                {
                    filterMode = FilterMode.Point,
                    format = RenderTextureFormat.ARGBFloat
                };
                Destroy(Texture);
                Texture = tex;
                Camera.targetTexture = Texture;
                Camera.Render();
                Displayer.texture = Texture;
                lastw = w;
                lasth = h;
            }
        }
    }
}
