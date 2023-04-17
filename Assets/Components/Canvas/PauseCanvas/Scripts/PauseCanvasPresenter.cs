using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PauseCanvasPresenter : MonoBehaviour
{
    private PauseCanvasView view;

    private Action onPlay;

    public void Initialize(Action onPlay)
    {
        if (TryGetComponent(out PauseCanvasView view))
        {
            this.view = view;
        }

        this.onPlay = onPlay;

        gameObject.SetActive(false);
        SubscribePlayButton();
    }

    private void SubscribePlayButton()
    {
        view.PlayButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                onPlay.Invoke();
            }).AddTo(this);
    }
}
