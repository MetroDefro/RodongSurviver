using RodongSurviver.Base;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsCanvasView : ViewBase
{
    #region [ Properties ]
    public Image[] WeaponSlots => weaponSlots;
    public Image[] ItemSlots => itemSlots;
    public TextMeshProUGUI[] WeaponSlotLevelTexts => weaponSlotLevelTexts;
    public TextMeshProUGUI[] ItemSlotLevelTexts => itemSlotLevelTexts;
    #endregion

    #region [ Variables ]
    [SerializeField] private Image[] weaponSlots;
    [SerializeField] private Image[] itemSlots;
    [SerializeField] private TextMeshProUGUI[] weaponSlotLevelTexts;
    [SerializeField] private TextMeshProUGUI[] itemSlotLevelTexts;
    #endregion

    #region [ Public methods ]
    public void SetWeaponSlot(int index, int level, Sprite sprite)
    {
        weaponSlots[index].sprite = sprite;
        weaponSlotLevelTexts[index].text = "" + level;
    }

    public void SetItemSlot(int index, int level, Sprite sprite)
    {
        itemSlots[index].sprite = sprite;
        itemSlotLevelTexts[index].text = "" + level;
    }

    public override void Show(Action onComplete)
    {

    }

    public override void ShowImmediate()
    {

    }

    public override void Hide(Action onComplete)
    {

    }

    public override void HideImmediate()
    {

    }

    public override void Dispose()
    {

    }
    #endregion
}
