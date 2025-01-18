using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using Object = UnityEngine.Object;

namespace NTE.EditorUI.Controlling
{
    public class RightMenuConfig
    {
        public List<IRightMenuItem> Items;

        public RightMenuConfig(List<IRightMenuItem> items) => Items = items;

        public List<GameObject> Instantiate(Transform parent)
        {
            List<GameObject> buf = new();
            for (int i = 0; i < Items.Count - 1; i++, buf.Add(Items[i].CreateGameInstance(parent))) ;
            return buf;
        }
    }

    public interface IRightMenuItem
    {
        public GameObject CreateGameInstance(Transform parent);
    }

    public class RightMenuButton : IRightMenuItem
    {
        public string Name;
        public string Hotkey;
        public Action OnClick;

        public RightMenuButton(string name, Action onClick)
        {
            Name = name;
            OnClick = onClick;
        }

        public RightMenuButton(string name, Action onClick, string hotkey = "")
        {
            Name = name;
            Hotkey = hotkey;
            OnClick = onClick;
        }

        public GameObject CreateGameInstance(Transform parent)
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("Prefabs/RightClickMenuButton"), parent);
            go.transform.Find("Text").GetComponent<TMP_Text>().text = Name;
            go.transform.Find("Hotkey").GetComponent<TMP_Text>().text = Hotkey;
            go.GetComponent<Button>().OnClick = OnClick;
            return go;
        }
    }

    public class RightMenuSeparator : IRightMenuItem
    {
        public GameObject CreateGameInstance(Transform parent) => Object.Instantiate(Resources.Load<GameObject>("Prefabs/RightClickMenuSeparator"), parent);
    }
}
