using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class PlayerSelectPresenter : MonoBehaviour
{
    private PlayerSelectView view;

    private Action<PlayerData> setPlayerData;
    private Action onPlay;
    private Action onShop;

    private LocalizedString localizedString;
    public void Initialize(Action onPlay, Action onShop, Action<PlayerData> setPlayerData)
    {
        if(TryGetComponent(out PlayerSelectView view))
        {
            this.view = view;
        }

        this.onPlay = onPlay;
        this.onShop = onShop;
        this.setPlayerData = setPlayerData;

        setPlayerData?.Invoke(view.PlayeButtonSets[0].Data);

        SubscribePlayButton();
        SubscribeShopButton();
        SubscribePlayerButtons();
        SubscribeSelectLanguageDropdown();

        localizedString = new LocalizedString("DefaultTable", 1000);
        localizedString.StringChanged += UpdateString;

    }

    public void Dispose()
    {
        localizedString.StringChanged -= UpdateString;
    }

    private void UpdateString(string str)
    {
        view.SelectLanguageText.text = str;
    }

    private void SubscribeSelectLanguageDropdown()
    {
        view.SelectLanguageDropdown.OnValueChangedAsObservable()
            .Subscribe(index =>
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            }).AddTo(this);
    }

    private void SubscribePlayButton()
    {
        view.PlayButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                onPlay?.Invoke();
            })
            .AddTo(this);
    }

    private void SubscribeShopButton()
    {
        view.ShopButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                onShop?.Invoke();
            })
            .AddTo(this);
    }

    private void SubscribePlayerButtons()
    {
        for(int i = 0; i < view.PlayeButtonSets.Count; i++)
        {
            int index = i;

            view.PlayeButtonSets[index].Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PlayerData playerData = view.PlayeButtonSets[index].Data;
                    setPlayerData?.Invoke(playerData);
                })
                .AddTo(this);
        }
    }    
}