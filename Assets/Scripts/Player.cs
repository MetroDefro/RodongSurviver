using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;
using System;

[System.Serializable]
public class PlayerData
{
    public WeaponType WeaponType;
    public float HP;
    public float Damage;
    public float Speed;
    public float Defence;
}

public class Player : MonoBehaviour
{
    public WeaponType WeaponType => data.WeaponType;

    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private PlayerData data;
    private PlayerHUD hud;

    private ReactiveProperty<Vector2> inputVector2 = new ReactiveProperty<Vector2>();
    private ReactiveProperty<float> hp = new ReactiveProperty<float>();

    private int level = 1;
    private float exp = 0;
    private float necessaryEXP = 2;

    private int speedId;
    private int isDeadId;

    private Action<int> onLevelUp;

    private CompositeDisposable disposables = new CompositeDisposable();

    public Action OnDied { get; set; } = null;

    #region Public Method
    public Player Initialize(PlayerHUD hud, Action<int> onLevelUp)
    {
        this.hud = hud;
        this.onLevelUp = onLevelUp;

        level = 1;
        exp = 0;
        necessaryEXP = 2;

        speedId = Animator.StringToHash("Speed");
        isDeadId = Animator.StringToHash("IsDead");

        hp.Value = data.HP;

        SubscribeFixedUpdate();
        SubscribeInputVector2();
        SubscribeHP();

        return this;
    }

    public void Hit(float damge)
    {
        hp.Value -= damge;
    }

    public void AddEXP(float addedEXP)
    {
        exp += addedEXP;

        if (exp >= necessaryEXP)
        {
            exp -= necessaryEXP;
            necessaryEXP *= 1.5f;
            level++;
            onLevelUp?.Invoke(level);
        }

        hud.SetEXPbar(Mathf.InverseLerp(0, necessaryEXP, exp));
    }

    public void Dispose()
    {
        rigidbody.MovePosition(new Vector2(0, 0));
        anim.SetBool(isDeadId, false);
    }
    #endregion

    #region Private Method

    private void SubscribeFixedUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 direction = inputVector2.Value * data.Speed * Time.deltaTime;
                rigidbody.MovePosition(rigidbody.position + direction);
            }).AddTo(disposables);
    }

    private void SubscribeInputVector2()
    {
        inputVector2.Subscribe(value =>
        {
            if (value.x != 0)
                spriteRenderer.flipX = value.x > 0;

            anim.SetFloat(speedId, value.magnitude);
        }).AddTo(disposables);
    }

    private void SubscribeHP()
    {
        hp.Subscribe(value =>
        {
            hud.SetHPbar(Mathf.InverseLerp(0, data.HP, value));

            if(value <= 0)
            {
                anim.SetBool(isDeadId, true);
                disposables.Clear();
                OnDied?.Invoke();
            }
        }).AddTo(disposables);
    }

    private void OnMove(InputValue value)
    {
        inputVector2.Value = value.Get<Vector2>();
    }

    #endregion
}
