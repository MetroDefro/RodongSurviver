using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInitData
{
    public Weapon Weapon;
    public float HP;
    public float Damage;
    public float Speed;
    public float Defence;
}

public class Player : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private PlayerInitData initData;
    private PlayerHUD hud;

    private ReactiveProperty<Vector2> InputVector2 = new ReactiveProperty<Vector2>();
    private ReactiveProperty<float> HP = new ReactiveProperty<float>();
    private List<Weapon> weapons = new List<Weapon>();

    private int Level = 1;
    private float EXP = 0;
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

        Weapon weapon = Instantiate(initData.Weapon);
        weapon.Initialize(this);
        weapons.Add(weapon);

        SubscribeFixedUpdate();
        SubscribeInputVector2();
        SubscribeInputHP();

        return this;
    }

    public void Hit(float damge)
    {
        HP.Value -= damge;
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

    private void SubscribeInputHP()
    {
        HP.Subscribe(value =>
        {
            hud.SetHPbar(Mathf.InverseLerp(0, initData.HP, value));

            if(value <= 0)
            {
                anim.SetBool(isDeadId, true);
                disposables.Clear();
            }
        }).AddTo(this);
    }

    private void OnMove(InputValue value)
    {
        InputVector2.Value = value.Get<Vector2>();
    }

    #endregion
}
