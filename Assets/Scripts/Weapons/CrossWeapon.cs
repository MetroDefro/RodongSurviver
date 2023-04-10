using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CrossWeapon : Weapon
{
    [SerializeField] private float fallHeight = 10;
    [SerializeField] private int fallSpeed = 10;

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
                        weapon.transform.position += (Vector3.down * fallSpeed * Time.fixedDeltaTime);
                    accumulateHeight += fallSpeed * Time.fixedDeltaTime;

                    if (accumulateHeight >= fallHeight)
                    {
                        accumulateHeight = 0;
                        StartCoroutine(Waiting());
                    }
                }
            }).AddTo(disposables);
    }

    private Vector2 GetRandomRange(float spwanRange)
    {
        return new Vector2(Random.Range(player.transform.position.x - spwanRange, player.transform.position.x + spwanRange)
                , Random.Range(player.transform.position.y - spwanRange, player.transform.position.y + spwanRange));
    }

    private IEnumerator Waiting()
    {
        isWaitingTime = true;
        foreach (var weapon in weaponObjects)
            weapon.SetActive(false);
        SetPosition();
        yield return new WaitForSeconds(CalculateSpeed());

        foreach (var weapon in weaponObjects)
            weapon.SetActive(true);
        isWaitingTime = false;
    }
}
