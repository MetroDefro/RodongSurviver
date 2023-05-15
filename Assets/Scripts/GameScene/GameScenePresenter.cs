using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using Zenject;
using RodongSurviver.Base;
using RodongSurviver.Manager;
using RodongSurviver.Components.DiedPanel;
using System.Collections;
using UnityEngine.InputSystem;

public class GameSceneModel
{
    public List<(ItemType type, Weapon weapon)> Weapons { get; set; } = new List<(ItemType type, Weapon weapon)>();
    public List<(ItemType type, Buff buff)> Buffs { get; set; } = new List<(ItemType type, Buff buff)>();

    public readonly int MaxCount = 6;
    public readonly int PotionRecoveryAmount = 5;
    public readonly int MoneyAmount = 1;
}

public class GameScenePresenter : PresenterBase
{
    #region [ Variables ]
    [SerializeField] private Player player;
    [SerializeField] private SlotsCanvasPresenter slotCanvasPresenter;
    [SerializeField] private LevelUpPresenter levelUpPresenter;
    [SerializeField] private PauseCanvasPresenter pauseCanvasPresenter;
    [SerializeField] private TopCanvasPresenter topCanvasPresenter;
    [SerializeField] private ResultPanelPresenter resultPanelPresenter;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BackGroundMover[] backGroundMovers;

    private BoxCollider2D gameArea;

    private Weapon[] allWeaponPrefabs;
    private ItemData[] allBuffDatas;
    private ItemData potionData;
    private ItemData moneyData;

    private GameManager gameManager;
    private SceneManager sceneManager;
    private ItemDataContainer itemDataContainer;

    private GameSceneView view;
    private GameSceneModel model;

    #endregion

    #region [ MonoBehaviour Messages ]
    private void Awake()
    {

    }

    private void Start()
    {
        Load();

        Initialize();
    }
    #endregion

    #region [ Public methods ]

    [Inject]
    public void Inject(GameManager gameManager, SceneManager sceneManager, ItemDataContainer itemDataContainer)
    {
        this.gameManager = gameManager;
        this.sceneManager = sceneManager;
        this.itemDataContainer = itemDataContainer;
    }

    public override void Dispose()
    {
        base.Dispose();
        view.Dispose();

        Disposables.Dispose();
        player.Dispose();
        slotCanvasPresenter.Dispose();
        levelUpPresenter.Dispose();
        pauseCanvasPresenter.Dispose();
        topCanvasPresenter.Dispose();
        resultPanelPresenter.Dispose();

        enemySpawner.Dispose();
        foreach (var weapon in model.Weapons)
            weapon.weapon.Dispose();

        model.Weapons.Clear();
        model.Buffs.Clear();
    }

    public void OnReset()
    {
        gameManager.PluseMoney(player.Status.Money.Value);

        Disposables.Clear();
        player.Reset();
        slotCanvasPresenter.Dispose();
        levelUpPresenter.Dispose();
        pauseCanvasPresenter.Dispose();
        topCanvasPresenter.Dispose();
        resultPanelPresenter.Dispose();

        enemySpawner.Reset();
        foreach (var weapon in model.Weapons)
            weapon.weapon.Dispose();

        model.Weapons.Clear();
        model.Buffs.Clear();
        Initialize();

        PlayGame();
    }

    public void SetupDiedAction(Action onDied) { }
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

        InitializeUI();

        allWeaponPrefabs = itemDataContainer.WeaponPrefabs;
        allBuffDatas = itemDataContainer.BuffItemDatas;
        moneyData = itemDataContainer.MoneyItemData;
        potionData = itemDataContainer.PotionItemData;

        gameArea = player.GameArea;

        foreach (var bg in backGroundMovers)
            bg.Initialize(player, gameArea);

        player.Initialize(gameManager.PlayerData, new Player.PlayerActions()
        {
            OnLevelUp = (level) => OnLevelUp(level),
            OnGetEXP = (necessaryEXP, exp) => OnGetEXP(necessaryEXP, exp),
            OnGetMoney = (money) => OnMoney(money),
            OnDied = () => GameOver(),
        });
        player.Status.SetEnforce(gameManager.EnforceData);

        enemySpawner.Initialize(player, gameArea);

        Weapon firstWeapon = Instantiate(allWeaponPrefabs[(int)player.WeaponType]).Initialize(player);
        model.Weapons.Add((firstWeapon.Data.Type, firstWeapon));
        slotCanvasPresenter.SetWeaponSlot(0, firstWeapon.Level, firstWeapon.Data.Sprite);

        PlayGame();
        SubscribeTimer();
    }

    private void InitializeUI()
    {
        slotCanvasPresenter.Initialize();
        levelUpPresenter.Initialize((weaponType) => OnItemLevelUp(weaponType));
        pauseCanvasPresenter.Initialize(new PauseCanvasPresenter.PauseCanvasActions
        {
            OnPlay = () => PlayGame(),
            OnRetry = () => OnReset(),
            OnHome = () => LoadMainScene()
        });
        resultPanelPresenter.Initialize(new ResultPanelPresenter.DiedPanelActions()
        {
            RetryEvent = () =>
            {
                resultPanelPresenter.Hide();
                OnReset();
            },
            HomeEvent = () => LoadMainScene()
        });
        topCanvasPresenter.Initialize(() =>
        {
            pauseCanvasPresenter.gameObject.SetActive(true);
            PauseGame();
        });
    }

    private void Load()
    {
        player = Instantiate(player);
        slotCanvasPresenter = Instantiate(slotCanvasPresenter);
        levelUpPresenter = Instantiate(levelUpPresenter);
        pauseCanvasPresenter = Instantiate(pauseCanvasPresenter);
        topCanvasPresenter = Instantiate(topCanvasPresenter);
        resultPanelPresenter = Instantiate(resultPanelPresenter);
        enemySpawner = Instantiate(enemySpawner);
    }

    private void LoadMainScene()
    {
        gameManager.PluseMoney(player.Status.Money.Value);

        sceneManager.LoadSceneAsync(SceneType.Main);
        Dispose();
    }

    private void PauseGame() => gameManager.PauseGame();

    private void PlayGame() => gameManager.PlayGame();

    private void GameOver()
    {
        resultPanelPresenter.Show(false);
        PauseGame();
    }

    private void GameClaer()
    {
        resultPanelPresenter.Show(true);
        PauseGame();
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

    private void OnMoney(int money)
    {
        topCanvasPresenter.SetMoney(money);
    }


    private void OnItemLevelUp(ItemType type)
    {
        if((int)type < 200)
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
        }
        else
        {
            if (type == ItemType.Potion)
                player.Status.PlusHP(model.PotionRecoveryAmount);
            else if (type == ItemType.Money)
                player.Status.AddMoney(model.MoneyAmount);
        }

        PlayGame();

        StartCoroutine(CheckNeedMoreLevelUp());
    }

    private IEnumerator CheckNeedMoreLevelUp()
    {
        yield return null;
        player.AddEXP(0);
    }

    private ItemData GetItemData()
    {
        bool isWeaponSlotFull = model.Weapons.Count >= model.MaxCount;
        bool isBuffSlotFull = model.Buffs.Count >= model.MaxCount;

        if (isWeaponSlotFull && isBuffSlotFull)
        {
            if (GetIsWeaponMaxLevel() && GetIsBuffnMaxLevel())
            {
                bool isPotion = UnityEngine.Random.Range(0, 2) == 0;

                return isPotion ? potionData : moneyData;
            }
        }
        
        ItemType type = GetRandomItemType();
        while (GetIsItemLevelMax(GetIsItemFromType(type)))
        {
            type = GetRandomItemType();
        }

        bool isWeapon = (int)type < 100;

        return isWeapon ? allWeaponPrefabs[(int)type].Data : allBuffDatas[(int)type - 100];


        ItemType GetRandomItemType()
        {
            ItemType type;
            bool isWeapon = UnityEngine.Random.Range(0, 2) == 0;

            if (isWeaponSlotFull && GetIsWeaponMaxLevel())
                isWeapon = false;
            else if (isBuffSlotFull && GetIsBuffnMaxLevel())
                isWeapon = true;

            if (isWeapon)
                type = isWeaponSlotFull ? GetRandomItemTypeInWeaponInSlot() : GetRandomItemTypeInWeapon();
            else
                type = isBuffSlotFull ? GetRandomItemTypeInBuffInSlot() : GetRandomItemTypeInBuff();

            return type;
        }
    }

    private bool GetIsWeaponMaxLevel() => model.Weapons.Where(w => w.weapon.Level < w.weapon.Data.MaxLevel).FirstOrDefault().weapon == null;
    private bool GetIsBuffnMaxLevel() => model.Buffs.Where(w => w.buff.Level < w.buff.Data.MaxLevel).FirstOrDefault().buff == null;
    private ItemType GetRandomItemTypeInWeapon() => (ItemType)UnityEngine.Random.Range(0, allWeaponPrefabs.Length);
    private ItemType GetRandomItemTypeInBuff() => (ItemType)(UnityEngine.Random.Range(0, allBuffDatas.Length) + 100);
    private ItemType GetRandomItemTypeInWeaponInSlot() => model.Weapons[UnityEngine.Random.Range(0, model.Weapons.Count)].type;
    private ItemType GetRandomItemTypeInBuffInSlot() => model.Buffs[UnityEngine.Random.Range(0, model.Buffs.Count)].type;

    private bool GetIsItemLevelMax(IItem item)
    {
        if (item == null)
            return false;
        else
            return item.Level == item.Data.MaxLevel;
    }

    private IItem GetIsItemFromType(ItemType type)
    {
        IItem item = (int)type < 100 ? model.Weapons.Where(o => o.type == type).FirstOrDefault().weapon : model.Buffs.Where(o => o.type == type).FirstOrDefault().buff;

        return item;
    }

    private void SubscribeTimer()
    {
        CreateTimerObservable()
            .Subscribe(second => 
            {
                topCanvasPresenter.SetTimer(second);

                if (second % 20 == 0)
                    enemySpawner.MaxEnemyCount = enemySpawner.InitEnemyCount * (second / 20 + 1) ;

                if (second % 60 == 0)
                    enemySpawner.CurrentEnemyIndex = (second / 60);

                if (second == 600)
                    GameClaer();
            })
            .AddTo(Disposables);
    }

    private IObservable<int> CreateTimerObservable()
    {
        return Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Select(second => (int)second++);
    }

    private void OnPause(InputValue value)
    {
        if (pauseCanvasPresenter.gameObject.activeSelf)
        {
            pauseCanvasPresenter.gameObject.SetActive(false);
            PlayGame();
        }
        else
        {
            pauseCanvasPresenter.gameObject.SetActive(true);
            PauseGame();
        }

    }
    #endregion
}