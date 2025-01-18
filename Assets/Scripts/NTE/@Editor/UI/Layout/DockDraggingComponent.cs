using NTE.Core.UI;

using UnityEngine.EventSystems;

namespace NTE.Editor.UI.Layout
{
    public class DockDraggingComponent : RectBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private bool Dragging = false;

        private DockLayout Parent => GetComponentInParent<DockLayout>();

        public void OnBeginDrag(PointerEventData e)
        {
            Dragging = true;
        }

        public void OnDrag(PointerEventData e)
        {
            if (Dragging)
                if (Parent.VerticalSplit)
                    Parent.ASize = 1 - (e.position.y / Parent.transform.rect.height);
                else
                    Parent.ASize = e.position.x / Parent.transform.rect.width;
        }

        public void OnEndDrag(PointerEventData e)
        {
            Dragging = false;
        }
    }
}
