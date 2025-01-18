using System;

using NTE.UI;

using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragger : RectBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool Dragging = false;

    public event Action<Vector2> OnDragEvent = x => { };

    public void OnBeginDrag(PointerEventData e)
    {
        Debug.Log("begin drag");
        Dragging = true;
        originPos = e.position;
    }

    Vector2 originPos;

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
