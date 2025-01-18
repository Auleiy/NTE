using TMPro;

using UnityEngine;

namespace NTE.Editor.UI.Windowing
{
    public class ConfirmWindow : Window
    {
        public enum SubmitType
        {
            Closed,
            Yes,
            No,
            OK,
            Cancel,
            Save,
            DoNotSave
        }

        public delegate void OnSubmitEventHandler(SubmitType type);

        public override string HeaderText => "确认";
        public TMP_Text Content;
        public string ContentText;

        public event OnSubmitEventHandler OnSubmit;

        [HideInInspector] public bool Submited;

        protected override void Awake()
        {
            base.Awake();
            OnSubmit += x =>
            {
                if (Submited)
                    return;
                Submited = true;
            };
        }

        protected override void OnClose()
        {
            OnSubmit?.Invoke(SubmitType.Closed);
        }

        protected void Submit(SubmitType type)
        {
            OnSubmit?.Invoke(type);
            Close();
        }

        protected override void OnGUI()
        {
            Content.text = ContentText;
        }
    }
}
