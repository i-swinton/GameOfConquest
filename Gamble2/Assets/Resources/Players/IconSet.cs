using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="IconSet", menuName ="IconSet/IconSet")]
public class IconSet : ScriptableObject
{
    [SerializeField] Sprite[] set;

    /// <summary>
    /// Finds the sprite within the given set of icons
    /// </summary>
    /// <param name="index">The index within this set of icons</param>
    /// <returns>Returns the sprite at the given index. </returns>
    public Sprite this[int index]
    {
        get
        {
            if(index >= set.Length)
            {
                throw new System.ArgumentOutOfRangeException("The index " + index + " is larger than the given set of sprites");
            }

            // Return the sprite at that index
            return set[index];
        }
    }
}
