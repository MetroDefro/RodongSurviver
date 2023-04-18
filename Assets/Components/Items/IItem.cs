public interface IItem
{
    public int Level { get; set; }
    public ItemData Data { get; set; }

    public void OnLevelUp();
}
