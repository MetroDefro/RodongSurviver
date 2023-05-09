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
}
