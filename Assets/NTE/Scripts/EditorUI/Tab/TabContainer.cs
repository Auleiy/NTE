using System.Collections.Generic;
using NTE.EditorUI.Controlling;
using UnityEngine;

public class TabContainer : MonoBehaviour
{
    public List<TabButton> Buttons = new();

    public int SelectedIndex = 0;

    private void Awake()
    {
        Buttons[SelectedIndex].Select();
    }

    public void Select(TabButton btn)
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

    public void Close(TabButton btn)
    {
        if (Buttons[SelectedIndex] == btn && SelectedIndex == Buttons.Count - 1)
        {
            SelectedIndex--;
            RefreshSelection();
        }
        Buttons.Remove(btn);
    }
}
