using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    #region [ Properties ]
    public WeaponType WeaponType => initData.WeaponType;
    public Vector2 InputVector2 => inputVector2.Value;
    public PlayerStatus Status => status;

    public Action OnDied { get; set; } = null;
    #endregion

    #region [ Variables ]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Transform hpBar;
    private PlayerData initData;
    private PlayerStatus status;

    private ReactiveProperty<Vector2> inputVector2 = new ReactiveProperty<Vector2>();

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
        this.initData = data;
        this.onGetEXP = onGetEXP;
        this.onLevelUp = onLevelUp;

        status = new PlayerStatus(data.HP, data.Speed, data.Magnetism);

        speedId = Animator.StringToHash("Speed");
        isDeadId = Animator.StringToHash("IsDead");

        Play();

        maxHPbarScaleX = hpBar.localScale.x;

        return this;
    }

    public void Hit(float damge)
    {
        status.MinusHP(damge);

        SetHPbar(Mathf.InverseLerp(0, initData.HP, status.HP));

        if (status.HP <= 0)
        {
            anim.SetBool(isDeadId, true);
            disposables.Clear();
            OnDied?.Invoke();
        }
    }

    public void AddEXP(float addedEXP)
    {
        status.AddEXP(addedEXP);

        if (status.EXP >= status.NecessaryEXP)
        {
            status.AddLevel();
            onLevelUp?.Invoke(status.Level);
        }

        onGetEXP?.Invoke(status.NecessaryEXP, status.EXP);
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
    }

    #endregion

    #region [ Private methods ]
    private void SubscribeFixedUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                Vector2 direction = inputVector2.Value * initData.Speed * Time.deltaTime;
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
