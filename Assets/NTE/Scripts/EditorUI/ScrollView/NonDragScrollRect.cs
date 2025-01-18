using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonDragScrollRect : ScrollRect
{
    public override void OnDrag(PointerEventData e) { }
    public override void OnEndDrag(PointerEventData e) { }
    public override void OnBeginDrag(PointerEventData e) { }
    public override void OnInitializePotentialDrag(PointerEventData e) { }
}
