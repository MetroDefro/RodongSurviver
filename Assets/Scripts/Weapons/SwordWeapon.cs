using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SwordWeapon : Weapon
{
    protected override float CalculateDamage()
    {
        return data.Damage * Level * 1;
    }

    protected override float CalculateSpeed()
    {
        return data.Speed * Level * 1;
    }

    protected override float CalculateSize()
    {
        return data.Size * Level * 1;
    }

    protected override int CalculateCount()
    {
        return data.Count * Level * 1;
    }

    protected override void Movement()
    {
        SubscribeMovement();
    }

    protected override void SetPosition()
    {
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            weaponObjects[i].transform.position = new Vector3(2, 0, 0);
            weaponObjects[i].transform.Rotate(Vector3.right * 360 * i / weaponObjects.Count);
        }
    }

    private void SubscribeMovement()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                transform.position = player.transform.position;
                transform.Rotate(Vector3.back * data.Speed * Time.fixedDeltaTime);
            }).AddTo(this);
    }
}
