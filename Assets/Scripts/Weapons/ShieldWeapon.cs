using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShieldWeapon : Weapon
{
    protected override float CalculateDamage()
    {
        return data.Damage * level * 1;
    }

    protected override void Movement()
    {
        SubscribeMovement();
    }

    private void SubscribeMovement()
    {
        float time = 0;
        Observable.EveryFixedUpdate()
            .Subscribe(_ => 
            {
                time += Time.deltaTime;

                if(time >= 6)
                {
                    time = 0;
                }
                else if (time >= 5)
                {
                    gameObject.SetActive(false);
                }
                else if (time >= 3)
                {
                    transform.position = player.transform.position + new Vector3(-1, 0, 0);
                    gameObject.SetActive(true);
                }
                else if (time >= 2)
                {
                    gameObject.SetActive(false);
                }
                else if (time >= 0)
                {
                    transform.position = player.transform.position + new Vector3(1, 0, 0);
                    gameObject.SetActive(true);
                }
            }).AddTo(this);
    }
}
