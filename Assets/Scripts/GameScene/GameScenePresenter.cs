using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using Zenject;
using RodongSurviver.Base;
using RodongSurviver.Manager;

public class GameSceneModel
{
    public List<(WeaponType type, Weapon weapon)> Weapons { get; set; } = new List<(WeaponType, Weapon)>();
    // private List<Item> items = new List<Item>();

    public float SpanSeconds = 0;
    public int MaxLevel = 6;
}

public class GameScenePresenter : PresenterBase
{
    #region variable
    private GameManager gameManager;

    private GameSceneView view;
    private GameSceneModel model;

    [SerializeField] private Player player;
    [SerializeField] private SlotsCanvasPresenter slotCanvasPresenter;
    [SerializeField] private LevelUpPresenter levelUpPresenter;
    [SerializeField] private PauseCanvasPresenter pauseCanvasPresenter;
    [SerializeField] private TopCanvasPresenter topCanvasPresenter;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BackGroundMover[] backGroundMovers;
    [SerializeField] private BoxCollider2D gameArea;

    [SerializeField] private Weapon[] allWeaponPrefabs = new Weapon[6];
    
    #endregion

    #region monoBehaviour message
    private void Awake()
    {

    }

    private void Start()
    {
        Initialize();
    }
    #endregion

    #region public method

    public override void Dispose()
    {
        base.Dispose();
        view.Dispose();
    }

    public void OnGameOver()
    {
        PauseGame();
    }

    public void OnReset()
    {
        Disposables.Clear();
        player.Dispose();
        topCanvasPresenter.Dispose();
        enemySpawner.Dispose();
        foreach (var weapon in model.Weapons)
            DestroyImmediate(weapon.weapon.gameObject);

        model.Weapons.Clear();
        Initialize();
    }

    public void SetupDiedAction(Action onDied) => 
        player.OnDied = () =>
        {
            onDied.Invoke();
            OnGameOver();
        };

    [Inject]
    public void Inject(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void Initialize()
    {
        if (TryGetComponent(out GameSceneView view))
        {
            this.view = view;
            this.view?.Show(null);
        }

        model = new GameSceneModel();


        foreach (var bg in backGroundMovers)
            bg.Initialize(player, gameArea);

        player.Initialize(topCanvasPresenter, gameManager.playerData, (level) => OnLevelUp(level));
        slotCanvasPresenter.Initialize();
        levelUpPresenter.Initialize((weaponType) =>
        {
            OnWeaponLevelUp(weaponType);
            PlayGame();
        });
        pauseCanvasPresenter.Initialize(() => 
        {
            pauseCanvasPresenter.gameObject.SetActive(true);
            PlayGame(); 
        });
        topCanvasPresenter.Initialize(() => PauseGame());
        enemySpawner.Initialize(player, gameArea);

        Weapon firstWeapon = Instantiate(allWeaponPrefabs[(int)player.WeaponType]).Initialize(player);
        model.Weapons.Add((player.WeaponType, firstWeapon));
        slotCanvasPresenter.SetWeaponSlot(0, firstWeapon.Level, firstWeapon.IconSprite);

        model.SpanSeconds = 0;
        SubscribeEveryUpdate();
    }
    #endregion

    #region private Method

    private void OnLevelUp(int level)
    {
        PauseGame();
        topCanvasPresenter.SetLevelUp(level);
        levelUpPresenter.SetLevelUpPanel(new Weapon[] { GetWeapon(), GetWeapon(), GetWeapon() });
    }

    private void PauseGame()
    {
        player.Pause();
        Disposables.Clear();
        enemySpawner.Pause();
        model.Weapons.ForEach((weapon) => weapon.weapon.Pause());
    }

    private void PlayGame()
    {
        SubscribeEveryUpdate();
        player.Play();
        enemySpawner.Play();
        model.Weapons.ForEach((weapon) => weapon.weapon.Play());
    }

    private void OnWeaponLevelUp(WeaponType weaponType)
    {
        Weapon weapon = model.Weapons.Where(o => o.type == weaponType).FirstOrDefault().weapon;
        if (weapon == null)
        {
            weapon = Instantiate(allWeaponPrefabs[(int)weaponType]).Initialize(player);
            model.Weapons.Add((weaponType, weapon));
        }
        else
        {
            weapon.AddLevel();
        }

        int index = model.Weapons.IndexOf((weaponType, weapon));

        slotCanvasPresenter.SetWeaponSlot(index, weapon.Level, weapon.IconSprite);
    }

    private Weapon GetWeapon()
    {
        // If all are max level, instead, money & HP will come out
        if (model.Weapons.Count >= 6)
        {
            int minLevel = model.MaxLevel;
            foreach (var w in model.Weapons)
            {
                if (minLevel > w.weapon.Level)
                    minLevel = w.weapon.Level;
            }

            if (minLevel == model.MaxLevel)
                return model.Weapons[0].weapon; // must be corrected
            else
            {
                int index = UnityEngine.Random.Range(0, 6);
                while (model.Weapons[index].weapon.Level == model.MaxLevel)
                {
                    index = UnityEngine.Random.Range(0, 6);
                }

                return model.Weapons[index].weapon;
            }
        }
        else
        {
            int weaponType = UnityEngine.Random.Range(0, allWeaponPrefabs.Length);
            Weapon weapon = model.Weapons.Where(o => o.type == (WeaponType)weaponType).FirstOrDefault().weapon;
            if (weapon != null)
            {
                while (weapon.Level == model.MaxLevel)
                {
                    weaponType = UnityEngine.Random.Range(0, allWeaponPrefabs.Length);
                }
            }

            return allWeaponPrefabs[weaponType];
        }
    }

    private void SubscribeEveryUpdate()
    {
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                model.SpanSeconds += Time.deltaTime;
                topCanvasPresenter.SetTimer(model.SpanSeconds);

                if (model.SpanSeconds % 30 < 0.1f)
                    enemySpawner.MaxEnemyCount = enemySpawner.InitEnemyCount + enemySpawner.InitEnemyCount * Mathf.FloorToInt(model.SpanSeconds / 30);
            }).AddTo(Disposables);
    }

    #endregion
}
