using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    #region [ Properties ]
    public WeaponType WeaponType => data.WeaponType;
    public Vector2 InputVector2 => inputVector2.Value;
    public float Magnetism => data.Magnetism;

    public Action OnDied { get; set; } = null;
    #endregion

    #region [ Variables ]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Transform hpBar;
    private PlayerData data;

    private ReactiveProperty<Vector2> inputVector2 = new ReactiveProperty<Vector2>();
    private ReactiveProperty<float> hp = new ReactiveProperty<float>();
    private int level = 1;
    private float exp = 0;
    private float necessaryEXP = 2;

    private int speedId;
    private int isDeadId;

    private float maxHPbarScaleX;

    private Action<int> onLevelUp;
    private Action<float, float> onGetEXP;

    private CompositeDisposable disposables = new CompositeDisposable();
    #endregion

    #region [ Public methods ]
    public Player Initialize(PlayerData data, Action<float, float> onGetEXP, Action<int> onLevelUp)
    {
        this.data = data;
        this.onGetEXP = onGetEXP;
        this.onLevelUp = onLevelUp;

        level = 1;
        exp = 0;
        necessaryEXP = 2;

        speedId = Animator.StringToHash("Speed");
        isDeadId = Animator.StringToHash("IsDead");

        hp.Value = data.HP;

        Play();

        maxHPbarScaleX = hpBar.localScale.x;

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
            // need fix
            exp -= necessaryEXP;
            if (level <= 10)
                necessaryEXP *= 1.2f;
            else if(level <= 20)
                necessaryEXP *= 1.1f;
            else if (level <= 30)
                necessaryEXP *= 1.05f;
            else
                necessaryEXP *= 1.025f;

            level++;
            onLevelUp?.Invoke(level);
        }

        onGetEXP?.Invoke(necessaryEXP, exp);
    }

    public void Dispose()
    {
        rigidbody.MovePosition(new Vector2(0, 0));
        anim.SetBool(isDeadId, false);
    }

    public void Pause()
    {
        anim.speed = 0;
        disposables.Clear();
    }

    public void Play()
    {
        anim.speed = 1;
        SubscribeFixedUpdate();
        SubscribeInputVector2();
        SubscribeHP();
    }

    #endregion

    #region [ Private methods ]
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
            SetHPbar(Mathf.InverseLerp(0, data.HP, value));

            if(value <= 0)
            {
                anim.SetBool(isDeadId, true);
                disposables.Clear();
                OnDied?.Invoke();
            }
        }).AddTo(disposables);
    }

    private void SetHPbar(float normalizeHP)
    {
        if (maxHPbarScaleX == 0)
            return;

        hpBar.localScale = new Vector2(maxHPbarScaleX * normalizeHP, hpBar.localScale.y);
    }

    private void OnMove(InputValue value)
    {
        inputVector2.Value = value.Get<Vector2>();
    }

    #endregion
}
