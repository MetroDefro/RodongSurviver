using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GunWeapon : Weapon
{
    [SerializeField] private int fireSpeed = 20;
    private bool isWaitingTime;

    protected override float CalculateDamage()
    {
        return data.Damage * Level * 1;
    }

    protected override float CalculateSpeed()
    {
        return data.Speed - (level - 1) * data.Speed * 0.4f;
    }

    protected override float CalculateSize()
    {
        return data.Size + (level - 1) * data.Size * 0.1f;
    }
    protected override float CalculateRange()
    {
        return data.Range * (level - 1) * data.Range * 0.2f;
    }

    protected override int CalculateCount()
    {
        return data.Count;
    }

    protected override void Movement()
    {
        StartCoroutine(Waiting());
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
                        weapon.transform.Translate(Vector3.up * fireSpeed * Time.fixedDeltaTime);
                }
            }).AddTo(disposables);
    }

    private IEnumerator Waiting()
    {
        while (true)
        {
            isWaitingTime = false;
            yield return new WaitForSeconds(CalculateSpeed());

            isWaitingTime = true;
            SetPosition();
        }
    }
}
