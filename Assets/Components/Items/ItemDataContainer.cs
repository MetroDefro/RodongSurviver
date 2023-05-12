using UnityEngine;

public class ItemDataContainer : MonoBehaviour
{
    public Weapon[] WeaponPrefabs => weaponPrefabs;
    public ItemData[] BuffItemDatas => buffItemDatas;
    public ItemData MoneyItemData => moneyItemData;
    public ItemData PotionItemData => potionItemData;

    [SerializeField] private Weapon[] weaponPrefabs;
    [SerializeField] private ItemData[] buffItemDatas;
    [SerializeField] private ItemData moneyItemData;
    [SerializeField] private ItemData potionItemData;

    private void Awake()
    {
        foreach(Weapon weapon in WeaponPrefabs)
            weapon.Data.Initialize("DefaultTable");
        foreach (ItemData data in BuffItemDatas)
            data.Initialize("DefaultTable");
        moneyItemData.Initialize("DefaultTable");
        potionItemData.Initialize("DefaultTable");
    }

    private void OnDestroy()
    {
        foreach (Weapon weapon in WeaponPrefabs)
            weapon.Data.Dispose();
        foreach (ItemData data in BuffItemDatas)
            data.Dispose();
        moneyItemData.Dispose();
        potionItemData.Dispose();
    }
}
