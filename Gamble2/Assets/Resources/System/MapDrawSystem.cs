using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class MapDrawSystem : MonoBehaviour
{
    public GameObject AttackArrowPrefab;

    GameObject arrow;

    static MapDrawSystem instance;
    private void Awake()
    {
        instance = this;
    }

    public static GameObject SpawnArrow(Vector3 startPos, Vector3 endPos)
    {
        // Clear the arrow before creating a new one
        if (instance.arrow) { CancelArrow(); }

        instance.arrow = Instantiate(instance.AttackArrowPrefab, startPos, Quaternion.Euler(-90,0,0));
        instance.arrow.GetComponentInChildren<LaunchArcMesh>().SetEndPosition(endPos);

        return instance.arrow;
    }

    public static void CancelArrow()
    {
        // If the arrow exists, delete it
        if (instance.arrow)
        {
            Destroy(instance.arrow);
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            MapSystem.Board board = BoardManager.instance.GetBoard();
            int roll = RNG.Roll(0, board.Count - 1);
            int roll2 = RNG.Roll(0, board.Count - 1);

            Debug.Log($"Roll1: {roll}, Roll2: {roll2}");
            SpawnArrow(board[roll].Position, board[roll2].Position);
        }
    }
}
