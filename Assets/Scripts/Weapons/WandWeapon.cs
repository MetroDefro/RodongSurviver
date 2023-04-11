using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class WandWeapon : Weapon
{
    protected override float CalculateDamage()
    {
        return data.Damage * Level * 1;
    }

    protected override float CalculateSpeed()
    {
        return data.Speed;
    }

    protected override float CalculateTerm()
    {
        return data.Term - (level - 1) * data.Term * 0.2f;
    }

    protected override float CalculateSize()
    {
        return data.Size + (level - 1) * data.Size * 0.1f;
    }
    protected override float CalculateRange()
    {
        return data.Range;
    }

    protected override int CalculateCount()
    {
        return data.Count * level;
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
            float angle = UnityEngine.Random.Range(0, 360);
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

                    SetPosition();
                }
                else
                {
                    isWaitingTime = false;

                    foreach (var weapon in weaponObjects)
                        weapon.SetActive(true);
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
            yield return new WaitForSeconds(1);
        }
    }
}
