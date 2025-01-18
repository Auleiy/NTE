using DG.Tweening;
using NTE.UI;
using NTE.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = NTE.EditorUI.Controlling.Button;

public abstract class Window : RectBehaviour, IPointerDownHandler
{
    public WindowDragger Dragger => GetComponentInChildren<WindowDragger>();
    public WindowContainer Container => GetComponentInParent<WindowContainer>();
    public Canvas Canvas => GetComponent<Canvas>();
    public Button CloseButton;
    public Hidable Hidable => GetComponent<Hidable>();
    public TMP_Text Header;
    public RawImage HeaderBar;

    public abstract string HeaderText { get; }

    protected virtual void Awake()
    {
        Dragger.OnDragEvent += OnDrag;
        CloseButton.OnClick += ExecuteOnClose;
    }

    protected virtual void Start()
    {
        Hidable.Show();
    }

    protected virtual void OnDrag(Vector2 offset)
    {
        transform.anchoredPosition += offset;
    }

    private void ExecuteOnClose()
    {
        OnClose();
        Container.Windows.Remove(this);
        Hidable.HideDestroy();
    }

    protected virtual void OnClose()
    { }

    protected void Close() => ExecuteOnClose();

    protected virtual void OnGUI()
    {
        Header.text = HeaderText;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Container.Focus(this);
    }

    public virtual void Focus()
    {
        HeaderBar.DOColor(Constants.WindowFocusColor, .1f);
    }
    public virtual void Unfocus()
    {
        HeaderBar.DOColor(Constants.WindowUnfocusColor, .1f);
    }
}
