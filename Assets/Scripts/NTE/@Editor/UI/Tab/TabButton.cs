using DG.Tweening;

using NTE.Core.UI;
using NTE.Editor.UI.Windowing;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Button = NTE.Editor.UI.Controlling.Button;

namespace NTE.Editor.UI.Tab
{
    // TabButton在Unity.ShaderGraph.Editor中出现，且被定义在全局命名空间，别名都会有冲突，只能换个名字了
    // Unity, Fuck you!
    [RequireComponent(typeof(RawImage))]
    public class NTabButton : RectBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public string Text
        {
            get => m_Text;
            set
            {
                m_Text = value;
                RefreshText();
            }
        }
        private string m_Text;

        public TextMeshProUGUI TextUI;
        public Button CloseButton;
        public TabEditor Content;

        public Color Normal, Hover, Selected;

        private bool IsSelected;

        private RawImage Graphic => GetComponent<RawImage>();
        private TabContainer Container => GetComponentInParent<TabContainer>();

        private void Awake()
        {
            CloseButton.OnClick += Close;
            Content.OnEdit += RefreshText;
        }

        public void RefreshText()
        {
            TextUI.text = m_Text + (Content.Edited ? "＊" : "");
        }

        public void OnPointerEnter(PointerEventData e)
        {
            if (!IsSelected)
                Graphic.DOColor(Hover, .1f);
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (!IsSelected)
                Graphic.DOColor(Normal, .1f);
        }

        public void OnPointerDown(PointerEventData e)
        { }

        public void OnPointerUp(PointerEventData e)
        {
            Container.Select(this);
        }

        public void Select()
        {
            IsSelected = true;
            Content.Hidable.Show();
            Graphic.DOColor(Selected, .1f);
        }

        public void Deselect()
        {
            IsSelected = false;
            Content.Hidable.Hide();
            Graphic.DOColor(Normal, .1f);
        }

        public void Close()
        {
            FileSaveWindow fsw = (FileSaveWindow)WindowContainer.Current.Create(Resources.Load<GameObject>("Prefabs/File Save Window"), new(768, -468));

            fsw.OnSubmit += x =>
            {
                if (x == ConfirmWindow.SubmitType.Closed || x == ConfirmWindow.SubmitType.Cancel)
                    return;
                if (x == ConfirmWindow.SubmitType.Save)
                    SaveFile();
                Content.Hidable.HideDestroy();
                Destroy(gameObject);

                Container.Close(this);
            };
        }

        public void SaveFile()
        {
            Debug.Log("Saved file");
        }
    }
}
