using DG.Tweening;

using NTE.Core.UI;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Editor.UI.Controlling
{
    public class RightMenu : RectBehaviour
    {
        private VerticalLayoutGroup Layout => GetComponent<VerticalLayoutGroup>();
        private CanvasGroup Alpha => GetComponent<CanvasGroup>();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) |
                Input.GetMouseButtonDown(1) |
                Input.GetMouseButtonDown(2))
                Hide();
        }

        private void Destroy() => Destroy(gameObject);

        public void Hide()
        {
            transform.DOAnchorPosY(transform.anchoredPosition.y - 32, 0.2f).SetEase(Ease.OutQuad);
            Alpha.DOFade(0, 0.2f).SetEase(Ease.OutQuad);
            Invoke(nameof(Destroy), 0.5f);
        }

        private void Init(RightMenuConfig cfg)
        {
            for (int i = transform.childCount; i > 0; i--, Destroy(transform.GetChild(i).gameObject)) ;
            cfg.Instantiate(transform);
        }

        public static void Create(RightMenuConfig cfg, Vector2 pos)
        {
            RightMenu o = Instantiate(Resources.Load<GameObject>("Prefabs/RightClickMenu"), GameObject.Find("/Canvas").transform).GetComponent<RightMenu>();
            o.Init(cfg);
            LayoutRebuilder.ForceRebuildLayoutImmediate(o.transform);
            o.Alpha.alpha = 1;
            o.transform.anchoredPosition = pos;
            float px = 0, py = 1;
            if (pos.y < Camera.main.pixelHeight / 2)
                py = 0;
            if (pos.x > Camera.main.pixelWidth / 2)
                px = 1;
            o.transform.pivot = new(px, py);
            o.transform.sizeDelta = new(o.transform.sizeDelta.x, 0);
            o.transform.DOSizeDelta(new(o.transform.sizeDelta.x, o.Layout.preferredHeight - 32), 0.3f).SetEase(Ease.OutQuad);
        }
    }
}
