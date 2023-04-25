using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UniRx;
using Zenject;
using RodongSurviver.Base;
using RodongSurviver.Manager;
using RodongSurviver.Components.DiedPanel;

public class GameSceneModel
{
    public List<(ItemType type, Weapon weapon)> Weapons { get; set; } = new List<(ItemType type, Weapon weapon)>();
    public List<(ItemType type, Buff buff)> Buffs { get; set; } = new List<(ItemType type, Buff buff)>();

    public int SpanSeconds = 0;
    public int MaxLevel = 6;
}

public class GameScenePresenter : PresenterBase
{
    #region [ Variables ]
    private GameManager gameManager;
    private SceneManager sceneManager;

    private GameSceneView view;
    private GameSceneModel model;

    [SerializeField] private Player player;
    [SerializeField] private SlotsCanvasPresenter slotCanvasPresenter;
    [SerializeField] private LevelUpPresenter levelUpPresenter;
    [SerializeField] private PauseCanvasPresenter pauseCanvasPresenter;
    [SerializeField] private TopCanvasPresenter topCanvasPresenter;
    [SerializeField] private DiedPanelPresenter diedPanelPresenter;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BackGroundMover[] backGroundMovers;
    private BoxCollider2D gameArea;

    [SerializeField] private Weapon[] allWeaponPrefabs = new Weapon[6];
    [SerializeField] private ItemData[] allBuffDatas = new ItemData[2];

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
    public void Inject(GameManager gameManager, SceneManager sceneManager)
    {
        this.gameManager = gameManager;
        this.sceneManager = sceneManager;
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
        slotCanvasPresenter.Dispose();
        levelUpPresenter.Dispose();
        pauseCanvasPresenter.Dispose();
        topCanvasPresenter.Dispose();
        diedPanelPresenter.Dispose();

        enemySpawner.Dispose();
        foreach (var weapon in model.Weapons)
            DestroyImmediate(weapon.weapon.gameObject);

        model.Weapons.Clear();
        model.Buffs.Clear();
        Initialize();
    }

    public void SetupDiedAction(Action onDied) { }
/*        player.OnDied = () =>
        {
            onDied.Invoke();
            PauseGame();
        };*/
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

        gameArea = player.GameArea;

        foreach (var bg in backGroundMovers)
            bg.Initialize(player, gameArea);

        player.Initialize(gameManager.playerData, new Player.PlayerActions()
        {
            OnLevelUp = (level) => OnLevelUp(level),
            OnGetEXP = (necessaryEXP, exp) => OnGetEXP(necessaryEXP, exp),
            OnGetMoney = (money) => OnMoney(money),
            OnDied = () =>
            {
                diedPanelPresenter.gameObject.SetActive(true);
                PauseGame();
            }

        });
        slotCanvasPresenter.Initialize();
        levelUpPresenter.Initialize((weaponType) => OnItemLevelUp(weaponType));
        pauseCanvasPresenter.Initialize(new PauseCanvasPresenter.PauseCanvasActions
        {
            OnPlay = () => PlayGame(),
            OnRetry = () => OnReset(),
            OnHome = () => LoadMainScene()
        });
        diedPanelPresenter.Initialize(new DiedPanelPresenter.DiedPanelActions()
        {
            RetryEvent = () =>
            {
                diedPanelPresenter.gameObject.SetActive(false);
                OnReset();
            },
            HomeEvent = () => LoadMainScene()
        });
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

        SubscribeTimer();
    }

    private void Load()
    {
        player = Instantiate(player);
        slotCanvasPresenter = Instantiate(slotCanvasPresenter);
        levelUpPresenter = Instantiate(levelUpPresenter);
        pauseCanvasPresenter = Instantiate(pauseCanvasPresenter);
        topCanvasPresenter = Instantiate(topCanvasPresenter);
        diedPanelPresenter = Instantiate(diedPanelPresenter);
        enemySpawner = Instantiate(enemySpawner);
    }

    private void LoadMainScene()
    {
        sceneManager.LoadSceneAsync(SceneType.Main);
        Dispose();
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

    private void PauseGame()
    {
        player.Pause();
        Disposables.Clear();
        enemySpawner.Pause();
        model.Weapons.ForEach((weapon) => weapon.weapon.Pause());
    }

    private void PlayGame()
    {
        SubscribeTimer();
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

    private void SubscribeTimer()
    {
        CreateTimerObservable()
            .Subscribe(second => 
            {
                model.SpanSeconds++;

                topCanvasPresenter.SetTimer(model.SpanSeconds);

                if (model.SpanSeconds % 30 == 0)
                    enemySpawner.MaxEnemyCount = enemySpawner.InitEnemyCount * (model.SpanSeconds / 30 + 1) ;

                if (model.SpanSeconds % 60 == 0)
                {
                    enemySpawner.CurrentEnemyIndex = (model.SpanSeconds / 60);
                }
 
            })
            .AddTo(Disposables);
    }

    private IObservable<int> CreateTimerObservable()
    {
        return Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Select(second => (int)second++);
    }
    #endregion
}
