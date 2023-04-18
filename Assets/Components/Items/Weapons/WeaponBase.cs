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
    private void OnDestroy()
    {
        foreach (var weapon in weaponObjects)
            Destroy(weapon.gameObject);

        weaponObjects.Clear();
        disposables.Clear();
    }
    private void OnDisable()
    {
        weaponObjects.Clear();
    }
    #endregion

    #region [ Public methods ]
    public Weapon Initialize(Player player)
    {
        level = 1;
        this.player = player;

        InitObject(initData.Count);

        float size = CalculateSize();
        foreach (GameObject weapon in weaponObjects)
            weapon.transform.localScale = new Vector3(size, size, size);

        Movement();

        return this;
    }

    public void OnLevelUp()
    {
        level++;

        if(weaponObjects.Count < CalculateCount())
        {
            int count = CalculateCount() - weaponObjects.Count;
            InitObject(count);
        }

        float size = CalculateSize();
        foreach (GameObject weapon in weaponObjects)
            weapon.transform.localScale = new Vector3(size, size, size);
    }

    public void Pause()
    {
        disposables.Clear();
    }

    public void Play()
    {
        Movement();
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

    private void InitObject(int count)
    {
        float size = CalculateSize();
        for (int i = 0; i < count; i++)
        {
            GameObject weapon = Instantiate(initData.WeaponObject, transform);
            weaponObjects.Add(weapon);
            SubscribeOnCollisionStay2D(weapon);
            weapon.transform.localScale = new Vector3(size, size, size);
        }

        SetPosition();
    }
    #endregion

    #region [ Abstract methods ]
    protected abstract void Movement();

    protected abstract void SetPosition();

    protected virtual float CalculateDamage()
    {
        return initData.Damage * Level * player.Status.Damage;
    }
    protected virtual float CalculateSpeed()
    {
        return initData.Speed * Level * player.Status.WeaponSpeed;
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
        return initData.Count + player.Status.WeaponCount;
    }
    #endregion
}