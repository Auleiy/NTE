using System.Collections.Generic;

using NTE.Core.UI;

using UnityEngine;
using UnityEngine.UI;

namespace NTE.Editor.UI.VisualEditor
{
    public class LineContainer : RectBehaviour
    {
        public List<Line> Lines = new();

        public void UpdateLayout()
        {
            float y = 0;
            for (int i = 0; i < Lines.Count; ++i)
            {
                RectTransform c = Lines[i].transform;
                c.anchorMin = new(0, 1);
                c.anchorMax = new(1, 1);
                if (c.TryGetComponent(out LayoutElement e))
                    c.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, e.preferredHeight);
                y -= c.sizeDelta.y;
                c.anchoredPosition = new(c.anchoredPosition.x, y);
            }
        }
    }
}