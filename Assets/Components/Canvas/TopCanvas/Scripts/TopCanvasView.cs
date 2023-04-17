using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopCanvasView : MonoBehaviour
{
    public Button PauseButton => pauseButton;

    [SerializeField] private RectTransform expBar;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI moneyText;

    private float maxEXPbarWidth;

    public void InitializeView()
    {
        maxEXPbarWidth = expBar.sizeDelta.x;
        expBar.sizeDelta = new Vector2(0, expBar.sizeDelta.y);

        levelText.text = "LV. " + 1;
    }

    public void SetLevelUp(int level)
    {
        levelText.text = "LV. " + level;
    }

    public void SetEXPbar(float normalizeEXP)
    {
        if (maxEXPbarWidth == 0)
            return;

        expBar.sizeDelta = new Vector2(maxEXPbarWidth * normalizeEXP, expBar.sizeDelta.y);
    }

    public void SetTimer(TimeSpan spantime)
    {
        timerText.text = spantime.ToString("mm' : 'ss");
    }
}