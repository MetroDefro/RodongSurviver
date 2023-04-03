using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[System.Serializable]
public class WeaponData
{
    public float Damage;
    public float Speed;
    public float SpeedMultiplier;
    public float DamageMultiplier;
}

public abstract class Weapon : MonoBehaviour
{
    protected Player player;
    [SerializeField] protected WeaponData data;
    protected int level = 1;

    public Weapon Initialize(Player player)
    {
        this.player = player;

        Movement();
        SubscribeOnCollisionStay2D();
        return this;
    }

    public int PlusLevel()
    {
        level++;
        return level;
    }

    private void SubscribeOnCollisionStay2D()
    {
        this.OnCollisionStay2DAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1))
            .Subscribe(collision => 
            {
                if (collision.gameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.Hit(CalculateDamage());
                }
            }).AddTo(this);
    }

    protected abstract void Movement();

    // (WeaponDamage * WeaponLevel * WeaponRank * weight) * (PlayerDamage * weight)
    protected abstract float CalculateDamage();
}
