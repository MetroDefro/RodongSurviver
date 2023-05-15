using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerStatus
{
    public IReadOnlyReactiveProperty<int> Level => level;
    public IReadOnlyReactiveProperty<float> EXP => exp;
    public IReadOnlyReactiveProperty<float> NecessaryEXP => necessaryEXP;
    public IReadOnlyReactiveProperty<int> Money => money;
    public IReadOnlyReactiveProperty<float> HP => hp;
    public IReadOnlyReactiveProperty<float> MaxHP => maxHP;
    public IReadOnlyReactiveProperty<float> Speed => speed;
    public IReadOnlyReactiveProperty<float> Magnetism => magnetism;
    public IReadOnlyReactiveProperty<float> Damage => damage;
    public IReadOnlyReactiveProperty<float> WeaponSpeed => weaponSpeed;
    public IReadOnlyReactiveProperty<float> WeaponTerm => weaponTerm;
    public IReadOnlyReactiveProperty<float> WeaponSize => weaponSize;
    public IReadOnlyReactiveProperty<int> WeaponCount => weaponCount;

    private ReactiveProperty<int> level = new ReactiveProperty<int>();
    private ReactiveProperty<float> exp = new ReactiveProperty<float>();
    private ReactiveProperty<float> necessaryEXP = new ReactiveProperty<float>();
    private ReactiveProperty<int> money = new ReactiveProperty<int>();
    private ReactiveProperty<float> hp = new ReactiveProperty<float>();
    private ReactiveProperty<float> maxHP = new ReactiveProperty<float>();
    private ReactiveProperty<float> speed = new ReactiveProperty<float>();
    private ReactiveProperty<float> magnetism = new ReactiveProperty<float>();
    private ReactiveProperty<float> damage = new ReactiveProperty<float>();
    private ReactiveProperty<float> weaponSpeed = new ReactiveProperty<float>();
    private ReactiveProperty<float> weaponTerm = new ReactiveProperty<float>();
    private ReactiveProperty<float> weaponSize = new ReactiveProperty<float>();
    private ReactiveProperty<int> weaponCount= new ReactiveProperty<int>();

    public PlayerStatus(float initHP, float speed, float magnetism)
    {
        this.level.Value = 1;
        this.exp.Value = 0;
        this.necessaryEXP.Value = 2;
        this.money.Value = 0;
        this.hp.Value = initHP;
        this.maxHP.Value = initHP;
        this.speed.Value = speed;
        this.magnetism.Value = magnetism;
        this.damage.Value = 1;
        this.weaponSpeed.Value = 1;
        this.weaponTerm.Value = 1;
        this.weaponSize.Value = 1;
        this.weaponCount.Value = 0;
    }

    public PlayerStatus SetEnforce(EnforceData enforceData)
    {
        AddMaxpHP(1 + enforceData.BuffGrades[(int)ItemType.Rice - 100] * 1.2f);
        AddSpeed(1 + enforceData.BuffGrades[(int)ItemType.Shoes - 100] * 0.1f);
        AddMagnetism(1 + enforceData.BuffGrades[(int)ItemType.Magnet - 100] * 0.2f);
        AddDamage(1 + enforceData.BuffGrades[(int)ItemType.Dumbbell - 100] * 0.2f);
        AddWeaponSpeed(1 + enforceData.BuffGrades[(int)ItemType.Tornado - 100] * 0.1f);
        AddWeaponTerm(1 + enforceData.BuffGrades[(int)ItemType.Thunder - 100] * 0.1f);
        AddWeaponSize(1 + enforceData.BuffGrades[(int)ItemType.Baloon - 100] * 0.1f);
        AddWeaponCount(enforceData.BuffGrades[(int)ItemType.Dice - 100]);

        return this;
    }

    public int AddLevel()
    {
        ++level.Value;
        
        // need fix
        exp.Value -= necessaryEXP.Value;
        if (level.Value <= 10)
            necessaryEXP.Value *= 1.2f;
        else if (level.Value <= 20)
            necessaryEXP.Value *= 1.15f;
        else if (level.Value <= 30)
            necessaryEXP.Value *= 1.10f;
        else
            necessaryEXP.Value *= 1.05f;

        return level.Value;
    }

    public float AddEXP(float value)
    {
        exp.Value += value;

        return exp.Value;
    }    
    
    public int AddMoney(int value)
    {
        money.Value += value;

        return money.Value;
    }

    public float PlusHP(float value)
    {
        hp.Value += value;
        if(hp.Value > maxHP.Value)
            hp.Value = maxHP.Value;

        return hp.Value;
    }

    public float MinusHP(float value)
    {
        hp.Value -= value;

        return hp.Value;
    }

    public float AddMaxpHP(float value)
    {
        float newMaxHP = maxHP.Value * value;
        float hpDifference = newMaxHP - maxHP.Value;

        maxHP.Value = newMaxHP;
        PlusHP(hpDifference);

        return maxHP.Value;
    }

    public float AddSpeed(float value)
    {
        speed.Value *= value;
        return speed.Value;
    }

    public float AddMagnetism(float value)
    {
        magnetism.Value *= value;
        return magnetism.Value;
    }

    public float AddDamage(float value)
    {
        damage.Value *= value;
        return damage.Value;
    }

    public float AddWeaponSpeed(float value)
    {
        weaponSpeed.Value *= value;
        return weaponSpeed.Value;
    }

    public float AddWeaponTerm(float value)
    {
        weaponTerm.Value *= value;
        return weaponTerm.Value;
    }

    public float AddWeaponSize(float value)
    {
        weaponSize.Value *= value;
        return weaponSize.Value;
    }

    public float AddWeaponCount(int value)
    {
        weaponCount.Value += value;
        return weaponCount.Value;
    }
}