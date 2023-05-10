using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UniRx;

public class EnemySpawner : MonoBehaviour
{
    public int MaxEnemyCount { get => maxEnemyCount; set => maxEnemyCount =  value; }
    public int CurrentEnemyIndex { get => currentEnemyIndex; set => currentEnemyIndex =  value; }
    public int InitEnemyCount => initEnemyCount;

    [SerializeField] private int initEnemyCount = 30;
    [SerializeField] private int maxPoolEnemyCount = 500;
    [SerializeField] private int maxEnemyCount = 30;
    [SerializeField] private float spwanRange = 60;

    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private EXPMarble expMarblePrefab;
    [SerializeField] private Money moneyPrefab;

    [SerializeField] private EnemyData[] enemyDatas = new EnemyData[10];

    private int currentEnemyIndex = 0;

    private BoxCollider2D gameArea;
    private Player player;

    private List<Enemy> enemies = new List<Enemy>();
    private List<EXPMarble> expMarbles = new List<EXPMarble>();
    private List<Money> moneys = new List<Money>();

    private IObjectPool<Enemy> pool;

    private CompositeDisposable Disposables { get; set; } = new CompositeDisposable();

    public void Initialize(Player player, BoxCollider2D gameArea)
    {
        this.player = player;
        this.gameArea = gameArea;
        pool = new ObjectPool<Enemy>(SpwanEnemy, OnGetPool, OnReleasePool, OnDestroyPool, true, initEnemyCount, maxPoolEnemyCount);
        SubscribeUpdate();
    }

    public void Reset()
    {
        Disposables.Clear();

        foreach (var enemy in enemies.ToArray())
            enemy.Reset();

        foreach (var expMarble in expMarbles.ToArray())
        {
            if (expMarble != null)
                expMarble.Dispose();
        }

        foreach (var money in moneys.ToArray())
        {
            if (money != null)
                money.Dispose();
        }

        pool.Clear();
        enemies.Clear();
        expMarbles.Clear();
        moneys.Clear();
    }

    public void Dispose()
    {
        Disposables.Dispose();

        foreach(var enemy in enemies.ToArray())
        {
            if(enemy != null)
                enemy.Dispose();
        }

        foreach (var expMarble in expMarbles.ToArray())
        {
            if (expMarble != null)
                expMarble.Dispose();
        }

        foreach (var money in moneys.ToArray())
        {
            if (money != null)
                money.Dispose();
        }

        pool.Clear();
        enemies.Clear();
        expMarbles.Clear();
        moneys.Clear();
    }

    private void SubscribeUpdate()
    {
        Observable.EveryFixedUpdate()
            .Subscribe(_ => 
            {
                if (enemies.Count < maxEnemyCount)
                {
                    pool.Get();
                }
            }).AddTo(Disposables);
    }

    private void OnEnemyDead(float exp, Transform enemyTransform)
    {
        if(Random.Range(0, 50) == 0)
            AddMoney(enemyTransform);

        AddEXPMarble(exp, enemyTransform);
    }

    private void AddEXPMarble(float exp, Transform enemyTransform)   
    {
        EXPMarble expMarble = Instantiate(expMarblePrefab, enemyTransform.position, enemyTransform.rotation);
        expMarble.Initialize(player, exp, (expMarble) => RemoveEXPMarble(expMarble));
        expMarbles.Add(expMarble);
    }

    private void RemoveEXPMarble(EXPMarble expMarble)
    {
        expMarbles.Remove(expMarble);
    }
    
    private void AddMoney(Transform enemyTransform)
    {
        Money money = Instantiate(moneyPrefab, enemyTransform.position, enemyTransform.rotation);
        money.Initialize(player, 1, (money) => RemoveMoney(money));
        moneys.Add(money);
    }

    private void RemoveMoney(Money money)
    {
        moneys.Remove(money);
    }

    private Enemy SpwanEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab, GetRandomVector(), enemyPrefab.transform.rotation, transform);
        return enemy;
    }

    private void OnGetPool(Enemy enemy)
    {
        enemies.Add(enemy.Initialize(player, enemyDatas[currentEnemyIndex], pool, (exp, enemyTransform) => OnEnemyDead(exp, enemyTransform)));
        enemy.transform.position = GetRandomVector();
        enemy.gameObject.SetActive(true);
    }

    private void OnReleasePool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        enemies.Remove(enemy);
    }

    private void OnDestroyPool(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private Vector2 GetRandomVector()
    {
        Vector2 randomVector = GetRandomRange(spwanRange);

        while (randomVector.x > gameArea.bounds.min.x && randomVector.x < gameArea.bounds.max.x && randomVector.y > gameArea.bounds.min.y && randomVector.y < gameArea.bounds.max.y)
        {
            randomVector = GetRandomRange(spwanRange);
        }

        return randomVector;
    }

    private Vector2 GetRandomRange(float spwanRange)
    {
        return new Vector2(Random.Range(player.transform.position.x - spwanRange, player.transform.position.x + spwanRange)
                , Random.Range(player.transform.position.y - spwanRange, player.transform.position.y + spwanRange));
    }
}