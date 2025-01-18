using UnityEngine;
using UnityEngine.UI;

namespace NTE.Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RectBehaviour : MonoBehaviour
    {
        public new RectTransform transform => (RectTransform)base.transform;
    }

    [RequireComponent(typeof(RectTransform))]
    public class SelectableRectBehaviour : Selectable
    {
        public new RectTransform transform => (RectTransform)base.transform;
    }
}
