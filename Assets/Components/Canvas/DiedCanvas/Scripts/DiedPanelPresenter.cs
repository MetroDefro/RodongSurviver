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
            public Action HomeEvent { get; set; }
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
            RegisterHomeButtonOnClick();
        }
        #endregion

        #region [ Public Methods ]
        public void Initialize(DiedPanelActions actions)
        {
            gameObject.SetActive(false);
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

        private void RegisterHomeButtonOnClick()
            => view?.HomeButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                actions?.HomeEvent.Invoke();
            }).AddTo(Disposables);
        #endregion
    }
}
