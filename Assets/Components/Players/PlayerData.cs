using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Object/Player Data")]
public class PlayerData : ScriptableObject
{
    public WeaponType WeaponType => weaponType;
    public float HP => hp;
    public float Damage => damage;
    public float Speed => speed;
    public float Defence => defence;
    public float Magnetism => magnetism;

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float hp;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float defence;
    [SerializeField] private float magnetism;
}