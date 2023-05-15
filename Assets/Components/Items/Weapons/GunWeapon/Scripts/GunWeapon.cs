using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class GunWeapon : Weapon
{
    protected override float CalculateTerm()
    {
        return base.CalculateTerm() - (level - 1) * initData.Term * player.Status.WeaponTerm.Value * 0.2f;
    }

    protected override float CalculateSize()
    {
        return base.CalculateSize() + (level - 1) * initData.Size * player.Status.WeaponSize.Value * 0.05f;
    }

    protected override int CalculateCount()
    {
        return base.CalculateCount() + Mathf.FloorToInt((level - 1) * 0.5f);
    }

    protected override void Movement()
    {
        SubscribeWaiting();
        SubscribeMovement();
    }

    protected override void SetPosition()
    {

    }

    private void SetPosition(GameObject gameObject)
    {
        float Scala = 2;
        gameObject.transform.localPosition = player.InputVector2 * Scala;
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (360 - Mathf.Atan2(player.InputVector2.x, player.InputVector2.y) * 180 / Mathf.PI)));
    }

    private void SubscribeMovement()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                transform.position = player.transform.position;
                foreach (var weapon in weaponObjects)
                    weapon.transform.Translate(Vector3.up * CalculateSpeed() * Time.fixedDeltaTime);
            }).AddTo(disposables);
    }

    private void SubscribeWaiting()
    {
        int index = 0;
        Observable.FromCoroutine<bool>(observer => Waiting(observer))
            .Subscribe(value => 
            {
                if (value)
                {
                    SetPosition(weaponObjects[index]);

                    if (index >= weaponObjects.Count - 1)
                        index = 0;
                    else
                        index++;
                }
            }).AddTo(disposables);
    }

    private IEnumerator Waiting(IObserver<bool> observer)
    {
        while (true)
        {
            observer.OnNext(true);
            yield return new WaitForSeconds(CalculateTerm());

            observer.OnNext(false);
            yield return new WaitForEndOfFrame();
        }
    }
}
