using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShieldWeapon : Weapon
{
    protected override float CalculateDamage()
    {
        return data.Damage * level * 1;
    }

    protected override float CalculateSpeed()
    {
        return data.Speed + (level - 1) * data.Speed * 0.6f;
    }

    protected override float CalculateSize()
    {
        return data.Size + (level - 1) * data.Size * 0.2f;
    }

    protected override float CalculateRange()
    {
        return data.Range * level * 1;
    }

    protected override int CalculateCount()
    {
        return data.Count + Mathf.FloorToInt(level * 0.4f);
    }

    protected override void Movement()
    {
        SubscribeMovement();
    }    
    
    protected override void SetPosition()
    {
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
                transform.Rotate(Vector3.back * CalculateSpeed() * Time.fixedDeltaTime);
            }).AddTo(disposables);
    }
}