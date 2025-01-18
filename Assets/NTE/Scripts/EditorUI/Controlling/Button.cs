using System;

using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NTE.EditorUI.Controlling
{
    public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Color Normal, Hover, Down;
        public Action OnClick;

        public RawImage Background;

        public void OnPointerDown(PointerEventData eventData)
        {
            Background.DOColor(Down, 0.1f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Background.DOColor(Hover, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Background.DOColor(Normal, 0.1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Background.DOColor(Hover, 0.1f);
            OnClick.Invoke();
        }
    }
}
