using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HammerWeapon : WeaponBase
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
        return data.Term - (level - 1) * data.Term * 0.4f;
    }

    protected override float CalculateSize()
    {
        return data.Size + (level - 1) * data.Size * 0.2f;
    }
    protected override float CalculateRange()
    {
        return data.Range * (level - 1) * data.Range * 0.2f;
    }

    protected override int CalculateCount()
    {
        return data.Count + Mathf.FloorToInt(level * 0.5f);
    }

    protected override void Movement()
    {
        SubscribeMovement();
    }

    protected override void SetPosition()
    {
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            weaponObjects[i].transform.position = GetRandomRange(CalculateRange());
            weaponObjects[i].transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void SubscribeMovement()
    {
        float accumulateRotate = 0;
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                if (!isWaitingTime)
                {
                    foreach (var weapon in weaponObjects)
                        weapon.transform.Rotate(Vector3.forward * CalculateSpeed() * Time.fixedDeltaTime);
                    accumulateRotate += CalculateSpeed() * Time.fixedDeltaTime;

                    if (accumulateRotate >= 90)
                    {
                        accumulateRotate = 0;
                        SubscribeWaiting();
                    }
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
            });
    }

    private Vector2 GetRandomRange(float spwanRange)
    {
        return new Vector2(UnityEngine.Random.Range(player.transform.position.x - spwanRange, player.transform.position.x + spwanRange)
                , UnityEngine.Random.Range(player.transform.position.y - spwanRange, player.transform.position.y + spwanRange));
    }

    private IEnumerator Waiting(IObserver<bool> observer)
    {
        observer.OnNext(true);
        yield return new WaitForSeconds(CalculateTerm());

        observer.OnNext(false);
    }
}
