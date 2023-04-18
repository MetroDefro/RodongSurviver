using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Object/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public GameObject WeaponObject => weaponObject;
    public float Damage => damage;
    public float Speed => speed;
    public float Term => term;
    public float Size => size;
    public float Range => range;
    public int Count => count;

    [SerializeField] private GameObject weaponObject;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float term;
    [SerializeField] private float size;
    [SerializeField] private float range;
    [SerializeField] private int count;
}
