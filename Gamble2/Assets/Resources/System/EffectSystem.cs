using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    static EffectSystem instance;

    [SerializeField]
    PopUpText textPrefab;

    [SerializeField]
    float defaultDuration = 1.0f;

    [SerializeField]
    Color defaultColor;

    [SerializeField]
    int defaultFontSize;

    [SerializeField]
    Vector3 defaultDirection;

    [SerializeField]
    float defaultSpeed =1;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Creates pop up text at the given position.
    /// </summary>
    /// <param name="pos">The position of the pop up text.</param>
    /// <param name="duration">How long the text lasts</param>
    /// <param name="fontColor">The color of the pop up text</param>
    /// <param name="fontSize">The size of the font.</param>
    /// <param name="direction">The direction of the pop up text</param>
    /// <param name="speed">How fast the pop up text moves</param>
    /// <returns>Returns a reference to the new pop up text</returns>
    public static PopUpText SpawnText(Vector3 pos, float duration, Color fontColor, int fontSize, Vector3 direction, float speed =1)
    {

        PopUpText text = Instantiate(instance.textPrefab, pos, Quaternion.identity);
        text.Init(duration, fontColor, fontSize, pos, direction, speed);

        return text;
    }



    /// <summary>
    /// Creates pop up text at the given position.
    /// </summary>
    /// <param name="pos">The position of the pop up text.</param>
    /// <param name="fontColor">The color of the pop up text</param>
    /// <returns>Returns a reference to the new pop up text</returns>
    public static PopUpText SpawnText(Vector3 pos, Color fontColor)
    {
        return SpawnText(pos, instance.defaultDuration, fontColor, instance.defaultFontSize, instance.defaultDirection, instance.defaultSpeed);
    }
   
}
