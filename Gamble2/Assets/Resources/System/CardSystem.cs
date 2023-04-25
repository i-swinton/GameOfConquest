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

    public static bool CanCardIn(List<TerritoryCard> cards)
    {
        // If it is above the cards min, then player can card in
        if(cards.Count >= 5) { return true; }
        
        // If the cards are less than three, then they cannot card in.
        if(cards.Count < 3) { return false; }

        // Search through the colors

        // Count each of the cards
        int infCount = 0; int calvCount = 0; int artCount=0;
        
        for(int i =0; i < cards.Count;  ++i )
        {
            switch(cards[i].CardType)
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
            if(infCount >= 3) { return true; }
            if (calvCount >= 3) { return true; }
            if(artCount >= 3) { return true; }

            // One of each
            if(infCount >0 && calvCount >0 && artCount > 0) { return true; }
        }

        // If we failed to find a card in by this point, the player cannot card-in
        return false;

    }


}
