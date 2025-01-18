using System;

using NTE.Core.UI;

using UnityEngine;
using UnityEngine.EventSystems;

namespace NTE.Editor.UI.Windowing
{
    public class WindowDragger : RectBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<Vector2> OnDragEvent = x => { };

        private bool Dragging = false;
        private Vector2 originPos;

        public void OnBeginDrag(PointerEventData e)
        {
            Debug.Log("begin drag");
            Dragging = true;
            originPos = e.position;
        }

        

        public void OnDrag(PointerEventData e)
        {
            if (Dragging)
            {
                OnDragEvent?.Invoke(e.position - originPos);
                originPos = e.position;
            }
        }

        public void OnEndDrag(PointerEventData e)
        {
            Debug.Log("end drag");
            Dragging = false;
        }
    }
}

