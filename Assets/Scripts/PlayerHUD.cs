using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private RectTransform expBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Button[] levelUpButton;
    [SerializeField] private TextMeshProUGUI[] levelUpButtonExplainText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image[] weaponSlot;
    [SerializeField] private Image[] itemSlot;
    [SerializeField] private TextMeshProUGUI[] weaponSlotLevelText;
    [SerializeField] private TextMeshProUGUI[] itemSlotLevelText;

    private float maxEXPbarWidth;

    private Action<WeaponType> onWeaponUp;

    private CompositeDisposable levelupButtonDisposables = new CompositeDisposable();

    public void Initialize(Action<WeaponType> onWeaponUp)
    {
        this.onWeaponUp = onWeaponUp;
        levelUpPanel.SetActive(false);

        maxEXPbarWidth = expBar.sizeDelta.x;
        expBar.sizeDelta = new Vector2(0, expBar.sizeDelta.y);

        levelText.text = "LV. " + 1;
    }

    public void Dispose()
    {
        for(int i = 0; i < weaponSlot.Length; i++)
        {
            SetWeaponSlot(i, 1, null);
        }
    }

    public void SetEXPbar(float normalizeEXP)
    {
        if (maxEXPbarWidth == 0)
            return;

        expBar.sizeDelta = new Vector2(maxEXPbarWidth * normalizeEXP, expBar.sizeDelta.y);
    }    

    public void SetLevelUpPanel(int level, Weapon[] weapons)
    {
        levelText.text = "LV. " + level;
        levelUpPanel.SetActive(true);
        for(int i = 0; i < levelUpButton.Length; i++)
        {
            levelUpButton[i].image.sprite = weapons[i].IconSprite;
            levelUpButtonExplainText[i].text = weapons[i].Explanation;
            SubscribeLevelUpButton(levelUpButton[i], weapons[i].WeaponType);
        }
    }

    public void SetWeaponSlot(int index, int Level, Sprite sprite)
    {
        weaponSlot[index].sprite = sprite;
        weaponSlotLevelText[index].text = "" + Level;
    }

    public void SetTimer(float spanSeconds)
    {
        TimeSpan spantime = TimeSpan.FromSeconds(spanSeconds);
        timerText.text = spantime.ToString("mm' : 'ss");
    }

    private void SubscribeLevelUpButton(Button button, WeaponType weaponType)
    {
        button.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => 
            {
                onWeaponUp.Invoke(weaponType);
                levelUpPanel.SetActive(false);
                levelupButtonDisposables.Clear();
            })
            .AddTo(levelupButtonDisposables);
    }
}
