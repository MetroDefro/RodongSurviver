using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

public class GameScenePresenter : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BackGroundMover[] backGroundMovers;
    [SerializeField] private BoxCollider2D gameArea;

    [SerializeField] private Weapon[] allWeaponPrefabs = new Weapon[6];
    private List<(WeaponType type, Weapon weapon)> weapons = new List<(WeaponType, Weapon)>();
    // private List<Item> items = new List<Item>();

    private CompositeDisposable disposables = new CompositeDisposable();

    private float spanSeconds = 0;
    private int maxLevel = 6;

    private void Start()
    {
        Initialize();
    }

    public void OnGameOver()
    {
        PauseGame();
    }

    public void OnReset()
    {
        disposables.Clear();
        player.Dispose();
        playerHUD.Dispose();
        enemySpawner.Dispose();
        foreach (var weapon in weapons)
            DestroyImmediate(weapon.weapon.gameObject);

        weapons.Clear();
        Initialize();
    }

    public void SetupDiedAction(Action onDied) => 
        player.OnDied = () =>
        {
            onDied.Invoke();
            OnGameOver();
        };

    public void Initialize()
    {
        foreach (var bg in backGroundMovers)
            bg.Initialize(player, gameArea);

        player.Initialize(playerHUD, (level) => OnLevelUp(level));
        playerHUD.Initialize((weaponType) =>
        {
            OnWeaponLevelUp(weaponType);
            PlayGame();
        }, () => PauseGame(), () => PlayGame());
        enemySpawner.Initialize(player, gameArea);

        Weapon firstWeapon = Instantiate(allWeaponPrefabs[(int)player.WeaponType]).Initialize(player);
        weapons.Add((player.WeaponType, firstWeapon));
        playerHUD.SetWeaponSlot(0, firstWeapon.Level, firstWeapon.IconSprite);

        spanSeconds = 0;
        SubscribeEveryUpdate();
    }

    private void OnLevelUp(int level)
    {
        PauseGame();
        playerHUD.SetLevelUpPanel(level, new Weapon[] { GetWeapon(), GetWeapon(), GetWeapon() });
    }

    private void PauseGame()
    {
        player.Pause();
        disposables.Clear();
        enemySpawner.Pause();
        weapons.ForEach((weapon) => weapon.Item2.Pause());
    }

    private void PlayGame()
    {
        SubscribeEveryUpdate();
        player.Play();
        enemySpawner.Play();
        weapons.ForEach((weapon) => weapon.Item2.Play());
    }

    private void SubscribeEveryUpdate()
    {
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                spanSeconds += Time.deltaTime;
                playerHUD.SetTimer(spanSeconds);

                if (spanSeconds % 30 < 0.1f)
                    enemySpawner.MaxEnemyCount = enemySpawner.InitEnemyCount + enemySpawner.InitEnemyCount * Mathf.FloorToInt(spanSeconds / 30);
            }).AddTo(disposables);
    }

    private void OnWeaponLevelUp(WeaponType weaponType)
    {
        Weapon weapon = weapons.Where(o => o.type == weaponType).FirstOrDefault().weapon;
        if (weapon == null)
        {
            weapon = Instantiate(allWeaponPrefabs[(int)weaponType]).Initialize(player);
            weapons.Add((weaponType, weapon));
        }
        else
        {
            weapon.AddLevel();
        }

        int index = weapons.IndexOf((weaponType, weapon));

        playerHUD.SetWeaponSlot(index, weapon.Level, weapon.IconSprite);
    }

    private Weapon GetWeapon()
    {
        // If all are max level, instead, money & HP will come out
        if (weapons.Count >= 6)
        {
            int minLevel = maxLevel;
            foreach (var w in weapons)
            {
                if (minLevel > w.weapon.Level)
                    minLevel = w.weapon.Level;
            }

            if (minLevel == maxLevel)
                return weapons[0].weapon; // must be corrected
            else
            {
                int index = UnityEngine.Random.Range(0, 6);
                while (weapons[index].weapon.Level == maxLevel)
                {
                    index = UnityEngine.Random.Range(0, 6);
                }

                return weapons[index].weapon;
            }
        }
        else
        {
            int weaponType = UnityEngine.Random.Range(0, allWeaponPrefabs.Length);
            Weapon weapon = weapons.Where(o => o.type == (WeaponType)weaponType).FirstOrDefault().weapon;
            if (weapon != null)
            {
                while (weapon.Level == maxLevel)
                {
                    weaponType = UnityEngine.Random.Range(0, allWeaponPrefabs.Length);
                }
            }

            return allWeaponPrefabs[weaponType];
        }
    }
}
