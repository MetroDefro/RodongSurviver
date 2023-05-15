using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public abstract class Weapon : MonoBehaviour, IItem
{
    #region [ Properties ]
    public int Level { get => level; set => level = value; }
    public ItemData Data { get => data; set => data = value; }
    #endregion

    #region [ Variables ]
    [SerializeField] private ItemData data;
    [SerializeField] protected WeaponData initData;
    protected Player player;
    protected int level;
    protected bool isWaitingTime;

    protected List<GameObject> weaponObjects = new List<GameObject>();

    protected CompositeDisposable disposables = new CompositeDisposable();
    #endregion

    #region [ MonoBehaviour Messages ]
    private void OnDisable()
    {

    }
    #endregion

    #region [ Public methods ]
    public Weapon Initialize(Player player)
    {
        level = 1;
        this.player = player;

        SubscribeOnCountValueChange();
        SubscribeOnSizeValueChange();

        Movement();

        return this;
    }

    public void Dispose()
    {
        foreach (var weapon in weaponObjects)
            Destroy(weapon.gameObject);

        weaponObjects.Clear();
        disposables.Clear();

        Destroy(gameObject);
    }

    public void OnLevelUp()
    {
        level++;

        SetSize();
    }

    #endregion

    #region [ Private methods ]
    private void SubscribeOnCollisionStay2D(GameObject weapon)
    {
        weapon.OnCollisionStay2DAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1))
            .Subscribe(collision => 
            {
                if (collision.gameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.Hit(CalculateDamage());
                }
            }).AddTo(this);
    }

    private void InitObjects()
    {
        if (weaponObjects.Count < CalculateCount())
        {
            int count = CalculateCount() - weaponObjects.Count;
            float size = CalculateSize();
            for (int i = 0; i < count; i++)
            {
                GameObject weapon = Instantiate(initData.WeaponObject, transform);
                weaponObjects.Add(weapon);
                SubscribeOnCollisionStay2D(weapon);
                weapon.transform.localScale = new Vector3(size, size, size);
            }
        }
    }

    private void SetSize()
    {
        float size = CalculateSize();
        foreach (GameObject weapon in weaponObjects)
            weapon.transform.localScale = new Vector3(size, size, size);
    }

    private void SubscribeOnCountValueChange()
    {
        player.Status.WeaponCount.Subscribe(_ => InitObjects()).AddTo(this);
    }

    private void SubscribeOnSizeValueChange()
    {
        player.Status.WeaponCount.Subscribe(_ => SetSize()).AddTo(this);
    }
    #endregion

    #region [ Abstract methods ]
    protected abstract void Movement();

    protected virtual void SetPosition()
    {
        InitObjects();
    }

    protected virtual float CalculateDamage()
    {
        return initData.Damage * Level * player.Status.Damage.Value;
    }
    protected virtual float CalculateSpeed()
    {
        return initData.Speed * Level * player.Status.WeaponSpeed.Value;
    }
    protected virtual float CalculateTerm()
    {
        return initData.Term;
    }
    protected virtual float CalculateSize()
    {
        return initData.Size;
    }
    protected virtual float CalculateRange()
    {
        return initData.Range;
    }
    protected virtual int CalculateCount()
    {
        return initData.Count + player.Status.WeaponCount.Value;
    }
    #endregion
}