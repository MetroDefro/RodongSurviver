using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Money : MonoBehaviour
{
    public void Dispose()
    {
        Destroy(gameObject);
    }

    public Money Initialize(Player player, int money, Action<Money> onDestroed)
    {
        SubscribeOnTriggerEnter2D(player, money, onDestroed);

        return this;
    }

    private void SubscribeOnTriggerEnter2D(Player player, int money, Action<Money> onDestroed)
    {
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.AddMoney(money);
                    onDestroed.Invoke(this);
                    Destroy(gameObject);
                }
            }).AddTo(this);
    }
}
