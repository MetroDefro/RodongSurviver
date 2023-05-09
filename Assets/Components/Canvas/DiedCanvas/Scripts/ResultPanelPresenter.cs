using RodongSurviver.Base;
using UnityEngine;
using UniRx;
using System;

namespace RodongSurviver.Components.DiedPanel
{
    [RequireComponent(typeof(ResultPanelView))]
    public class ResultPanelPresenter : PresenterBase
    {
        public class DiedPanelActions 
        {
            public Action RetryEvent { get; set; }
            public Action HomeEvent { get; set; }
        }
        #region [ Variables ]
        private ResultPanelView view;
        private DiedPanelActions actions;
        #endregion

        #region [ MonoBehaviour Messages ]
        private void Awake()
        {
            view = GetComponent<ResultPanelView>();
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

        public void Show(bool isGameClear)
        {
            view.GameClaerText.gameObject.SetActive(isGameClear);
            view.GameOverText.gameObject.SetActive(!isGameClear);

            view.ShowImmediate();
        }

        public void Hide()
        {
            view.HideImmediate();
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
