using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EXPMarble : MonoBehaviour
{
    public void Dispose()
    {
        Destroy(gameObject);
    }

    public EXPMarble Initialize(Player player, float exp, Action<EXPMarble> onDestroed)
    {
        SubscribeOnTriggerEnter2D(player, exp, onDestroed);
        SubscribeUpdate(player);

        return this;
    }

    private void SubscribeOnTriggerEnter2D(Player player, float exp, Action<EXPMarble> onDestroed)
    {
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.AddEXP(exp);
                    onDestroed.Invoke(this);
                    Destroy(gameObject);
                }
            }).AddTo(this);
    }

    private void SubscribeUpdate(Player player)
    {
        float moveSpeed = 10;
        Observable.EveryUpdate()
            .Subscribe(_ => 
            {
                if(Mathf.Abs(Vector2.Distance(player.transform.position, transform.position)) <= player.Status.Magnetism)
                {
                    Vector2 moveVector = Vector2.Lerp(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                    transform.position = moveVector;
                }
            })
            .AddTo(this);
    }
}