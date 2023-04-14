using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;

public class PlayerHUDPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform expBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI moneyText;

    private float maxEXPbarWidth;

    private Action onPause;
    private Action onPlay;

    public void Initialize(Action onPause, Action onPlay)
    {
        this.onPause = onPause;
        this.onPlay = onPlay;

        maxEXPbarWidth = expBar.sizeDelta.x;
        expBar.sizeDelta = new Vector2(0, expBar.sizeDelta.y);

        levelText.text = "LV. " + 1;

        SubscribePauseButton();
        SubscribePlayButton();
    }

    public void Dispose()
    {

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

    public void SetTimer(float spanSeconds)
    {
        TimeSpan spantime = TimeSpan.FromSeconds(spanSeconds);
        timerText.text = spantime.ToString("mm' : 'ss");
    }

    private void SubscribePauseButton()
    {
        pauseButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                pausePanel.SetActive(true);
                onPause.Invoke();
            }).AddTo(this);
    }

    private void SubscribePlayButton()
    {
        playButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
                pausePanel.SetActive(false);
                onPlay.Invoke();
            }).AddTo(this);
    }
}
