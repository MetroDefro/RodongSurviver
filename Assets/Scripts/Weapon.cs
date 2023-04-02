using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float Damage;
    [SerializeField] private float Speed;
    [SerializeField] private float SpeedMultiplier;
    [SerializeField] private float DamageMultiplier;

    private void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Enemy enemy))
        {

        }
    }
}
