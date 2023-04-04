using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EXPMarble : MonoBehaviour
{
    public EXPMarble Initialize(Player player, float exp)
    {
        SubscribeOnTriggerEnter2D(player, exp);

        return this;
    }

    private void SubscribeOnTriggerEnter2D(Player player, float exp)
    {
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.AddEXP(exp);
                    Destroy(gameObject);
                }
            }).AddTo(this);
    }
}
