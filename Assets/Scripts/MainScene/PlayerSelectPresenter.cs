using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerSelectPresenter : MonoBehaviour
{
    private PlayerSelectView view;

    private Action<PlayerData> setPlayerData;
    private Action onPlay;

    public void Initialize(Action onPlay, Action<PlayerData> setPlayerData)
    {
        if(TryGetComponent(out PlayerSelectView view))
        {
            this.view = view;
        }

        this.onPlay = onPlay;
        this.setPlayerData = setPlayerData;

        setPlayerData?.Invoke(view.PlayeButtonSets[0].Data);

        SubscribePlayButton();
        SubscribePlayerButtons();
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