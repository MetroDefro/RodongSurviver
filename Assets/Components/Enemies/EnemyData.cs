using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Object/Enemy Data")]
public class EnemyData: ScriptableObject
{
    public float Speed { get => speed; set => speed = value; }
    public float Damage => damage;
    public float HP => hp;
    public float EXP => exp;
    public AnimatorOverrideController AnimatorController => animatorController;

    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float hp;
    [SerializeField] private float exp; 
    [SerializeField] private AnimatorOverrideController animatorController;
}