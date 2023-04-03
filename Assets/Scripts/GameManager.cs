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

    [SerializeField] private Weapon[] weapons = new Weapon[1];
    // [SerializeField] private Item[] items = new Item[1];

    public List<int> Levels = new List<int>();

    private void Start()
    {
        foreach (var bg in backGroundMovers) 
            bg.Initialize(player, gameArea);

        foreach(var w in weapons)
            Levels.Add(w.Level);

        player.Initialize(playerHUD);
        Instantiate(weapons[(int)player.WeaponType]).Initialize(player);
        
        playerHUD.Initialize(() => OnLevelUp(), (weaponType) => OnWeaponLevelUp(weaponType));
        enemySpawner.Initialize(player, gameArea);
    }

    private void OnLevelUp()
    {
        playerHUD.SetLevelUpPanel(new (WeaponType, Sprite)[] { getWeapon(), getWeapon(), getWeapon() });
    }

    private void OnWeaponLevelUp(WeaponType weaponType)
    {
        weapons[(int)weaponType].PlusLevel();
        Levels[(int)weaponType]++;
    }

    private (WeaponType, Sprite) getWeapon()
    {
        // If all are max level, instead, money & HP will come out
        int minLevel = 6;
        foreach(var level in Levels)
        {
            if(minLevel > level)
                minLevel = level;
        }

        if (minLevel == 6)
            return (0, weapons[0].SpriteRenderer.sprite);

        int weaponType = Random.Range(0, weapons.Length - 1);
        while (Levels[weaponType] == 6)
        {
            weaponType = Random.Range(0, weapons.Length - 1);
        }
        return ((WeaponType)weaponType, weapons[weaponType].SpriteRenderer.sprite);
    }
}
