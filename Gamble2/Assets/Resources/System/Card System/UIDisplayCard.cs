using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDisplayCard : UIElement
{
    //---------------------------------------------------------- Variables -----------------------------------------------------------------
    [Space]
    [Header("Display Card")]


    // The card being referenced by the the display card
    TerritoryCard cardRef;

    [Tooltip("The icon for the type of unit card this is")]
    [SerializeField]Image unitTypeIcon;
    [Tooltip("The icon for the tile this card benefits")]
    [SerializeField]Image tileImage;
    [Tooltip("The textMesh the name of the tile this card effects")]
    [SerializeField]TextMeshProUGUI tileName;

    [SerializeField] IconSet icons;

    //------------------------------------------------------ Properties -----------------------------------------------------------------------
    /// <summary>
    /// The card which the display card is referencing 
    /// </summary>
    public TerritoryCard Card
    {
        get
        {
            return cardRef;
        }
    }


    //--------------------------------------------------------- Public Functions ------------------------------------------------------------
    
    /// <summary>
    /// Initializes the card with data it needs to fix itself
    /// </summary>
    /// <param name="card">The card which this Display card is going to represent.</param>
    /// <param name="player">The player which owns this card.</param>
    /// <param name="board">The board which this card is referencing.</param>
    public void Initialize(TerritoryCard card, Player player, MapSystem.Board board)
    {
        cardRef = card;

       // Debug.Log("Initialize data: " + card.CardType + ", " + board[card.TargetTileID].Name + " at " + card.TargetTileID + ", " + card.UnitType);

        // Get icons from players
        // IconSet icons = player.Icons;

        unitTypeIcon.sprite = icons[(int)card.CardType];

        // If wild, hide the territory
        if (card.CardType == CardSystem.CardType.Wild)
        {
            // Scale away the image
            tileImage.transform.localScale = Vector3.zero;
            tileName.text = "";
            tileName.autoSizeTextContainer = true;
        }
        else
        {
            // Set the image of the tile
            // Get the tile's name
            tileName.text = board[card.TargetTileID].Name;
        }
    }

}
