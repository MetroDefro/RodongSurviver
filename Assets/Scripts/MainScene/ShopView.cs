using RodongSurviver.Base;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : ViewBase
{
    public RectTransform Content => content;
    public TextMeshProUGUI MoneyText => moneyText;
    public Button CloseButton => closeButton;

    [SerializeField] private RectTransform content;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button closeButton;

    public override void Dispose()
    {

    }

    public override void Hide(Action onComplete)
    {
        HideImmediate();
    }

    public override void HideImmediate()
    {
        gameObject.SetActive(false);
    }

    public override void Show(Action onComplete)
    {
        ShowImmediate();
    }

    public override void ShowImmediate()
    {
        gameObject.SetActive(true);
    }
}
