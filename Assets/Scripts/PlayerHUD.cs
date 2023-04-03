using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private RectTransform HPbar;
    private float maxHPbarWidth;

    public void Initialize()
    {
        maxHPbarWidth = HPbar.sizeDelta.x;
    }

    public void SetHPbar(float normalizeHP)
    {
        if (maxHPbarWidth == 0)
            return;

        HPbar.sizeDelta = new Vector2(maxHPbarWidth * normalizeHP, HPbar.sizeDelta.y);
    }
}
