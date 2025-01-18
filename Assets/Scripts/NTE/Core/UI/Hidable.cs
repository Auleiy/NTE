using DG.Tweening;

using NTE.Core.Utils;

using UnityEngine;

namespace NTE.Core.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Hidable : MonoBehaviour
    {
        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

        public float ShowAlpha = 1, HideAlpha, Duration = 0.3f;

        public bool ChangeInteractable, ChangeBlockRaycasts, Shown = true;

        public void Show()
        {
            if (!Shown)
            {
                CanvasGroup.DOFade(ShowAlpha, Duration);
                SetInteraction(true);
                Shown = true;
            }
        }
        public void ShowImmediate()
        {
            if (!Shown)
            {
                CanvasGroup.alpha = ShowAlpha;
                SetInteraction(true);
                Shown = true;
            }
        }

        public void Hide()
        {
            if (Shown)
            {
                CanvasGroup.DOFade(HideAlpha, Duration);
                SetInteraction(true);
                Shown = false;
            }
        }
        public void HideImmediate()
        {
            if (Shown)
            {
                CanvasGroup.alpha = HideAlpha;
                SetInteraction(true);
                Shown = false;
            }
        }
        public void HideDestroy()
        {
            if (Shown)
            {
                Hide();
                this.WaitInvoke(() => { Destroy(gameObject); }, Duration + 0.1f);
            }
        }
        public void HideDestroyImmediate()
        {
            if (Shown)
            {
                HideImmediate();
                Destroy(gameObject);
            }
        }

        public bool Toggle()
        {
            if (!Shown)
                Show();
            else
                Hide();
            return Shown;
        }

        public bool Select(bool value) // 方便链式引用
        {
            if (value)
                Show();
            else
                Hide();
            return value;
        }

        private void SetInteraction(bool value)
        {
            if (ChangeInteractable)
                CanvasGroup.interactable = value;
            if (ChangeBlockRaycasts)
                CanvasGroup.blocksRaycasts = value;
        }
    }
}
