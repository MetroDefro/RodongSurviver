using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShieldWeapon : Weapon
{
    protected override float CalculateSpeed()
    {
        return base.CalculateSpeed() * 0.6f;
    }

    protected override float CalculateSize()
    {
        return base.CalculateSize() + (level - 1) * initData.Size * player.Status.WeaponSize.Value * 0.1f;
    }

    protected override int CalculateCount()
    {
        return base.CalculateCount() + Mathf.FloorToInt(level * 0.4f);
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();
        SetPosition();
    }

    protected override void Movement()
    {
        SetPosition();
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
                transform.Rotate(Vector3.back * CalculateSpeed() * Time.fixedDeltaTime);
            }).AddTo(disposables);
    }
}