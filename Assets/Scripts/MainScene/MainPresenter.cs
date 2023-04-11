using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MainPresenter : MonoBehaviour
{
    [SerializeField] private Button[] playerButtons;
    [SerializeField] private Button playButton;

    private void Start()
    {
        SubscribePlayButton();
        SubscribePlayerButtons();
    }

    private void SubscribePlayButton()
    {
        playButton.OnClickAsObservable()
            .Subscribe(_ => 
            {

            })
            .AddTo(this);
    }

    private void SubscribePlayerButtons()
    {
        foreach (var button in playerButtons)
        {
            button.OnClickAsObservable()
                .Subscribe(_ => 
                {

                })
                .AddTo(this);
        }
    }
}
