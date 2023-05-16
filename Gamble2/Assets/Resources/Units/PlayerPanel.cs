using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    private Player _player;
    public TextMeshProUGUI text;


    public TextMeshProUGUI troopText;
    public TextMeshProUGUI tileText;
    public TextMeshProUGUI draftText;


    public Image playerFill;

    // Update is called once per frame
    void Update()
    {
        text.text = _player.draftTroop.ToString();
    }

    public void Setup(Player player)
    {
        _player = player;
        playerFill.color = _player.playerColor;

        _player.onTerritoryCountChange += OnTerritoryCountChange;
        _player.onTroopCountChange += OnTroopCountChange;
    }

    public void OnTroopCountChange(int value)
    {
        troopText.text = $"Troops {value}";
    }
    public void OnTerritoryCountChange(int value, int bonus)
    {
        tileText.text = $"Territories: {value}";

        // Find the next calculate amount of units
        draftText.text = $"Next Draft: {GameMaster.GetInstance().GetDraftAmount(_player)}";
    }
}
