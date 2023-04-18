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
    public List<(ItemType type, Weapon weapon)> Weapons { get; set; } = new List<(ItemType type, Weapon weapon)>();
    public List<(ItemType type, Buff buff)> Buffs { get; set; } = new List<(ItemType type, Buff buff)>();

    public float SpanSeconds = 0;
    public int MaxLevel = 6;
}

public class GameScenePresenter : PresenterBase
{
    #region [ Variables ]
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
    [SerializeField] private ItemData[] allBuffDatas = new ItemData[2];

    #endregion

    #region [ MonoBehaviour Messages ]
    private void Awake()
    {

    }

    private void Start()
    {
        Initialize();
    }
    #endregion

    #region [ Public methods ]

    [Inject]
    public void Inject(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public override void Dispose()
    {
        base.Dispose();
        view.Dispose();
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
            PauseGame();
        };
    #endregion

    #region [ Private methods ]
    private void Initialize()
    {
        if (TryGetComponent(out GameSceneView view))
        {
            this.view = view;
            this.view?.Show(null);
        }

        model = new GameSceneModel();


        foreach (var bg in backGroundMovers)
            bg.Initialize(player, gameArea);

        player.Initialize(gameManager.playerData, (necessaryEXP, exp) => OnGetEXP(necessaryEXP, exp), (level) => OnLevelUp(level));
        slotCanvasPresenter.Initialize();
        levelUpPresenter.Initialize((weaponType) => OnItemLevelUp(weaponType));
        pauseCanvasPresenter.Initialize(() => PlayGame());
        topCanvasPresenter.Initialize(() => 
        { 
            pauseCanvasPresenter.gameObject.SetActive(true); 
            PauseGame(); 
        });
        enemySpawner.Initialize(player, gameArea);

        Weapon firstWeapon = Instantiate(allWeaponPrefabs[(int)player.WeaponType]).Initialize(player);
        model.Weapons.Add((firstWeapon.Data.Type, firstWeapon));
        slotCanvasPresenter.SetWeaponSlot(0, firstWeapon.Level, firstWeapon.Data.Sprite);

        model.SpanSeconds = 0;
        SubscribeEveryUpdate();
    }

    private void OnGetEXP(float necessaryEXP, float exp)
    {
        topCanvasPresenter.SetEXPbar(Mathf.InverseLerp(0, necessaryEXP, exp));
    }

    private void OnLevelUp(int level)
    {
        PauseGame();
        topCanvasPresenter.SetLevelUp(level);
        levelUpPresenter.SetLevelUpPanel(new ItemData[] { GetItemData(), GetItemData(), GetItemData() });
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

    private void OnItemLevelUp(ItemType type)
    {
        IItem item = GetIsItemFromType(type);

        if ((int)type < 100)
        {
            if (item == null)
            {
                item = Instantiate(allWeaponPrefabs[(int)type]).Initialize(player);
                model.Weapons.Add((item.Data.Type, (Weapon)item));
            }
            else
                item.OnLevelUp();

            int index = model.Weapons.IndexOf((item.Data.Type, (Weapon)item));
            slotCanvasPresenter.SetWeaponSlot(index, item.Level, item.Data.Sprite);
        }
        else
        {
            if (item == null)
            {
                item = new Buff(player, allBuffDatas[(int)type - 100]);
                model.Buffs.Add((item.Data.Type, (Buff)item));
            }
            else
                item.OnLevelUp();

            int index = model.Buffs.IndexOf((item.Data.Type, (Buff)item));
            slotCanvasPresenter.SetBuffSlot(index, item.Level, item.Data.Sprite);
        }

        PlayGame();
    }

    private ItemData GetItemData()
    {
        // If all are max level, instead, money & HP will come out
        if (model.Weapons.Count >= 6 && model.Buffs.Count >= 6)
        {
            int minLevel = model.MaxLevel;
            foreach (var w in model.Weapons)
            {
                if (minLevel > w.weapon.Level)
                    minLevel = w.weapon.Level;
            }
            foreach (var b in model.Buffs)
            {
                if (minLevel > b.buff.Level)
                    minLevel = b.buff.Level;
            }

            if (minLevel == model.MaxLevel)
                return model.Weapons[0].weapon.Data; // must be corrected
        }

        ItemType type = GetRandomItemType();
        while (GetIsItemLevelMax(GetIsItemFromType(type)))
        {
            type = GetRandomItemType();
        }

        if ((int)type < 100)
            return allWeaponPrefabs[(int)type].Data;
        else
            return allBuffDatas[(int)type - 100];
    }

    private ItemType GetRandomItemType()
    {
        Array values = Enum.GetValues(typeof(ItemType));
        return (ItemType)values.GetValue(new System.Random().Next(0, values.Length));
    }

    private bool GetIsItemLevelMax(IItem item)
    {
        if (item == null)
            return false;
        else
            return item.Level == model.MaxLevel;
    }

    private IItem GetIsItemFromType(ItemType type)
    {
        IItem item = (int)type < 100 ? model.Weapons.Where(o => o.type == type).FirstOrDefault().weapon : model.Buffs.Where(o => o.type == type).FirstOrDefault().buff;

        return item;
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
