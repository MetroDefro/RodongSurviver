using RodongSurviver.Base;
using System;
using UniRx;

public class PauseCanvasPresenter : PresenterBase
{
    #region [ Variables ]
    private PauseCanvasView view;

    private Action onPlay;
    private Action onRetry;
    #endregion

    #region [ Public methods ]
    public void Initialize(Action onPlay, Action onRetry)
    {
        if (TryGetComponent(out PauseCanvasView view))
        {
            this.view = view;
        }

        this.onPlay = onPlay;
        this.onRetry = onRetry;

        gameObject.SetActive(false);
        SubscribePlayButton();
        SubscribeRetryButton();
    }
    #endregion

    #region [ Private methods ]
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

    private void SubscribeRetryButton()
    {
        view.RetryButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                onRetry.Invoke();
            }).AddTo(this);
    }
    #endregion
}
