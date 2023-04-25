using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardInUI : MonoBehaviour
{
    // The positions used for anchoring cards that are being traded in.
    [SerializeField] Transform[] cardInAnchors;


    [SerializeField] UIDisplayCard cardPrefab;

    public List<UIDisplayCard> cards = new List<UIDisplayCard>();

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
    }
}
