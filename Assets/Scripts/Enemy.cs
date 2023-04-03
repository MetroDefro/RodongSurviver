using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Pool;

[System.Serializable]
public class EnemyData
{
    public float Speed;
    public float Damage;
    public float HP;
    public float Defence;
    public float exp;
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private EXPMarble expMarblePrefab;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyData data;
    private Player player;
    private int isHitId;
    private int isDeadId;

    private bool isDead;

    private IObjectPool<Enemy> pool;
    private CompositeDisposable disposables = new CompositeDisposable();

    private void OnEnable()
    {
        SubscribeFixedUpdate();
        SubscribeOnCollisionStay2D();
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

        return this;
    }

    public void Hit(float damage)
    {
        if (isDead)
            return;

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

        EXPMarble expMarble = Instantiate(expMarblePrefab, transform.position, transform.rotation);
        expMarble.Initialize(player, data.exp);

        yield return new WaitForSeconds(1f);

        isDead = false;
        gameObject.SetActive(false);
    }
    #endregion
}
