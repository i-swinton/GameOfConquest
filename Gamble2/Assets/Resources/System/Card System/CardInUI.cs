using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardInUI : UIElement
{
    [Header("Card-In UI")]
    // The positions used for anchoring cards that are being traded in.
    [SerializeField] Transform[] cardInAnchors;

    [SerializeField] Button turnInButton;
    [SerializeField] Button cancelButton;

    // The anchors to the bottom of the screen
    [SerializeField] GameObject bottomAnchor;

    // The array of cards being turned in
    UIDisplayCard[] turnInCards = new UIDisplayCard[3];
    TerritoryCard[] turnInRefs = new TerritoryCard[3];

    [Header("Prefabs")]
    [SerializeField] UIDisplayCard cardPrefab;
    [SerializeField] GameObject emptyPrefab;
    

    public List<UIDisplayCard> cards = new List<UIDisplayCard>();

    List<TerritoryCard> playerCards;

    [Header("Drag and Drop")]
    [SerializeField] float dropThreshold;

    // Reference to the board
    MapSystem.Board board;

    // Can the user card in currently
    bool canCardIn = false;

    public List<GameObject> rootPositions;
    int fillSize = 0;


    // ACTION LIST POWER!!!!
    Actions.ActionList actions;

    Player targetPlayer;
    

    //--------------------------------------------- Properties ----------------------------------------------------------------
    public bool CanCardIn
    {
        get
        {
            return canCardIn;
        }
        set
        {
            canCardIn = value;
            turnInButton.interactable = canCardIn;
        }
    }

    //-------------------------------------------- Public Functions ----------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        // Create the action list
        actions = new Actions.ActionList();

        // Get the root position
        rootPositions = new List<GameObject>();

        // Grab the board
        board = BoardManager.instance.GetBoard();

        // Check if we can card in
        CheckCardIn();

        for (int i = 0; i < 7; ++i)
        {
            rootPositions.Add(Instantiate(emptyPrefab, bottomAnchor.transform));
        }
    }

    public override void Show()
    {
        base.Show();
        if (GameMaster.GetInstance().IsNetworked)
        {
            // Load your own card UI
            LoadCardInUI(GameMaster.GetInstance().GetPlayerAt( ClientPlayerController.LocalPlayer));
        }
        else
        {
            // Load the current player's card UI
            LoadCardInUI(GameMaster.GetInstance().GetPlayer());
        }
    }

    public void LoadCardInUI(Player player)
    {
        // Grab the board if the board is empty
        if (board == null) { board = BoardManager.instance.GetBoard(); }

        // Set the target player
        targetPlayer = player;

        // INSERT READ IN CARDS FROM PLAYER HERE
        playerCards = player.cards;

        // Set the player
        targetPlayer = player;
        if(targetPlayer == null)
        {
            targetPlayer = GameMaster.GetInstance().GetPlayer();
        }

        for (int i = 0; i < playerCards.Count; ++i)
        {
            MakeCard(playerCards[i], player, board);
        } 
    }

    public void MakeCard(TerritoryCard card, Player p, MapSystem.Board b)
    {

        // Bind the cards to the hide element to disappear with it. 
        // Spawn them off screen for movment stuff
        cards.Add(Instantiate(cardPrefab, new Vector3(0, -1000), Quaternion.identity, hideElement.transform));
        cards[cards.Count - 1].Initialize(card, p, b);

        cards[cards.Count - 1].GetComponentInChildren<DragUI>().OnDragEnd += OnCardDragEnd;

        UIDisplayCard uiCard = cards[cards.Count - 1];

        uiCard.name = "Card " + cards.Count;


        AdjustItems();
    }

    public void MakeCard(MapSystem.Board b)
    {
        TerritoryCard card = CardGainUI.RandomCard();

        // Bind the cards to the hide element to disappear with it. 
        // Spawn them off screen for movment stuff
        cards.Add(Instantiate(cardPrefab, new Vector3(0, -1000), Quaternion.identity, hideElement.transform));
        cards[cards.Count - 1].Initialize(card, b[0].Owner, b);

        cards[cards.Count - 1].GetComponentInChildren<DragUI>().OnDragEnd += OnCardDragEnd;

        UIDisplayCard uiCard = cards[cards.Count - 1];

        uiCard.name = "Card " + cards.Count;


        AdjustItems();

        //actions.Add(new Actions.Parent(uiCard.gameObject, rootPositions[rootPositions.Count - 1],0.5f));
    }

    public void OnCardDragEnd(Vector2 position, Transform cardTransform, Vector2 lastPosition)
    {

        float closestDistance = float.MaxValue;
        int index = -1;
        // Adjust 
        for(int i=0; i < cardInAnchors.Length; ++i)
        {
            // Compare distances to see which is the closest
            float dist = Vector2.Distance(position, cardInAnchors[i].position);
            if(dist < closestDistance)
            {
                closestDistance = dist;
                index = i;
            }
        }
        // If it is further than the drop threshold, return to the original spot
        if(closestDistance > dropThreshold)
        {
            cardTransform.position = lastPosition;
            cardTransform.GetComponentInChildren<DragUI>().ForceLastPosition(lastPosition);
            return;
        }



        // Otherwise, we lock it in place
        cardTransform.position = cardInAnchors[index].position;

        // Move into turn in slot
        turnInCards[index] = cardTransform.GetComponent<UIDisplayCard>();
        turnInRefs[index] = turnInCards[index].Card;

        // Check if we can card in
        CheckCardIn();

        AdjustItems();
    }

    

    // Update is called once per frame
    void Update()
    {
        // Keep trying to get the board if unable to
        if(board == null)
        {
            board = BoardManager.instance.GetBoard();
            return;
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            NotifySystem.Message("Spawned a card for player");
            MakeCard(board);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            IsVisible = !IsVisible;
        }

        // Update the action list
        actions.Update(Time.deltaTime);

    }

    public void CheckCardIn()
    {
        // First, check if the turn in refs are correct

        for(int i=0;  i <  turnInRefs.Length; ++i)
        {
            if (turnInCards[i] == null) { CanCardIn = false; return; }
        }


        // Next, see if the turn in refs are a valid list
        CanCardIn = CardSystem.CanCardIn(turnInRefs.ToList());

        // If you can still card in after all the checks, enable the button

    }

    public Vector2 FillRootPosition()
    {
        // If we are over the fill size, reset it
        if (fillSize >= rootPositions.Count)
        {
            fillSize = 0;
        }

        return rootPositions[fillSize].transform.position;
    }

    public void AdjustItems()
    {
        int step = 0;

        var list = turnInCards.ToList();

        for(int i =0; i < cards.Count; ++i )
        {
            // If the cards are contains 
            if(list.Contains(cards[i]))
            {
                continue;
            }

            actions.Add(
                new Actions.Move(
                   rootPositions[step].transform.position,
                   cards[i].gameObject,
                   0.5f
                ));

            // Set the new last position for fixing thing
            cards[i].GetComponentInChildren<DragUI>().ForceLastPosition(rootPositions[step].transform.position);


            //actions.Add(
            //    new Actions.Parent
            //    (
            //        cards[i].gameObject,
            //        rootPositions[step].gameObject, 
            //        0.5f
            //    )
            //    );

            ++step;
        }
    }


    /// <summary>
    /// Hide the the menu and undoes and actions performed during the card in
    /// </summary>
    public void CancelCardIn()
    {
        Hide();

        // Remove all of the cards
        for(int i=0; i <cards.Count; ++i)
        {
            Destroy(cards[i].gameObject);
        }

        // Clear the cards
        cards.Clear();

        // Turn in the card
        for(int i =0; i < turnInCards.Length; ++i)
        {
            turnInCards[i] = null;
        }
    }

    public void PerfomrCardIn()
    {
        // Don't do anything if we still don't have the go ahead.
        if (!CanCardIn) { return; }

        // Insert player for card in here
        int troopCount = CardSystem.CardIn(turnInRefs.ToList(), GameMaster.GetInstance().GetPlayer(), board);

        targetPlayer.draftTroop += troopCount;
        

        NotifySystem.Message($"{board[0].Owner} has recieved {troopCount} units.");


        // Delete the cards
        RemoveCardRefs();
    }

    void RemoveCardRefs()
    {
        // Loop through and remove the turn in cards
        for(int i=0; i < turnInCards.Length; ++i)
        {
            // Remove from total list
            cards.Remove(turnInCards[i]);

            // Delete the turn in cards
            Destroy(turnInCards[i].gameObject);

            // Replace with null
            turnInCards[i] = null;
        }

        CheckCardIn();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach(Transform t in cardInAnchors)
        {
            Gizmos.DrawWireSphere(t.position, dropThreshold);
        }
    }

}
