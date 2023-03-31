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
    private ReactiveProperty<Vector2> InputVector2 = new ReactiveProperty<Vector2>();
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
        SubscribeInputVector2();
    }

    #region Private Method

    private void SubscribeFixedUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 direction = InputVector2.Value * speed * Time.deltaTime;
                rigidbody.MovePosition(rigidbody.position + direction);
            }).AddTo(this);
    }

    private void OnMove(InputValue value)
    {
        InputVector2.Value = value.Get<Vector2>();
    }

    private void SubscribeInputVector2()
    {
        InputVector2.Subscribe(_ =>
        {
            if (InputVector2.Value.x != 0)
                spriteRenderer.flipX = InputVector2.Value.x < 0;

            anim.SetFloat(speedId, InputVector2.Value.magnitude);
        }).AddTo(this);
    }
    #endregion
}
