using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardInUI : UIElement
{
    [Header("Card-In UI")]
    // The positions used for anchoring cards that are being traded in.
    [SerializeField] Transform[] cardInAnchors;


    [SerializeField] UIDisplayCard cardPrefab;

    public List<UIDisplayCard> cards = new List<UIDisplayCard>();

    [Header("Drag and Drop")]
    [SerializeField] float dropThreshold;

    MapSystem.Board board;

    // Start is called before the first frame update
    void Start()
    {
        
        // Grab the board
        board = BoardManager.instance.GetBoard();   

        // Make card

    }

    public void MakeCard(MapSystem.Board b)
    {
        TerritoryCard card = new(0, RNG.Roll(0, b.Count - 1));

        cards.Add(Instantiate(cardPrefab, transform));
        cards[cards.Count - 1].Initialize(card, b[0].Owner, b);

        cards[cards.Count - 1].GetComponentInChildren<DragUI>().OnDragEnd += OnCardDragEnd;
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
            MakeCard(board);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            IsVisible = !IsVisible;
        }
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
