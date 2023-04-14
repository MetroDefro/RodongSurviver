using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPresenter : MonoBehaviour
{
    private LevelUpView view;

    private Action<WeaponType> onWeaponUp;
    private CompositeDisposable levelupButtonDisposables = new CompositeDisposable();

    public void Initialize(Action<WeaponType> onWeaponUp)
    {
        if (TryGetComponent(out LevelUpView view))
        {
            this.view = view;
        }

        this.onWeaponUp = onWeaponUp;

        gameObject.SetActive(false);
    }

    public void Dispose()
    {

    }

    public void SetLevelUpPanel(Weapon[] weapons)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < view.LevelUpButtons.Length; i++)
        {
            view.LevelUpButtons[i].image.sprite = weapons[i].IconSprite;
            view.LevelUpButtonExplainTexts[i].text = weapons[i].Explanation;
            SubscribeLevelUpButton(view.LevelUpButtons[i], weapons[i].WeaponType);
        }
    }

    private void SubscribeLevelUpButton(Button button, WeaponType weaponType)
    {
        button.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                onWeaponUp.Invoke(weaponType);
                gameObject.SetActive(false);
                levelupButtonDisposables.Clear();
            })
            .AddTo(levelupButtonDisposables);
    }
}
