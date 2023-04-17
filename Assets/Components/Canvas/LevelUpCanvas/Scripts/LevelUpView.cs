using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpView : MonoBehaviour
{
    public Button[] LevelUpButtons => levelUpButtons;
    public TextMeshProUGUI[] LevelUpButtonExplainTexts => levelUpButtonExplainTexts;

    [SerializeField] private Button[] levelUpButtons;
    [SerializeField] private TextMeshProUGUI[] levelUpButtonExplainTexts;
}
