using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UniRx;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private int maxEnemyCount = 50;
    [SerializeField] private float spwanRange = 40;
    [SerializeField] private Enemy enemyPrefab;
    private BoxCollider2D gameArea;
    private Player player;

    private List<Enemy> enemies = new List<Enemy>();
    private IObjectPool<Enemy> pool;

    public void Initialize(Player player, BoxCollider2D gameArea)
    {
        this.player = player;
        this.gameArea = gameArea;
        pool = new ObjectPool<Enemy>(SpwanEnemy, OnGetPool, OnReleasePool, OnDestroyPool, true, enemyCount, maxEnemyCount);
        SubscribeUpdate();
    }

    private void SubscribeUpdate()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => 
            {
                if (enemies.Count <= maxEnemyCount)
                    pool.Get();
            }).AddTo(this);
    }

    private Enemy SpwanEnemy()
    {
        Vector2 randomVector = GetRandomRange(spwanRange);

        while (randomVector.x > gameArea.bounds.min.x && randomVector.x < gameArea.bounds.max.x && randomVector.y > gameArea.bounds.min.y && randomVector.y < gameArea.bounds.max.y)
        {
            randomVector = GetRandomRange(spwanRange);
        }

        Enemy enemy = Instantiate(enemyPrefab, randomVector, enemyPrefab.transform.rotation, transform);
        enemies.Add(enemy.Initialize(player, pool));
        return enemy;
    }

    private void OnGetPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    private void OnReleasePool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyPool(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private Vector2 GetRandomRange(float spwanRange)
    {
        return new Vector2(Random.Range(player.transform.position.x - spwanRange, player.transform.position.x + spwanRange)
                , Random.Range(player.transform.position.y - spwanRange, player.transform.position.y + spwanRange));
    }
}