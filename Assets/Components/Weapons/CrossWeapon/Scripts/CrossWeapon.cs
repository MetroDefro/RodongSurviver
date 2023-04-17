using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CrossWeapon : WeaponBase
{
    [SerializeField] private float fallHeight = 10;

    protected override float CalculateTerm()
    {
        return base.CalculateTerm() - (level - 1) * data.Term * player.Status.WeaponTerm * 0.4f;
    }

    protected override float CalculateSize()
    {
        return base.CalculateSize() + (level - 1) * data.Size * player.Status.WeaponSize * 0.2f;
    }
    protected override float CalculateRange()
    {
        return base.CalculateRange() + (level - 1) * data.Range * 0.2f;
    }

    protected override int CalculateCount()
    {
        return base.CalculateCount() + Mathf.FloorToInt(level * 0.5f);
    }

    protected override void Movement()
    {
        SubscribeMovement();
    }

    protected override void SetPosition()
    {
        for (int i = 0; i < weaponObjects.Count; i++)
        {
            weaponObjects[i].transform.position = GetRandomRange(CalculateRange()) + new Vector2(0, fallHeight);
        }
    }

    private void SubscribeMovement()
    {
        float accumulateHeight = 0;
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                if (!isWaitingTime)
                {
                    foreach (var weapon in weaponObjects)
                        weapon.transform.position += (Vector3.down * CalculateSpeed() * Time.fixedDeltaTime);
                    accumulateHeight += CalculateSpeed() * Time.fixedDeltaTime;

                    if (accumulateHeight >= fallHeight)
                    {
                        accumulateHeight = 0;
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
