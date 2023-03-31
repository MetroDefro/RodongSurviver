using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private float speed = 5;
    private Vector2 InputDirection;
    private int speedId;
    private int isDeadId;

    private void Awake()
    {

    }

    private void Start()
    {
        speedId = Animator.StringToHash("Speed");
        isDeadId = Animator.StringToHash("IsDead");
        SubscribeFixedUpdate();
    }

    private void SubscribeFixedUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 direction = InputDirection * speed * Time.deltaTime;
                rigidbody.MovePosition(rigidbody.position + direction);
                
                if(InputDirection.x != 0)
                {
                    spriteRenderer.flipX = InputDirection.x < 0;
                }
                anim.SetFloat(speedId, InputDirection.magnitude);

            }).AddTo(this);
    }

    private void OnMove(InputValue value)
    {
        InputDirection = value.Get<Vector2>();
    }
}
