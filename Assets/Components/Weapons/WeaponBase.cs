using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public abstract class WeaponBase : MonoBehaviour
{
    #region [ Properties ]
    public WeaponType WeaponType => data.WeaponType;
    public string Explanation => data.Explanation;
    public Sprite IconSprite => data.IconSprite;
    public int Level => level;
    #endregion

    #region [ Variables ]
    [SerializeField] protected WeaponData data;
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
    public WeaponBase Initialize(Player player)
    {
        level = 1;
        this.player = player;

        InitObject(data.Count);

        float size = CalculateSize();
        foreach (GameObject weapon in weaponObjects)
            weapon.transform.localScale = new Vector3(size, size, size);

        Movement();

        return this;
    }

    public int AddLevel()
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

        return level;
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
            GameObject weapon = Instantiate(data.WeaponObject, transform);
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

    protected abstract float CalculateDamage();
    protected abstract float CalculateSpeed();
    protected abstract float CalculateTerm();
    protected abstract float CalculateSize();
    protected abstract float CalculateRange();
    protected abstract int CalculateCount();
    #endregion
}
