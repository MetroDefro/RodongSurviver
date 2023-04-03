using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EXPMarble : MonoBehaviour
{
    private Player player;
    private float exp;

    public EXPMarble Initialize(Player player, float exp)
    {
        this.player = player;
        this.exp = exp;

        SubscribeOnTriggerEnter2D();

        return this;
    }

    private void SubscribeOnTriggerEnter2D()
    {
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.PlusEXP(exp);
                    Destroy(gameObject);
                }
            }).AddTo(this);
    }
}
