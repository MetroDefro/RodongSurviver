using RodongSurviver.Base;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsCanvasView : ViewBase
{
    #region [ Properties ]
    public Image[] WeaponSlots => weaponSlots;
    public Image[] BuffSlots => buffSlots;
    public TextMeshProUGUI[] WeaponSlotLevelTexts => weaponSlotLevelTexts;
    public TextMeshProUGUI[] BuffSlotLevelTexts => buffSlotLevelTexts;
    #endregion

    #region [ Variables ]
    [SerializeField] private Image[] weaponSlots;
    [SerializeField] private Image[] buffSlots;
    [SerializeField] private TextMeshProUGUI[] weaponSlotLevelTexts;
    [SerializeField] private TextMeshProUGUI[] buffSlotLevelTexts;
    #endregion

    #region [ Public methods ]
    public void SetWeaponSlot(int index, int level, Sprite sprite)
    {
        weaponSlots[index].sprite = sprite;
        weaponSlotLevelTexts[index].text = "" + level;
    }

    public void SetBuffSlot(int index, int level, Sprite sprite)
    {
        buffSlots[index].sprite = sprite;
        buffSlotLevelTexts[index].text = "" + level;
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
