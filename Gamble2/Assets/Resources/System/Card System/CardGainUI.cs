using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGainUI : UIElement
{
    [Header("Prefabs")]
    [SerializeField] UIDisplayCard cardPrefab;
    [Header("References")]
    [SerializeField] Transform targetEndPos;
    [SerializeField] Transform targetStarPos;

    static CardGainUI instance;
    Actions.ActionList actions;

    List<UIDisplayCard> cardList;
    Player target;
    
    [SerializeField]
    float spawnOffset;

    public static TerritoryCard RandomCard()
    {
        return new(0, RNG.Roll(0, BoardManager.instance.GetBoard().Count - 1));
    }

    public static void GainCards(List<TerritoryCard> cards, Player target, MapSystem.Board board)
    {
        foreach(var card in cards)
        {
            UIDisplayCard newCard = Instantiate(instance.cardPrefab, instance.targetStarPos.position, Quaternion.identity);
            instance.cardList.Add(newCard);
            newCard.Initialize(card, target, board);
        }
        instance.IsVisible = true;

        instance.target = target;

        instance.MoveCards();
    }
    public static void GainCards(TerritoryCard card, Player target, MapSystem.Board board)
    {

        UIDisplayCard newCard = Instantiate(instance.cardPrefab, instance.targetStarPos.position, Quaternion.identity,instance.hideElement.transform);
        instance.cardList.Add(newCard);
        newCard.Initialize(card, target, board);


        instance.IsVisible = true;

        instance.target = target;

        instance.MoveCards();

    }

    public void MoveCards()
    {
        // 

        float startOffset = -(spawnOffset * cardList.Count/2.0f);
        

        for(int i=0; i < cardList.Count; ++i)
        {

            actions.Add(
    new Actions.Move(targetEndPos.position+Vector3.right*startOffset, cardList[i].gameObject, 0.5f)
    );

            startOffset += spawnOffset;
        }
    }


    private void Awake()
    {
        instance = this;
        actions = new Actions.ActionList();

        cardList = new List<UIDisplayCard>();
    }



   public void AssignCards()
    {
        for (int i = 0; i < cardList.Count; ++i)
        {
            target.cards.Add(cardList[i].Card);
        }

        // Reset their ability to get a card
        target.canGetCard = false;


        IsVisible = false;
        GameMaster.GetInstance().EndTurn();
    }

    // Update is called once per frame
    void Update()
    {
        actions.Update(Time.deltaTime);    
    }

    public override void Hide()
    {
        base.Hide();

        // Clear all of the cards
        while(cardList.Count > 0)
        {
            Destroy(cardList[0].gameObject);
            cardList.RemoveAt(0);
        }
    }
}