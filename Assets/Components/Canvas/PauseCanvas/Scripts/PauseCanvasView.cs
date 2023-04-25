using RodongSurviver.Base;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasView : ViewBase
{
    #region [ Properties ]
    public Button PlayButton => playButton;
    public Button RetryButton => retryButton;
    public Button HomeButton => homeButton;
    #endregion

    #region [ Variables ]
    [SerializeField] private Button playButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;
    #endregion

    #region [ Public methods ]
    public override void Show(Action onComplete)
    {

    }

    public override void ShowImmediate()
    {

    }

    public override void Hide(Action onComplete)
    {

    }

    public override void HideImmediate()
    {

    }

    public override void Dispose()
    {

    }
    #endregion
}
