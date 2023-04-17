using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;

public class TopCanvasPresenter : MonoBehaviour
{
    private TopCanvasView view;

    private Action onPause;

    public void Initialize(Action onPause)
    {
        if (TryGetComponent(out TopCanvasView view))
        {
            this.view = view;
        }

        this.onPause = onPause;

        view.InitializeView();
        SubscribePauseButton();
    }

    public void Dispose()
    {

    }

    public void SetLevelUp(int level)
    {
        view.SetLevelUp(level);
    }

    public void SetEXPbar(float normalizeEXP)
    {
        view.SetEXPbar(normalizeEXP);
    }    

    public void SetTimer(float spanSeconds)
    {
        TimeSpan spantime = TimeSpan.FromSeconds(spanSeconds);
        view.SetTimer(spantime);
    }

    private void SubscribePauseButton()
    {
        view.PauseButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                onPause.Invoke();
            }).AddTo(this);
    }
}
