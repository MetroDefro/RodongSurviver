using RodongSurviver.Base;
using System;
using UniRx;

public class PauseCanvasPresenter : PresenterBase
{
    #region [ Variables ]
    private PauseCanvasView view;

    private Action onPlay;
    #endregion

    #region [ Public methods ]
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
    #endregion
}
