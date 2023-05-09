using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ItemBuySlot : MonoBehaviour
{
    #region [ Variables ]
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    [SerializeField] private Button buyButton;

    private Action<ItemType> onClick;
    #endregion

    #region [ Public methods ]
    public void Initialize(ItemData data, int grade, Action<ItemType> onClick)
    {
        this.onClick = onClick;

        thumbnail.sprite = data.Sprite;
        name.text = data.Type.ToString();
        price.text = data.Prices[grade] + "$";

        SubscribeOnClickBuyButton(data.Type);
    }

    public void SetPrice(ItemData data, int grade)
    {
        if (grade >= data.Prices.Length)
            SetSoldOut();
        else
            price.text = data.Prices[grade] + "$";
    }
    #endregion

    #region [ Private methods ]
    private void SetSoldOut()
    {
        price.text = "0$";
        buyButtonText.text = "sold out";
        buyButton.interactable = false;
    }

    private void SubscribeOnClickBuyButton(ItemType type)
    {
        buyButton.onClick.AddListener(() => onClick?.Invoke(type));

/*        buyButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => 
            {
                onClick?.Invoke(type);
            })
            .AddTo(this);*/
    }
    #endregion
}