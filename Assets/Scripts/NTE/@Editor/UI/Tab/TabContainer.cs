using System.Collections.Generic;

using NTE.Editor.UI.Tab;

using UnityEngine;

namespace NTE.Editor.UI.Tab
{
    public class TabContainer : MonoBehaviour
    {
        public List<NTabButton> Buttons = new();

        public int SelectedIndex = 0;

        private void Awake()
        {
            Buttons[SelectedIndex].Select();
        }

        public void Select(NTabButton btn)
        {
            Buttons[SelectedIndex].Deselect();
            if (!Buttons.Contains(btn))
                Buttons.Insert(btn.transform.GetSiblingIndex(), btn);
            SelectedIndex = Buttons.IndexOf(btn);
            btn.Select();
        }

        public void RefreshSelection()
        {
            Buttons[SelectedIndex].Select();
        }

        public void Close(NTabButton btn)
        {
            if (Buttons[SelectedIndex] == btn && SelectedIndex == Buttons.Count - 1)
            {
                SelectedIndex--;
                RefreshSelection();
            }
            Buttons.Remove(btn);
        }
    }
}
