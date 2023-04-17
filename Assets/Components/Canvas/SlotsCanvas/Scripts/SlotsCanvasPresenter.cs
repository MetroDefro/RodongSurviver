using UnityEngine;
using RodongSurviver.Base;

public class SlotsCanvasPresenter : PresenterBase
{
    #region [ Variables ]
    private SlotsCanvasView view;
    #endregion

    #region [ Public methods ]
    public override void Dispose()
    {
        int count = view.WeaponSlots.Length;
        for (int i = 0; i < count; i++)
        {
            SetWeaponSlot(i, 1, null);
        }
    }

    public void Initialize()
    {
        if (TryGetComponent(out SlotsCanvasView view))
        {
            this.view = view;
        }
    }

    public void SetWeaponSlot(int index, int level, Sprite sprite)
    {
        view.SetWeaponSlot(index, level, sprite);
    }

    public void SetItemSlot(int index, int level, Sprite sprite)
    {
        view.SetItemSlot(index, level, sprite);
    }
    #endregion
}