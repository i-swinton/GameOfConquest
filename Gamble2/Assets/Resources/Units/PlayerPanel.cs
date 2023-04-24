using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    private Player _player;
    public TextMeshPro text;

    // Update is called once per frame
    void Update()
    {
        text.text = _player.draftTroop.ToString();
    }

    public void Setup(Player player)
    {
        _player = player;
        GetComponent<SpriteRenderer>().color = _player.playerColor;
    }
}
