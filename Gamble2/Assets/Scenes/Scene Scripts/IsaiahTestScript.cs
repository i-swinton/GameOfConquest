using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsaiahTestScript : MonoBehaviour
{
    Actions.ActionList actions;

    public Transform targetPos;

    public PopUpText textPrefab;

    // Start is called before the first frame update
    void Start()
    {
        actions = new Actions.ActionList();

        //actions.Add(new Actions.Move(targetPos.position, gameObject, 10.0f, 0.0f, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear));

    }

    void SpawnText()
    {
        MapSystem.Board board = BoardManager.instance.GetBoard();
        var pos = board[RNG.Roll(0, board.Count - 1)].Position;

        PopUpText text = Instantiate(textPrefab, pos, Quaternion.identity);
        text.Init(1.0f, Color.red, 30, pos, Vector2.up, 5.0f);
    }
    // Update is called once per frame
    void Update()
    {
        actions.Update(Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.K))
        {
            SpawnText();
        }
    }
}
