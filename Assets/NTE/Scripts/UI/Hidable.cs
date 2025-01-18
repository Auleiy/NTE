using DG.Tweening;

using UnityEngine;

using NTE.Utils;

namespace NTE.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Hidable : MonoBehaviour
    {
        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

        public float ShowAlpha = 1, HideAlpha, Duration = 0.3f;

        public void Show() => CanvasGroup.DOFade(ShowAlpha, Duration);
        public void ShowImmediate() => CanvasGroup.alpha = ShowAlpha;

        public void Hide() => CanvasGroup.DOFade(HideAlpha, Duration);
        public void HideImmediate() => CanvasGroup.alpha = HideAlpha;
        public void HideDestroy()
        {
            Hide();
            this.WaitInvoke(() => { Destroy(gameObject); }, Duration + 0.1f);
        }
        public void HideDestroyImmediate()
        {
            HideImmediate();
            Destroy(gameObject);
        }
    }
}
