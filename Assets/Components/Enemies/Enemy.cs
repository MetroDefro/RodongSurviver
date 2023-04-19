using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Pool;

public abstract class Enemy : MonoBehaviour
{
    #region [ Variables ]
    [SerializeField] private EXPMarble expMarblePrefab;
    [SerializeField] private Animator anim;
    [SerializeField] private CapsuleCollider2D collider2D;
    [SerializeField] protected Rigidbody2D rigidbody;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected EnemyData data;
    protected Player player;

    private float hp;
    private int isHitId;
    private int isDeadId;

    private bool isDead;

    private Action<EXPMarble> onDead;
    private Action<EXPMarble> onEXPMarbleDestroed;
    private IObjectPool<Enemy> pool;
    protected CompositeDisposable disposables = new CompositeDisposable();
    #endregion

    #region [ MonoBehaviour Messages ]
    private void OnEnable()
    {
        collider2D.enabled = true;
        Play();
    }
    #endregion

    #region [ Public Method ]
    public Enemy Initialize(Player player, EnemyData data, IObjectPool<Enemy> pool, Action<EXPMarble> onDead, Action<EXPMarble> onEXPMarbleDestroed)
    {
        this.data = data;
        this.player = player;
        this.pool = pool;
        this.onDead = onDead;
        this.onEXPMarbleDestroed = onEXPMarbleDestroed;

        data.Speed = UnityEngine.Random.Range(1, 3);
        hp = data.HP;
        anim.runtimeAnimatorController = data.AnimatorController;

        isHitId = Animator.StringToHash("IsHit");
        isDeadId = Animator.StringToHash("IsDead");

        return this;
    }

    public void Hit(float damage)
    {
        if (isDead)
            return;

        hp -= damage;

        if (hp <= 0)
            StartCoroutine(Dying());
        else
            StartCoroutine(Hitting());
    }

    public void Dispose()
    {
        disposables.Clear();
        pool.Release(this);
    }

    public void Pause()
    {
        anim.speed = 0;
        disposables.Clear();
    }

    public void Play()
    {
        anim.speed = 1;
        Movement();
        SubscribeOnCollisionStay2D();
    }
    #endregion

    #region [ Private Method ]
    protected abstract void Movement();

    private void SubscribeOnCollisionStay2D()
    {
        this.OnCollisionStay2DAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1))
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.Hit(data.Damage);
                }
            }).AddTo(disposables);
    }

    private IEnumerator Hitting()
    {
        anim.SetBool(isHitId, true);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool(isHitId, false);
    }

    private IEnumerator Dying()
    {
        isDead = true;

        anim.SetBool(isDeadId, true);
        disposables.Clear();
        collider2D.enabled = false;

        EXPMarble expMarble = Instantiate(expMarblePrefab, transform.position, transform.rotation);
        expMarble.Initialize(player, data.EXP, onEXPMarbleDestroed);
        onDead.Invoke(expMarble);

        yield return new WaitForSeconds(1f);

        pool.Release(this);
        isDead = false;
    }
    #endregion
}
