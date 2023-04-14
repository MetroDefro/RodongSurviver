using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsCanvasView : MonoBehaviour
{
    public Image[] WeaponSlots => weaponSlots;
    public Image[] ItemSlots => itemSlots;
    public TextMeshProUGUI[] WeaponSlotLevelTexts => weaponSlotLevelTexts;
    public TextMeshProUGUI[] ItemSlotLevelTexts => itemSlotLevelTexts;


    [SerializeField] private Image[] weaponSlots;
    [SerializeField] private Image[] itemSlots;
    [SerializeField] private TextMeshProUGUI[] weaponSlotLevelTexts;
    [SerializeField] private TextMeshProUGUI[] itemSlotLevelTexts;
}
