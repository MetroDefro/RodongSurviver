using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectView : MonoBehaviour
{
    public List<PlayerButtonSet> PlayeButtonSets => playeButtonSets;
    public Button PlayButton => playButton;
    public Button ShopButton => shopButton;
    public Text SelectLanguageText => selectLanguageText;
    public Dropdown SelectLanguageDropdown => selectLanguageDropdown;

    [SerializeField] private Button playButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Text selectLanguageText;
    [SerializeField] private Dropdown selectLanguageDropdown;
    [SerializeField] private List<PlayerButtonSet> playeButtonSets;
}

[System.Serializable]
public class PlayerButtonSet
{
    public Button Button => button;
    public PlayerData Data => data;

    [SerializeField] private Button button;
    [SerializeField] private PlayerData data;
}