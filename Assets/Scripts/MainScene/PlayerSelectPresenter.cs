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

    public class PlayerSelectAction
    {
        public Action<PlayerData> SetPlayerData;
        public Action OnPlay;
        public Action OnShop;
        public Action<int> OnSelectLangauge;
    }

    private PlayerSelectAction actions;

    public void Initialize(PlayerSelectAction actions, int currentLangaugeIndex)
    {
        if (TryGetComponent(out PlayerSelectView view))
        {
            this.view = view;
        }

        this.actions = actions;

        this.actions.SetPlayerData?.Invoke(view.PlayeButtonSets[0].Data);

        view.SelectLanguageDropdown.value = currentLangaugeIndex;

        SubscribePlayButton();
        SubscribeShopButton();
        SubscribePlayerButtons();
        SubscribeSelectLanguageDropdown();
    }

    private void SubscribeSelectLanguageDropdown()
    {
        view.SelectLanguageDropdown.OnValueChangedAsObservable()
            .Subscribe(index => actions.OnSelectLangauge?.Invoke(index))
            .AddTo(this);
    }

    private void SubscribePlayButton()
    {
        view.PlayButton.OnClickAsObservable()
            .Subscribe(_ => actions.OnPlay?.Invoke())
            .AddTo(this);
    }

    private void SubscribeShopButton()
    {
        view.ShopButton.OnClickAsObservable()
            .Subscribe(_ => actions.OnShop?.Invoke())
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
                    actions.SetPlayerData?.Invoke(playerData);
                })
                .AddTo(this);
        }
    }    
}