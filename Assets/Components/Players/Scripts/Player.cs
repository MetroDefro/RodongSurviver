using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    public class PlayerActions
    {
        public Action<int> OnLevelUp { get; set; }
        public Action<float, float> OnGetEXP { get; set; }
        public Action<int> OnGetMoney { get; set; }
        public Action OnDied { get; set; }
    }

    #region [ Properties ]
    public ItemType WeaponType => initData.WeaponType;
    public Vector2 InputVector2 => inputVector2.Value;
    public PlayerStatus Status => status;
    public BoxCollider2D GameArea => gameArea;

    // public Action OnDied { get; set; } = null;
    #endregion

    #region [ Variables ]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Transform hpBar;
    [SerializeField] private BoxCollider2D gameArea;
    private PlayerActions actions;
    private PlayerData initData;
    private PlayerStatus status;

    private ReactiveProperty<Vector2> inputVector2 = new ReactiveProperty<Vector2>();

    private int speedId;
    private int isDeadId;

    private float maxHPbarScaleX;

    private CompositeDisposable disposables = new CompositeDisposable();
    #endregion

    #region [ Public methods ]
    public Player Initialize(PlayerData data, PlayerActions actions)
    {
        this.initData = data;
        this.actions = actions;

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

        SetHPbar(Mathf.InverseLerp(0, status.MaxHP, status.HP));

        if (status.HP <= 0)
        {
            anim.SetBool(isDeadId, true);
            disposables.Clear();
            actions.OnDied?.Invoke();
        }
    }

    public void AddEXP(float addedEXP)
    {
        status.AddEXP(addedEXP);

        if (status.EXP >= status.NecessaryEXP)
        {
            status.AddLevel();
            actions.OnLevelUp?.Invoke(status.Level);
        }

        actions.OnGetEXP?.Invoke(status.NecessaryEXP, status.EXP);
    }    
    
    public void AddMoney(int AddedMoney)
    {
        status.AddMoney(AddedMoney);

        actions.OnGetMoney?.Invoke(status.Money);
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
                Vector2 direction = inputVector2.Value * status.Speed * Time.deltaTime;
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
