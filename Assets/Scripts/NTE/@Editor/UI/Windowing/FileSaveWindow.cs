using NTE.Editor.UI.Controlling;

namespace NTE.Editor.UI.Windowing
{
    public class FileSaveWindow : ConfirmWindow
    {
        public Button Save, DoNotSave, Cancel;

        protected override void Awake()
        {
            base.Awake();
            Save.OnClick += () => Submit(SubmitType.Save);
            DoNotSave.OnClick += () => Submit(SubmitType.DoNotSave);
            Cancel.OnClick += () => Submit(SubmitType.Cancel);
        }
    }
}
