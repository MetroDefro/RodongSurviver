using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Pool;

public class EnemyData
{
    public float Speed;
    public float Damage;
    public float HP;
    public float Defence;
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EnemyData data;
    private Player player;
    private int isHitId;
    private int isDeadId;

    private IObjectPool<Enemy> pool;
    private CompositeDisposable disposables = new CompositeDisposable();

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        pool.Release(this);
    }

    #region Public Method
    public Enemy Initialize(Player player, IObjectPool<Enemy> pool)
    {
        this.player = player;
        this.pool = pool;
        isHitId = Animator.StringToHash("IsHit");
        isDeadId = Animator.StringToHash("IsDead");

        data = new EnemyData
        {
            Damage = 2,
            Speed = 1,
            HP = 5,
            Defence = 1,
        };

        SubscribeFixedUpdate();
        SubscribeOnCollisionStay2D();

        return this;
    }

    public void Hit(float damage)
    {
        data.HP -= damage;

        if (data.HP <= 0)
            StartCoroutine(Dying());
        else
            StartCoroutine(Hitting());
    }
    #endregion

    #region Private Method
    private void SubscribeFixedUpdate()
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

    private void SubscribeOnCollisionStay2D()
    {
        this.OnCollisionEnter2DAsObservable()
            .Subscribe(collision =>
            {
                if (collision.gameObject == player.gameObject)
                {
                    player.Hit(data.Damage);
                }
            }).AddTo(this);
    }

    private IEnumerator Hitting()
    {
        anim.SetBool(isHitId, true);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool(isHitId, false);
    }

    private IEnumerator Dying()
    {
        anim.SetBool(isDeadId, true);
        disposables.Clear();
        yield return new WaitForSeconds(1f);
        pool.Release(this);
    }
    #endregion
}
