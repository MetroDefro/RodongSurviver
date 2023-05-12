using RodongSurviver.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpView : ViewBase
{
    #region [ Properties ]
    public Button[] LevelUpButtons => levelUpButtons;
    public Text[] LevelUpButtonExplainTexts => levelUpButtonExplainTexts;
    #endregion

    #region [ Variables ]
    [SerializeField] private Button[] levelUpButtons;
    [SerializeField] private Text[] levelUpButtonExplainTexts;
    #endregion

    #region [ Public methods ]
    public void SetButton(int index, Sprite sprite, string text)
    {
        levelUpButtons[index].image.sprite = sprite;
        levelUpButtonExplainTexts[index].text = text;
    }

    public override void Show(Action onComplete)
    {
        gameObject.SetActive(true);
    }

    public override void ShowImmediate()
    {
        gameObject.SetActive(true);
    }

    public override void Hide(Action onComplete)
    {
        gameObject.SetActive(false);
    }

    public override void HideImmediate()
    {
        gameObject.SetActive(false);
    }

    public override void Dispose()
    {

    }
    #endregion
}
