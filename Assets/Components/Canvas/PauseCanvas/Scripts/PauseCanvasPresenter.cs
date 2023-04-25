using RodongSurviver.Base;
using System;
using UniRx;

public class PauseCanvasPresenter : PresenterBase
{
    #region [ Variables ]
    private PauseCanvasView view;

    public class PauseCanvasActions
    {
        public Action OnPlay { get; set; }
        public Action OnRetry { get; set; }
        public Action OnHome { get; set; }
    }

    private PauseCanvasActions actions;
    #endregion

    #region [ Public methods ]
    public void Initialize(PauseCanvasActions actions)
    {
        if (TryGetComponent(out PauseCanvasView view))
        {
            this.view = view;
        }

        this.actions = actions;

        gameObject.SetActive(false);
        SubscribePlayButton();
        SubscribeRetryButton();
        SubscribeHomeButton();
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
                actions.OnPlay.Invoke();
            }).AddTo(this);
    }

    private void SubscribeRetryButton()
    {
        view.RetryButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                actions.OnRetry.Invoke();
            }).AddTo(this);
    }    
    
    private void SubscribeHomeButton()
    {
        view.HomeButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                actions.OnHome.Invoke();
            }).AddTo(this);
    }
    #endregion
}
