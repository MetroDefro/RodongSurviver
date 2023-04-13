using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RodongSurviver.Base;
using System;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class GameSceneView : ViewBase
{
    [SerializeField] private RectTransform expBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button[] levelUpButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private TextMeshProUGUI[] levelUpButtonExplainText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image[] weaponSlot;
    [SerializeField] private Image[] itemSlot;
    [SerializeField] private TextMeshProUGUI[] weaponSlotLevelText;
    [SerializeField] private TextMeshProUGUI[] itemSlotLevelText;

    public override void Dispose()
    {

    }

    public override void Hide(Action onComplete)
    {

    }

    public override void HideImmediate()
    {

    }

    public override void Show(Action onComplete)
    {

    }

    public override void ShowImmediate()
    {

    }
}
