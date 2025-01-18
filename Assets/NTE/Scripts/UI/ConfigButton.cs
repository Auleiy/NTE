using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NTE.UI
{
    public class ConfigButton : RectBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Graphic Target;
        public UnityEvent OnClick;

        public Color Normal, Hover, Click;
        public float Transition, HoverRotation;

        public void OnPointerDown(PointerEventData eventData)
        {
            Target.CrossFadeColor(Click, Transition, false, true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Target.CrossFadeColor(Hover, Transition, false, true);
            if (HoverRotation != 0)
                transform.DORotate(new(0, 0, HoverRotation), Transition * 2).SetEase(Ease.OutCubic);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Target.CrossFadeColor(Normal, Transition, false, true);
            if (HoverRotation != 0)
                transform.DORotate(new(0, 0, 0), Transition * 2).SetEase(Ease.OutCubic);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnClick.Invoke();
            Target.CrossFadeColor(Hover, Transition, false, true);
        }
    }
}
