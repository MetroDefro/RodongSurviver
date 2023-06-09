using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SwordWeapon : Weapon
{
    protected override float CalculateTerm()
    {
        return base.CalculateTerm() - (level - 1) * initData.Term * player.Status.WeaponTerm.Value * 0.2f;
    }

    protected override float CalculateSize()
    {
        return base.CalculateSize() + (level - 1) * initData.Size * player.Status.WeaponSize.Value * 0.1f;
    }

    protected override void Movement()
    {
        SubscribeWaiting();
        SubscribeMovement();
    }

    protected override void SetPosition()
    {
        base.SetPosition();
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            float Scala = 2;
            float angle = 360 * i / weaponObjects.Count;
            Vector3 AngleToVector3 = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
            weaponObjects[i].transform.localPosition = AngleToVector3 * Scala;
            weaponObjects[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (360 - angle)));
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

                    foreach (var weapon in weaponObjects)
                        weapon.SetActive(false);
                }
                else
                {
                    isWaitingTime = false;

                    foreach (var weapon in weaponObjects)
                        weapon.SetActive(true);

                    SetPosition();
                }
            })
            .AddTo(disposables);
    }

    private IEnumerator Waiting(IObserver<bool> observer)
    {
        while (true)
        {
            observer.OnNext(true);
            yield return new WaitForSeconds(CalculateTerm());

            observer.OnNext(false);
            yield return new WaitForSeconds(1);
        }
    }
}
