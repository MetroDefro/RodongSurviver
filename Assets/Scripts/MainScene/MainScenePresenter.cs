using UnityEngine;
using Zenject;
using RodongSurviver.Base;
using RodongSurviver.Manager;

public class MainScenePresenter : PresenterBase
{
    #region [ Variables ]
    [SerializeField] private PlayerSelectPresenter playerSelectPresenter;
    [SerializeField] private ShopPresenter shopPresenter;

    private GameManager gameManager;
    private SceneManager sceneManager;
    private PopupManager popupManager;
    private ItemDataContainer itemDataContainer;

    private MainSceneView view;
    #endregion

    #region [ MonoBehaviour Messages ]
    private void Start()
    {
        Initialize();
    }
    #endregion

    #region [ Public methods ]
    [Inject]
    public void Inject(GameManager gameManager, SceneManager sceneManager, PopupManager popupManager, ItemDataContainer itemDataContainer)
    {
        this.gameManager = gameManager;
        this.sceneManager = sceneManager;
        this.popupManager = popupManager;
        this.itemDataContainer = itemDataContainer;
    }

    public override void Dispose()
    {
        base.Dispose();
        view.Dispose();
    }

    public void Initialize()
    {
        if (TryGetComponent(out MainSceneView view))
        {
            this.view = view;
        }

        playerSelectPresenter.Initialize(
            () => sceneManager.LoadSceneAsync(SceneType.Game),
            () => shopPresenter.Show(),
            (playerData) => SetPlayerData(playerData));

        if(gameManager.EnforceData == null)
            gameManager.EnforceData = CreateEnforceData();

        shopPresenter.Initialize(itemDataContainer.BuffItemDatas, type => OnBuyItem(type), gameManager.EnforceData.Money);
        
        for(int i = 0; i < itemDataContainer.BuffItemDatas.Length; ++i)
            shopPresenter.SetPrice(i, itemDataContainer.BuffItemDatas[i], gameManager.EnforceData.BuffGrades[i]);
    }
    #endregion

    #region [ Private methods ]
    private EnforceData CreateEnforceData()
    {
        return new EnforceData()
        {
            Money = 0,
            BuffGrades = new int[itemDataContainer.BuffItemDatas.Length],
        };
    }

    private void SetPlayerData(PlayerData playerData)
    {
        gameManager.PlayerData = playerData;
    }

    private void OnBuyItem(ItemType type)
    {
        int index = (int)type - 100;
        int currentGrade = gameManager.EnforceData.BuffGrades[index];
        int currentPrice = itemDataContainer.BuffItemDatas[index].Prices[currentGrade];

        if(currentPrice <= gameManager.EnforceData.Money)
        {
            gameManager.EnforceData.Money -= currentPrice;
            gameManager.EnforceData.BuffGrades[index]++;
            shopPresenter.SetMoney(gameManager.EnforceData.Money);
            shopPresenter.SetPrice(index, itemDataContainer.BuffItemDatas[index], gameManager.EnforceData.BuffGrades[index]);
        }
        else
        {
            DefaultPopup popup = (DefaultPopup)popupManager.GetPopup();
            popup.SetConfirm(() => popup.HideImmediate()).SetContent("잔액이 부족합니다.");
            popup.ShowImmediate();
        }
    }
    #endregion
}