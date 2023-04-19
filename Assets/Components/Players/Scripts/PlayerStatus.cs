using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus
{
    public int Level => level;
    public float EXP => exp;
    public float NecessaryEXP => necessaryEXP;
    public float HP => hp;
    public float MaxHP => maxHP;
    public float Speed => speed;
    public float Magnetism => magnetism;
    public float Damage => damage;
    public float WeaponSpeed => weaponSpeed;
    public float WeaponTerm => weaponTerm;
    public float WeaponSize => weaponSize;
    public int WeaponCount => weaponCount;

    private int level;
    private float exp;
    private float necessaryEXP;
    private float hp;
    private float maxHP;
    private float speed;
    private float magnetism;
    private float damage;
    private float weaponSpeed;
    private float weaponTerm;
    private float weaponSize;
    private int weaponCount;

    public PlayerStatus(float initHP, float speed, float magnetism)
    {
        this.level = 1;
        this.exp = 0;
        this.necessaryEXP = 2;
        this.hp = initHP;
        this.maxHP = initHP;
        this.speed = speed;
        this.magnetism = magnetism;
        this.damage = 1;
        this.weaponSpeed = 1;
        this.weaponTerm = 1;
        this.weaponSize = 1;
        this.weaponCount = 0;
    }

    public int AddLevel()
    {
        ++level;
        
        // need fix
        exp -= necessaryEXP;
        if (level <= 10)
            necessaryEXP *= 1.2f;
        else if (level <= 20)
            necessaryEXP *= 1.1f;
        else if (level <= 30)
            necessaryEXP *= 1.05f;
        else
            necessaryEXP *= 1.025f;

        return level;
    }

    public float AddEXP(float value)
    {
        exp += value;

        return exp;
    }

    public float PlusHP(float value)
    {
        hp += value;
        if(hp > maxHP)
            hp = maxHP;

        return hp;
    }

    public float MinusHP(float value)
    {
        hp -= value;

        return hp;
    }

    public float AddMaxpHP(float value)
    {
        float newMaxHP = maxHP * value;
        float hpDifference = newMaxHP - maxHP;

        maxHP = newMaxHP;
        PlusHP(hpDifference);

        return maxHP;
    }

    public float AddSpeed(float value)
    {
        speed *= value;
        return speed;
    }

    public float AddMagnetism(float value)
    {
        magnetism *= value;
        return magnetism;
    }

    public float AddDamage(float value)
    {
        damage *= value;
        return damage;
    }

    public float AddWeaponSpeed(float value)
    {
        weaponSpeed *= value;
        return weaponSpeed;
    }

    public float AddWeaponTerm(float value)
    {
        weaponTerm *= value;
        return weaponTerm;
    }

    public float AddWeaponSize(float value)
    {
        weaponSize *= value;
        return weaponSize;
    }

    public float AddWeaponCount()
    {
        ++ weaponCount;
        return weaponCount;
    }
}