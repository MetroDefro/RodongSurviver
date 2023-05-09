using RodongSurviver.Base;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShopPresenter : PresenterBase
{
    #region [ Variables ]
    [SerializeField] private ItemBuySlot itemBuySlotPrefab;

    private List<ItemBuySlot> itemBuySlots = new List<ItemBuySlot>();

    private ShopView view;
    #endregion

    #region [ Public methods ]
    public void Initialize(ItemData[] itemDatas , Action<ItemType> onClick, int money)
    {
        if (TryGetComponent(out ShopView view))
            this.view = view;

        foreach (ItemData data in itemDatas)
        {
            ItemBuySlot slot = Instantiate(itemBuySlotPrefab, view.Content);
            slot.Initialize(data, 0, (type) => onClick?.Invoke(type));
            itemBuySlots.Add(slot);
        }

        SetMoney(money);
        SubscribeCloseButton();
    }

    public void Hide()
    {
        view.HideImmediate();
    }

    public void Show()
    {
        view.ShowImmediate();
    }

    public void SetPrice(int index, ItemData itemData, int grade)
    {
        itemBuySlots[index].SetPrice(itemData, grade);
    }
    #endregion

    #region [ Private methods ]
    public void SetMoney(int money)
    {
        view.MoneyText.text = money + "$";
    }

    private void SubscribeCloseButton()
    {
        view.CloseButton.OnClickAsObservable()
            .Subscribe(_ => 
            {
                Hide();
            })
            .AddTo(this);
    }
    #endregion
}
