using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ZombieEnemy : Enemy
{
    protected override void Movement()
    {
        SubscribeMovement();
    }

    private void SubscribeMovement()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 targetDirection = (player.transform.position - transform.position).normalized;

                rigidbody.MovePosition(rigidbody.position + targetDirection * data.Speed * Time.deltaTime);

                if (targetDirection.x != 0)
                    spriteRenderer.flipX = targetDirection.x < 0;
            }).AddTo(disposables);
    }
}