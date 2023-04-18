using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Object/Player Data", order = 0)]
public class PlayerData : ScriptableObject
{
    public ItemType WeaponType => weaponType;
    public float HP => hp;
    public float Speed => speed;
    public float Magnetism => magnetism;

    [SerializeField] private ItemType weaponType;
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    [SerializeField] private float magnetism;
}