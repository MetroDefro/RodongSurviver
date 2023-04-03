using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInitData
{
    public WeaponType WeaponType;
    public float HP;
    public float Damage;
    public float Speed;
    public float Defence;
}

public class Player : MonoBehaviour
{
    public WeaponType WeaponType => initData.WeaponType;

    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private PlayerInitData initData;
    private PlayerHUD hud;

    private ReactiveProperty<Vector2> InputVector2 = new ReactiveProperty<Vector2>();
    private ReactiveProperty<float> HP = new ReactiveProperty<float>();

    private int Level = 1;
    private float EXP = 0;
    private float NecessaryEXP = 2;
    private float speed;

    private int speedId;
    private int isDeadId;

    private CompositeDisposable disposables = new CompositeDisposable();
    
    #region Public Method
    public Player Initialize(PlayerHUD hud)
    {
        this.hud = hud;

        speedId = Animator.StringToHash("Speed");
        isDeadId = Animator.StringToHash("IsDead");

        HP.Value = initData.HP;
        speed = initData.Speed;

        SubscribeFixedUpdate();
        SubscribeInputVector2();
        SubscribeHP();

        return this;
    }

    public void Hit(float damge)
    {
        HP.Value -= damge;
    }

    public void PlusEXP(float addedEXP)
    {
        EXP += addedEXP;

        if (EXP >= NecessaryEXP)
        {
            EXP -= NecessaryEXP;
            NecessaryEXP *= 1.2f;
            Level++;
            hud.SetLevelUp(Level);
        }

        hud.SetEXPbar(Mathf.InverseLerp(0, NecessaryEXP, EXP));
    }
    #endregion

    #region Private Method

    private void SubscribeFixedUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 direction = InputVector2.Value * speed * Time.deltaTime;
                rigidbody.MovePosition(rigidbody.position + direction);
            }).AddTo(disposables);
    }

    private void SubscribeInputVector2()
    {
        InputVector2.Subscribe(value =>
        {
            if (value.x != 0)
                spriteRenderer.flipX = value.x > 0;

            anim.SetFloat(speedId, value.magnitude);
        }).AddTo(disposables);
    }

    private void SubscribeHP()
    {
        HP.Subscribe(value =>
        {
            hud.SetHPbar(Mathf.InverseLerp(0, initData.HP, value));

            if(value <= 0)
            {
                anim.SetBool(isDeadId, true);
                disposables.Clear();
            }
        }).AddTo(disposables);
    }

    private void OnMove(InputValue value)
    {
        InputVector2.Value = value.Get<Vector2>();
    }

    #endregion
}
