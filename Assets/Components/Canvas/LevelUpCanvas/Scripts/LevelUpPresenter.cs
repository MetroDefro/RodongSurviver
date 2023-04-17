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

    private Action<WeaponType> onWeaponUp;
    private CompositeDisposable levelupButtonDisposables = new CompositeDisposable();
    #endregion

    #region [ Public methods ]
    public void Initialize(Action<WeaponType> onWeaponUp)
    {
        if (TryGetComponent(out LevelUpView view))
        {
            this.view = view;
        }

        this.onWeaponUp = onWeaponUp;

        gameObject.SetActive(false);
    }

    public override void Dispose()
    {
        view?.Dispose();
    }

    public void SetLevelUpPanel(WeaponBase[] weapons)
    {
        view.ShowImmediate();

        int count = view.LevelUpButtons.Length;
        for (int i = 0; i < count; i++)
        {
            view.SetButton(i, weapons[i].IconSprite, weapons[i].Explanation);
            SubscribeLevelUpButton(view.LevelUpButtons[i], weapons[i].WeaponType);
        }
    }
    #endregion

    #region [ Private methods ]
    private void SubscribeLevelUpButton(Button button, WeaponType weaponType)
    {
        button.OnClickAsObservable()
            .ThrottleFirst(ClickThrottleFirstTime)
            .Subscribe(_ =>
            {
                onWeaponUp.Invoke(weaponType);
                levelupButtonDisposables.Clear();

                view.HideImmediate();
            })
            .AddTo(levelupButtonDisposables);
    }
    #endregion
}
