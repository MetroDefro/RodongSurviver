using RodongSurviver.Base;
using UnityEngine;
using UniRx;
using System;

namespace RodongSurviver.Components.DiedPanel
{
    [RequireComponent(typeof(DiedPanelView))]
    public class DiedPanelPresenter : PresenterBase
    {
        public class DiedPanelActions 
        {
            public Action RetryEvent { get; set; }
        }
        #region [ Variables ]
        private DiedPanelView view;
        private DiedPanelActions actions;
        #endregion

        #region [ MonoBehaviour Messages ]
        private void Awake()
        {
            view = GetComponent<DiedPanelView>();
        }

        private void OnEnable()
        {
            RegisterRetryButtonOnClick();
        }
        #endregion

        #region [ Public Methods ]
        public void Initialize(DiedPanelActions actions)
        {
            this.actions = actions;
        }
        #endregion

        #region [ Register Methods ]
        private void RegisterRetryButtonOnClick()
            => view?.RetryButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                actions?.RetryEvent.Invoke();
            }).AddTo(Disposables);
        #endregion
    }
}
