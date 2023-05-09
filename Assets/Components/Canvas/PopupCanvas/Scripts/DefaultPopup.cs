using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DefaultPopup : PopupBase
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Text content;

    #region [ Override Methods ]
    public override PopupBase SetButton(string button)
    {
        return this;
    }

    public override PopupBase SetContent(string content)
    {
        this.content.text = content;
        return this;
    }

    public override PopupBase SetTitle(string title)
    {
        return this;
    }

    public override void Dispose() 
    {
        onConfirm = null;
        onCancel = null;
        disposables.Clear();
    }

    public override void Hide(Action onComplete) 
    {
        HideImmediate();
        onComplete?.Invoke();
    }

    public override void HideImmediate()
    {
        Dispose();
        gameObject.SetActive(false);
    }

    public override void Show(Action onComplete) 
    {
        ShowImmediate();
        onComplete?.Invoke();
    }

    public override void ShowImmediate()
    {
        SubscribeConfirmButton();
        gameObject.SetActive(true);
    }
    #endregion


    private void SubscribeConfirmButton()
    {
        confirmButton.OnClickAsObservable().Subscribe(_ => 
        {
            onConfirm?.Invoke();
        }).AddTo(disposables);
    }
}
