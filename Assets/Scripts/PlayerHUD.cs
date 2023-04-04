using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private RectTransform hpBar;
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

    private float maxHPbarWidth;
    private float maxEXPbarWidth;

    private Action<WeaponType> onWeaponUp;

    private CompositeDisposable disposables = new CompositeDisposable();

    public void Initialize(Action<WeaponType> onWeaponUp)
    {
        this.onWeaponUp = onWeaponUp;
        levelUpPanel.SetActive(false);

        maxHPbarWidth = hpBar.sizeDelta.x;
        maxEXPbarWidth = expBar.sizeDelta.x;
        expBar.sizeDelta = new Vector2(0, expBar.sizeDelta.y);

        SubscribeEveryUpdate();
    }

    public void SetHPbar(float normalizeHP)
    {
        if (maxHPbarWidth == 0)
            return;

        hpBar.sizeDelta = new Vector2(maxHPbarWidth * normalizeHP, hpBar.sizeDelta.y);
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
            levelUpButton[i].image.sprite = weapons[i].Sprite;
            levelUpButtonExplainText[i].text = weapons[i].Explanation;
            SubscribeLevelUpButton(levelUpButton[i], weapons[i].WeaponType);
        }
    }

    public void SetWeaponSlot(int index, int Level, Sprite sprite)
    {
        weaponSlot[index].sprite = sprite;
        weaponSlotLevelText[index].text = "" + Level;
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

    private void SubscribeEveryUpdate()
    {
        timerText.text = "00 : 00";
        float spanSeconds = 0;
        Observable.EveryUpdate()
            .Subscribe(_ => 
            {
                spanSeconds += Time.deltaTime;
                TimeSpan spantime = TimeSpan.FromSeconds(spanSeconds);
                timerText.text = spantime.ToString("mm' : 'ss");
            }).AddTo(this);
    }
}
