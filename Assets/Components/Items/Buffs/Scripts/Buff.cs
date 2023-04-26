public class Buff : IItem
{
    public int Level { get => level; set => level = value; }
    public ItemData Data { get => data; set => data = value; }

    private Player player;
    private ItemData data;
    private int level;

    public Buff(Player player, ItemData data)
    {
        this.player = player;
        this.data = data;

        level = 0;
        OnLevelUp();
    }

    public void OnLevelUp()
    {
        level++;

        switch (data.Type)
        {
            case ItemType.Magnet:
                player.Status.AddMagnetism(1.2f);
                break;

            case ItemType.Tornado:
                player.Status.AddWeaponSpeed(1.1f);
                break;

            case ItemType.Thunder:
                player.Status.AddWeaponTerm(1.1f);
                break;

            case ItemType.Dumbbell:
                player.Status.AddDamage(1.2f);
                break;

            case ItemType.Shoes:
                player.Status.AddSpeed(1.1f);
                break;

            case ItemType.Rice:
                player.Status.AddMaxpHP(1.2f);
                break;

            case ItemType.Dice:
                player.Status.AddWeaponCount();
                break;

            case ItemType.Baloon:
                player.Status.AddWeaponSize(1.1f);
                break;

            default:
                break;
        }
    }
}
