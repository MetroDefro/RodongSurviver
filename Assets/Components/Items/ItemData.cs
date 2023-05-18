using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data")]
public class ItemData : ScriptableObject
{
    public ItemType Type => type;
    public Sprite Sprite => sprite;
    public string Explanation => localizedString.GetLocalizedString();
    public int[] Prices => prices;
    public int MaxLevel => maxLevel;

    [SerializeField] private ItemType type;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int[] prices;
    [SerializeField] private int maxLevel;

    private LocalizedString localizedString;

    private void Awake()
    {
        localizedString = new LocalizedString("DefaultTable", "ITEM-" + type);
    }
}