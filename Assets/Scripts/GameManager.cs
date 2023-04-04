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

    [SerializeField] private Weapon[] allWeaponPrefabs = new Weapon[2];
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
        playerHUD.SetLevelUpPanel(level, new Weapon[] { GetWeapon(), GetWeapon(), GetWeapon() });
    }

    private void OnWeaponLevelUp(WeaponType weaponType)
    {
        if (weapons.Where(o => o.Item1 == weaponType).FirstOrDefault().Item2 == null)
        {
            Weapon weapon = Instantiate(allWeaponPrefabs[(int)weaponType]).Initialize(player);
            weapons.Add((weaponType, weapon));
        }

        weapons.Where(o => o.Item1 == weaponType).FirstOrDefault().Item2.AddLevel();

        playerHUD.SetWeaponSlot((int)weaponType, weapons[(int)weaponType].Item2.Level, weapons[(int)weaponType].Item2.Sprite);
    }

    private Weapon GetWeapon()
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
                return weapons[0].Item2; // must be corrected
        }

        int weaponType = Random.Range(0, allWeaponPrefabs.Length);
        // If the number of weapon types is less than 6, an error continues
        //while (weapons.Where(o => o.Item1 == (WeaponType)weaponType).FirstOrDefault().Item2.Level == maxLevel)
        //{
        //    weaponType = Random.Range(0, allWeaponPrefabs.Length);
        //}
        return allWeaponPrefabs[weaponType];
    }
}
