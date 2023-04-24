using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsaiahTestScript : MonoBehaviour
{
    Actions.ActionList actions;

    public Transform targetPos;

    // Start is called before the first frame update
    void Start()
    {
        actions = new Actions.ActionList();

        actions.Add(new Actions.Move(targetPos.position, gameObject, 10.0f, 0.0f, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear));

    }

    // Update is called once per frame
    void Update()
    {
        actions.Update(Time.deltaTime);
    }
}
