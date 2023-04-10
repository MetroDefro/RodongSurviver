using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class WandWeapon : Weapon
{
    [SerializeField] private int fireSpeed = 10;
    private bool isWaitingTime;

    protected override float CalculateDamage()
    {
        return data.Damage * Level * 1;
    }

    protected override float CalculateSpeed()
    {
        return data.Speed - (level - 1) * data.Speed * 0.2f;
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
        return data.Count * level;
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
            float angle = Random.Range(0, 360);
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
                        weapon.transform.Translate(Vector3.up * fireSpeed * Time.fixedDeltaTime);
                }
            }).AddTo(disposables);
    }

    private IEnumerator Waiting()
    {
        while (true)
        {
            isWaitingTime = true;
            foreach (var weapon in weaponObjects)
                weapon.SetActive(false);
            SetPosition();
            yield return new WaitForSeconds(CalculateSpeed());

            isWaitingTime = false;
            foreach (var weapon in weaponObjects)
                weapon.SetActive(true);
            yield return new WaitForSeconds(1);
        }
    }

}
