using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public void Dispose()
    {
        Destroy(gameObject);
    }

    public Potion Initialize(Player player, int recover, Action<Potion> onDestroed)
    {
        SubscribeOnTriggerEnter2D(player, recover, onDestroed);

        return this;
    }

    private void SubscribeOnTriggerEnter2D(Player player, int recover, Action<Potion> onDestroed)
    {
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.Status.PlusHP(recover);
                    onDestroed.Invoke(this);
                    Destroy(gameObject);
                }
            }).AddTo(this);
    }
}
