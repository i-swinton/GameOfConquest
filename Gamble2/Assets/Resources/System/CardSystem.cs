using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardSystem
{
    // The size of the bonus being added to the user
    public static int BonusAmount = 5;

    // How much the bonus increases per card-in
    public static int BonusStepAmount = 5;

    

    public enum CardType
    {
        Infantry,
        Calvary,
        Artillery,
        Wild
    }

    /// <summary>
    /// Initializes the Card System for the game by setting the starting values
    /// </summary>
    /// <param name="startingBonusAmount">The initial amount of bonus units awarded for carding in.</param>
    /// <param name="bonusStepSize">The amount of additional troops gained on the next card in.</param>
    public static void Initialize(int startingBonusAmount = 5, int bonusStepSize = 5)
    {
        BonusAmount = startingBonusAmount;
        BonusStepAmount = bonusStepSize;
    }

    /// <summary>
    /// Checks if the set of cards are elligible for a card in.
    /// </summary>
    /// <param name="cards">The cards which are being checked if they are able to card in.</param>
    /// <returns>If the cards are allowed to card in, returns true. Otherwise, returns false. </returns>
    public static bool CanCardIn(List<TerritoryCard> cards)
    {
        // If it is above the cards min, then player can card in
        if (cards.Count >= 5) { return true; }

        // If the cards are less than three, then they cannot card in.
        if (cards.Count < 3) { return false; }

        // Search through the colors

        // Count each of the cards
        int infCount = 0; int calvCount = 0; int artCount = 0;

        for (int i = 0; i < cards.Count; ++i)
        {
            switch (cards[i].CardType)
            {
                // Increment infantry, calvary, or artillery accordingly
                case CardType.Infantry:
                    {
                        ++infCount;
                        break;
                    }
                case CardType.Calvary:
                    {
                        ++calvCount;
                        break;
                    }

                case CardType.Artillery:
                    {
                        ++artCount;
                        break;
                    }
                // If we have one wild in a hand of three, we are guarenteed to be able to card in.
                case CardType.Wild:
                    {
                        return true;
                    }
            }

            // Check if any of the card-in rules are true

            // Three of a kind
            if (infCount >= 3) { return true; }
            if (calvCount >= 3) { return true; }
            if (artCount >= 3) { return true; }

            // One of each
            if (infCount > 0 && calvCount > 0 && artCount > 0) { return true; }
        }

        // If we failed to find a card in by this point, the player cannot card-in
        return false;

    }

    /// <summary>
    /// Checks if the given hand of cards must perform a card in action.
    /// </summary>
    /// <param name="cards">The set of cards being checked for the need to card-in</param>
    /// <returns>If the user must card in, returns true. Otherwise, returns false.</returns>
    public static bool MustCardIn(List<TerritoryCard> cards)
    {
        // If they can't card in, then they don't have to card in
        if (!CanCardIn(cards)) { return false; }

        // If the user has five cards, they can card in and must do so. 
        if(cards.Count >= 5) { return true; }

        // If none of the above statements are true, then hand does not have to card-in
        return false;
    }


    /// <summary>
    /// Performs the card in action.
    /// </summary>
    /// <param name="cards">The cards being carded in. NOTE: These must be a set of three cards passed in, otherwise, it will throw an error. </param>
    /// <param name="player">The player who is performing the card-in.</param>
    /// <param name="board">The board which the cards are going to be applied to.</param>
    /// <returns>The number of additional troops which the player can deploy.</returns>
    public static int CardIn(List<TerritoryCard> cards,Player player ,MapSystem.Board board)
    {
        if(cards.Count != 3) 
        {
#if UNITY_EDITOR
            throw new CardInOverflow("The improper amount of cards were used for card-in. " + cards.Count + " cards were used. ", cards.Count);
#else
            return 0;
#endif
        }

        // Calculate the units to recieve
        int unitsToRecieve = BonusAmount ;

        // Increment the bonus amount 
        BonusAmount += BonusStepAmount;

        // Check if any of the positions on the board are owned by the player
        foreach(TerritoryCard card in cards)
        {
            // Wild cards do not have territories
            if(card.CardType == CardType.Wild) { continue; }

            // Check if player owns the tile
            if(board[card.TargetTileID].Owner == player)
            {
                // Create new units on that tile to become the new type of units
                board[card.TargetTileID].AddUnits(new Unit(2, card.UnitType));
            }

        }

        // Return the number of unit to recieve
        return unitsToRecieve;
        
    }

    
}

// ---------------------------------------------------- Errors -----------------------------------------------------4
[System.Serializable]
public class CardInOverflow:System.Exception
{
    public int CardInSize { get; }

        public CardInOverflow() { }

        public CardInOverflow(string message)
            : base(message) { }

        public CardInOverflow(string message, System.Exception inner)
            : base(message, inner) { }

    public CardInOverflow(string message, int cardInSize)
    : base(message) 
    {
        CardInSize = cardInSize;
    }
}

