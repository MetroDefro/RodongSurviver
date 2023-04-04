using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BackGroundMover[] backGroundMovers;
    [SerializeField] private BoxCollider2D gameArea;

    [SerializeField] private Weapon[] allWeaponPrefabs = new Weapon[1];
    private List<(WeaponType, Weapon)> weapons = new List<(WeaponType, Weapon)>();
    // private List<Item> items = new List<Item>();

    private int maxLevel = 6;

    private void Start()
    {
        foreach (var bg in backGroundMovers) 
            bg.Initialize(player, gameArea);

        player.Initialize(playerHUD, (level) => OnLevelUp(level));
        playerHUD.Initialize((weaponType) => OnWeaponLevelUp(weaponType));
        enemySpawner.Initialize(player, gameArea);


        Weapon firstWeapon = Instantiate(allWeaponPrefabs[(int)player.WeaponType]).Initialize(player);
        weapons.Add((player.WeaponType, firstWeapon));
        playerHUD.SetWeaponSlot(0, firstWeapon.Level, firstWeapon.Sprite);
    }

    private void OnLevelUp(int level)
    {
        playerHUD.SetLevelUpPanel(level, new(WeaponType, Sprite, string)[] { getWeapon(), getWeapon(), getWeapon() });
    }

    private void OnWeaponLevelUp(WeaponType weaponType)
    {
        weapons.Where(o => o.Item1 == weaponType).FirstOrDefault().Item2.AddLevel();

        playerHUD.SetWeaponSlot((int)weaponType, weapons[(int)weaponType].Item2.Level, weapons[(int)weaponType].Item2.Sprite);
    }

    private (WeaponType, Sprite, string) getWeapon()
    {
        // If all are max level, instead, money & HP will come out
        if (weapons.Count >= 6)
        {
            Debug.Log("To Do::");

            int minLevel = maxLevel;
            foreach (var weapon in weapons)
            {
                if (minLevel > weapon.Item2.Level)
                    minLevel = weapon.Item2.Level;
            }

            if (minLevel == maxLevel)
                return (0, weapons[0].Item2.Sprite, weapons[0].Item2.Explanation); // must be corrected
        }


        int weaponType = Random.Range(0, allWeaponPrefabs.Length - 1);
        while (weapons.Where(o => o.Item1 == (WeaponType)weaponType).FirstOrDefault().Item2.Level == maxLevel)
        {
            weaponType = Random.Range(0, allWeaponPrefabs.Length - 1);
        }
        return ((WeaponType)weaponType, allWeaponPrefabs[weaponType].Sprite, allWeaponPrefabs[weaponType].Explanation);
    }
}
