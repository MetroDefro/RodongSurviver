using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GunWeapon : Weapon
{
    protected override float CalculateTerm()
    {
        return base.CalculateTerm() - (level - 1) * initData.Term * player.Status.WeaponTerm * 0.2f;
    }

    protected override float CalculateSize()
    {
        return base.CalculateSize() + (level - 1) * initData.Size * player.Status.WeaponSize * 0.1f;
    }

    protected override void Movement()
    {
        SubscribeWaiting();
        SubscribeMovement();
    }

    protected override void SetPosition()
    {
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            float Scala = 2;
            weaponObjects[i].transform.localPosition = player.InputVector2 * Scala;
            weaponObjects[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (360 - Mathf.Atan2(player.InputVector2.x, player.InputVector2.y) * 180 / Mathf.PI)));
        }
    }

    private void SubscribeMovement()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                transform.position = player.transform.position;
                if (!isWaitingTime)
                {
                    foreach (var weapon in weaponObjects)
                        weapon.transform.Translate(Vector3.up * CalculateSpeed() * Time.fixedDeltaTime);
                }
            }).AddTo(disposables);
    }

    private void SubscribeWaiting()
    {
        Observable.FromCoroutine<bool>(observer => Waiting(observer))
            .Subscribe(value =>
            {
                if (value)
                {
                    isWaitingTime = true;

                    SetPosition();
                }
                else
                {
                    isWaitingTime = false;
                }
            })
            .AddTo(disposables);
    }

    private IEnumerator Waiting(IObserver<bool> observer)
    {
        while (true)
        {
            observer.OnNext(false);
            yield return new WaitForSeconds(CalculateTerm());

            observer.OnNext(true);
        }
    }
}
