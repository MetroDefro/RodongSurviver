using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data")]
public class ItemData : ScriptableObject
{
    public ItemType Type => type;
    public Sprite Sprite => sprite;
    public string Explanation => explation;
    public int[] Prices => prices;
    public int MaxLevel => maxLevel;

    [SerializeField] private ItemType type;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string explation;
    [SerializeField] private int[] prices;
    [SerializeField] private int maxLevel;

    private LocalizedString localizedString;

    public void Initialize(TableReference tableReference)
    {
        localizedString = new LocalizedString(tableReference, (int)type);
        localizedString.StringChanged += UpdateString;
    }

    public void Dispose()
    {
        localizedString.StringChanged -= UpdateString;
    }

    private void UpdateString(string str)
    {
        explation = str;
    }
}