using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    [Header("Default UI Elements")]
    [Tooltip("This is the pieceof UI which helps in managing hiding ")]
    [SerializeField] protected GameObject hideElement;

    [SerializeField]
    bool isVisible = true;
    /// <summary>
    /// Is the piece of UI currently visible?
    /// </summary>
    public bool IsVisible
    {
        get { return isVisible; }
        set
        {
            if (value) { Show(); }
            else { Hide(); }
        }
    }

    /// <summary>
    /// Hides the piece of UI
    /// </summary>
    public virtual void Hide()
    {
        hideElement.SetActive(false);
        isVisible = false;
    }

    /// <summary>
    /// Shows the piece of UI
    /// </summary>
    public virtual void Show()
    {
        hideElement.SetActive(true);
        isVisible = true;
    }

   
}
