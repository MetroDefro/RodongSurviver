using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using RodongSurviver.Base;

public class LevelUpPresenter : PresenterBase
{
    #region [ Variables ]
    private LevelUpView view;

    private Action<ItemType> onItemLevelUp;
    private CompositeDisposable levelupButtonDisposables = new CompositeDisposable();
    #endregion

    #region [ Public methods ]
    public void Initialize(Action<ItemType> onItemLevelUp)
    {
        if (TryGetComponent(out LevelUpView view))
        {
            this.view = view;
        }

        this.onItemLevelUp = onItemLevelUp;

        gameObject.SetActive(false);
    }

    public override void Dispose()
    {
        view?.Dispose();
    }

    public void SetLevelUpPanel(ItemData[] items)
    {
        view.ShowImmediate();

        int count = view.LevelUpButtons.Length;
        for (int i = 0; i < count; i++)
        {
            view.SetButton(i, items[i].Sprite, items[i].Explanation);
            SubscribeLevelUpButton(view.LevelUpButtons[i], items[i].Type);
        }
    }
    #endregion

    #region [ Private methods ]
    private void SubscribeLevelUpButton(Button button, ItemType type)
    {
        button.OnClickAsObservable()
            .ThrottleFirst(ClickThrottleFirstTime)
            .Subscribe(_ =>
            {
                onItemLevelUp.Invoke(type);
                levelupButtonDisposables.Clear();

                view.HideImmediate();
            })
            .AddTo(levelupButtonDisposables);
    }
    #endregion
}
