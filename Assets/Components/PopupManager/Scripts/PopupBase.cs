using System;
using UnityEngine;
using UniRx;

public abstract class PopupBase : MonoBehaviour
{
    protected Action onConfirm;
    protected Action onCancel;

    protected CompositeDisposable disposables = new CompositeDisposable();
    
    public virtual PopupBase SetConfirm(Action onConfirm)
    {
        this.onConfirm = onConfirm;
        return this;
    }

    public virtual PopupBase SetCancel(Action onCancel)
    {
        this.onCancel = onCancel;
        return this;
    }

    #region [ Abstract Methods ]
    public abstract PopupBase SetContent(string content);
    public abstract PopupBase SetTitle(string title);
    public abstract PopupBase SetButton(string button);

    public abstract void Dispose();
    public abstract void Hide(Action onComplete);
    public abstract void HideImmediate();
    public abstract void Show(Action onComplete);
    public abstract void ShowImmediate();
    #endregion
}
