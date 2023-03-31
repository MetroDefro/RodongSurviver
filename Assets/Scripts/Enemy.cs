using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    public Transform Player;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float Speed;
    private int isHitId;
    private int isDeadId;

    private IObjectPool<Enemy> pool;

    private void Start()
    {
        isHitId = Animator.StringToHash("IsHitId");
        isDeadId = Animator.StringToHash("IsDead");

        SubscribeFixedUpdate();
    }

    private void OnDisable()
    {
        pool.Release(this);
    }

    private void SubscribeFixedUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 targetDirection = (Player.position - transform.position).normalized;

                rigidbody.MovePosition(rigidbody.position + targetDirection * Speed * Time.deltaTime);

                if (targetDirection.x != 0)
                    spriteRenderer.flipX = targetDirection.x < 0;
            }).AddTo(this);
    }

    public void SetPool(IObjectPool<Enemy> pool)
    {
        this.pool = pool;
    }
}
