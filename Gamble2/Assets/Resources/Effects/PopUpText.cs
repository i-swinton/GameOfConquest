using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshPro text;
    Actions.ActionList actions;

    public string Text
    {
        get
        {
            return text.text;
        }
        set
        {
            text.text = value;
        }
    }

    private void Awake()
    {
        // Create the action list
        actions = new Actions.ActionList();
    }

    /// <summary>
    /// Initializes the pop up text on spawn with the parameters it needs
    /// </summary>
    /// <param name="duration">The length of time the text is visible</param>
    /// <param name="color">The color of the font</param>
    /// <param name="fontSize">The size of the font</param>
    /// <param name="spawnPosition">The position where the text spawns</param>
    /// <param name="moveDirection">The direction the text moves in.</param>
    /// <param name="speed">The speed at which the text moves</param>
    public void Init(float duration, Color color,int fontSize, Vector3 spawnPosition, Vector3 moveDirection, float speed =1.0f)
    {
        // Push in front of everyhting else
        spawnPosition -= Vector3.forward;

        // Set the font size
        text.fontSize = fontSize;
        Vector3 endPos = spawnPosition + moveDirection.normalized * speed;

        text.color = color;

        // Move in the given direction and fade away
        actions.Add(
            new Actions.Move(endPos, gameObject, duration)
            );

        actions.Add(
            new Actions.Fade(text, 0, duration
            ));
        actions.Add(
            new Actions.Destroy(gameObject, duration));

    }

   
    void Update()
    {
        actions.Update(Time.deltaTime);   
    }
}
