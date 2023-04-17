using RodongSurviver.Base;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RodongSurviver.Components.DiedPanel
{
    public class DiedPanelView : ViewBase
    {
        #region [ Variables ]
        [SerializeField] private Button retryButton;
        #endregion

        #region [ Properties ]
        public Button RetryButton => retryButton;
        #endregion

        #region [ Override Methods ]
        public override void Dispose() { }

        public override void Hide(Action onComplete) { }

        public override void HideImmediate() { }

        public override void Show(Action onComplete) { }

        public override void ShowImmediate() { }
        #endregion

        #region [ Public Methods ]

        #endregion
    }
}
