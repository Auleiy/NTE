using NTE.UI;
using UnityEngine;

[RequireComponent(typeof(Hidable))]
public abstract class TabEditor : MonoBehaviour
{
    public delegate void OnEditEventHandler();

    public Hidable Hidable => GetComponent<Hidable>();

    public bool Edited;

    public OnEditEventHandler OnEdit;

    protected void Edit()
    {
        Edited = true;
        OnEdit?.Invoke();
    }
}
