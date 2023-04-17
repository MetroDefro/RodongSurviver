using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class SlotsCanvasPresenter : MonoBehaviour
{
    private SlotsCanvasView view;

    public void Dispose()
    {
        for (int i = 0; i < view.WeaponSlots.Length; i++)
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

    public void SetWeaponSlot(int index, int Level, Sprite sprite)
    {
        view.WeaponSlots[index].sprite = sprite;
        view.WeaponSlotLevelTexts[index].text = "" + Level;
    }

    public void SetItemSlot(int index, int Level, Sprite sprite)
    {
        view.ItemSlots[index].sprite = sprite;
        view.ItemSlotLevelTexts[index].text = "" + Level;
    }
}