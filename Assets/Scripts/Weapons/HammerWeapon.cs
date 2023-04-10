using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HammerWeapon : Weapon
{
    [SerializeField] private float rotateSpeed;
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
                        weapon.transform.Rotate(Vector3.forward * rotateSpeed * Time.fixedDeltaTime);
                    accumulateRotate += rotateSpeed * Time.fixedDeltaTime;

                    if (accumulateRotate >= 90)
                    {
                        accumulateRotate = 0;
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
