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
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D gameArea;

    private List<Enemy> enemies = new List<Enemy>();
    private IObjectPool<Enemy> pool;

    private void Start()
    {
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
        Vector2 randomVector = new Vector2(Random.Range(player.position.x - spwanRange, player.position.x + spwanRange), Random.Range(player.position.y - spwanRange, player.position.y + spwanRange));

        while (randomVector.x > gameArea.bounds.min.x && randomVector.x < gameArea.bounds.max.x && randomVector.y > gameArea.bounds.min.y && randomVector.y < gameArea.bounds.max.y)
        {
            randomVector = new Vector2(Random.Range(player.position.x - spwanRange, player.position.x + spwanRange), Random.Range(player.position.y - spwanRange, player.position.y + spwanRange));
        }

        Enemy enemy = Instantiate(enemyPrefab, randomVector, enemyPrefab.transform.rotation, transform);
        enemy.Player = player;
        enemy.SetPool(pool);
        enemies.Add(enemy);
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
}