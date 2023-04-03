using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private RectTransform HPbar;
    [SerializeField] private RectTransform EXPbar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Button[] levelUpButton;
    private float maxHPbarWidth;
    private float maxEXPbarWidth;

    private Action onLevelUp;
    private Action<WeaponType> onWeaponUp;

    private CompositeDisposable disposables = new CompositeDisposable();

    public void Initialize(Action onLevelUp, Action<WeaponType> onWeaponUp)
    {
        this.onLevelUp = onLevelUp;
        this.onWeaponUp = onWeaponUp;
        levelUpPanel.SetActive(false);

        maxHPbarWidth = HPbar.sizeDelta.x;
        maxEXPbarWidth = EXPbar.sizeDelta.x;
        EXPbar.sizeDelta = new Vector2(0, EXPbar.sizeDelta.y);
    }

    public void SetHPbar(float normalizeHP)
    {
        if (maxHPbarWidth == 0)
            return;

        HPbar.sizeDelta = new Vector2(maxHPbarWidth * normalizeHP, HPbar.sizeDelta.y);
    }

    public void SetEXPbar(float normalizeEXP)
    {
        if (maxEXPbarWidth == 0)
            return;

        EXPbar.sizeDelta = new Vector2(maxEXPbarWidth * normalizeEXP, EXPbar.sizeDelta.y);
    }    
    
    public void SetLevelUp(int level)
    {
        levelText.text = "LV. " + level;
        onLevelUp?.Invoke();
    }

    public void SetLevelUpPanel((WeaponType, Sprite)[] weapons)
    {
        levelUpPanel.SetActive(true);
        for(int i = 0; i < levelUpButton.Length; i++)
        {
            levelUpButton[i].image.sprite = weapons[i].Item2;
            SubscribeLevelUpButton(levelUpButton[i], weapons[i].Item1);
        }
    }

    private void SubscribeLevelUpButton(Button button, WeaponType weaponType)
    {
        button.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => 
            {
                onWeaponUp.Invoke(weaponType);
                levelUpPanel.SetActive(false);
                disposables.Clear();
            })
            .AddTo(disposables);
    }
}
