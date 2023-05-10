using RodongSurviver.Base;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RodongSurviver.Components.DiedPanel
{
    public class ResultPanelView : ViewBase
    {
        #region [ Properties ]
        public Button RetryButton => retryButton;
        public Button HomeButton => homeButton;
        public TextMeshProUGUI GameOverText => gameoverText;
        public TextMeshProUGUI GameClaerText => gameclaerText;
        #endregion

        #region [ Variables ]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private TextMeshProUGUI gameoverText;
        [SerializeField] private TextMeshProUGUI gameclaerText;
        #endregion

        #region [ Override Methods ]
        public override void Dispose() { }

        public override void Hide(Action onComplete) { }

        public override void HideImmediate() 
        { 
            gameObject.SetActive(false);
        }

        public override void Show(Action onComplete) { }

        public override void ShowImmediate()
        {
            gameObject.SetActive(true);
        }
        #endregion

        #region [ Public Methods ]

        #endregion
    }
}

