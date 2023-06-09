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

    [SerializeField]
    protected bool blockingUI = false;


    static bool isBlocking = false;


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

    public static bool IsBlocking
    {
        get
        {
            return isBlocking;
        }
    }

    /// <summary>
    /// Hides the piece of UI
    /// </summary>
    public virtual void Hide()
    {
        hideElement.SetActive(false);
        isVisible = false;

        if(blockingUI)
        {
            isBlocking = false;
        }
    }

    /// <summary>
    /// Shows the piece of UI
    /// </summary>
    public virtual void Show()
    {
        hideElement.SetActive(true);
        isVisible = true;

        if(blockingUI)
        {
            isBlocking = true;
        }
    }

   
}

namespace Actions
{
    public class UIHide : Action
    {
        UIElement target;

        public UIHide(UIElement element, 
            float duration, float delay = 0, ActionType group = ActionType.NoGroup, bool isBlocking = false, EaseType easeType = EaseType.Linear) :
            base(duration,delay,group,isBlocking, easeType)
        {
            target = element;
        }

        public override void End()
        {
            target.Hide();
            base.End();
        }
    }

    public class UIShow : Action
    {
        UIElement target;

        public UIShow(UIElement element,
            float duration, float delay = 0, ActionType group = ActionType.NoGroup, bool isBlocking = false, EaseType easeType = EaseType.Linear) :
            base(duration, delay, group, isBlocking, easeType)
        {
            target = element;
        }

        public override void End()
        {
            target.Show();
            base.End();
        }
    }

}