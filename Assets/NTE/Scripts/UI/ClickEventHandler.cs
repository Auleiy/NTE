using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NTE.UI
{
    public class ClickEventHandler : UIBehaviour, IPointerClickHandler
    {
        public UnityEvent<PointerEventData> Click;

        public void OnPointerClick(PointerEventData eventData)
        {
            Click.Invoke(eventData);
        }
    }
}
