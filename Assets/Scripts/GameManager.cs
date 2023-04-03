using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BackGroundMover[] backGroundMovers;
    [SerializeField] private BoxCollider2D gameArea;

    private void Start()
    {
        foreach (var bg in backGroundMovers) 
            bg.Initialize(player, gameArea);

        player.Initialize(playerHUD);
        playerHUD.Initialize();
        enemySpawner.Initialize(player, gameArea);
    }
}
