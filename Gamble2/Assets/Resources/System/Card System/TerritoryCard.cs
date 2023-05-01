using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct TerritoryCard : INetworkSerializeByMemcpy
{
    // ------------------------------------------ Variables -----------------------------------------
    int unitCount;
    // Unit type if needed
    Unit.Type unitType;
    int targetTile;
    CardSystem.CardType cardType;


    //----------------------------------------- Properties ------------------------------------------------

    public int UnitCount
    {
        get
        {
            return unitCount;
        }
    }

    public Unit.Type UnitType
    {
        get
        {
            return unitType;
        }
    }

    public int TargetTileID
    {
        get
        {
            return targetTile;
        }
    }

    public CardSystem.CardType CardType
    {
        get
        {
            return cardType;
        }
    }

    //-------------------------------------- Public Functions ---------------------------------------


    public TerritoryCard(int unitCount, int targetTile,CardSystem.CardType cardType, Unit.Type type = Unit.Type.Standard)
    {
        this.unitCount = unitCount;
        this.targetTile = targetTile;
        unitType = type;
        this.cardType = cardType;
    }

    public TerritoryCard(int unitCount, int targetTile,  Unit.Type type = Unit.Type.Standard)
    {
        this.unitCount = unitCount;
        this.targetTile = targetTile;
        unitType = type;
        this.cardType = (CardSystem.CardType)RNG.Roll(0,3);
    }



}
