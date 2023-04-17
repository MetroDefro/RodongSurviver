using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Object/Enemy Data")]
public class EnemyData: ScriptableObject
{
    public float Speed { get => speed; set => value = speed; }
    public float Damage => damage;
    public float HP { get => hp; set => value = hp; }
    public float Defence => defence;
    public float EXP => exp;

    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float hp;
    [SerializeField] private float defence;
    [SerializeField] private float exp;
}