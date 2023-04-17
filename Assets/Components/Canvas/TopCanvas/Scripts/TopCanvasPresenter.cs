using UniRx;
using System;
using RodongSurviver.Base;

public class TopCanvasPresenter : PresenterBase
{
    #region [ Variables ]
    private TopCanvasView view;

    private Action onPause;
    #endregion

    #region [ Public methods ]
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

    public override void Dispose()
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
    #endregion

    #region [ Private methods ]
    private void SubscribePauseButton()
    {
        view.PauseButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                onPause.Invoke();
            }).AddTo(this);
    }
    #endregion
}
