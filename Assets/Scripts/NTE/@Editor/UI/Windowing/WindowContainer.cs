using System.Collections.Generic;

using UnityEngine;

namespace NTE.Editor.UI.Windowing
{
    public class WindowContainer : MonoBehaviour
    {
        public List<Window> Windows = new();

        public static WindowContainer Current;

        private void Awake()
        {
            Current = this;
        }

        public void Focus(Window w)
        {
            if (Windows.Count > 0)
            {
                Windows[0].Unfocus();
                Windows.Remove(w);
            }
            Windows.Insert(0, w);
            w.Focus();

            for (int i = 0; i < Windows.Count; i++)
                Windows[i].transform.SetSiblingIndex(transform.childCount - i - 1);
        }

        public Window Create(GameObject template, Vector2 position)
        {
            Window w = Instantiate(template, transform).GetComponent<Window>();
            w.transform.anchoredPosition = position;
            Focus(w);
            return w;
        }
    }
}
