using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{

    public class SetText : Action
    {
        TMPro.TextMeshProUGUI targetUI;
        TMPro.TextMeshPro targetWorld;

        bool isUI;

        string newText;

        //----------------------------------------- Public Functions -----------------------------------

        public SetText(TMPro.TextMeshProUGUI target, string text,
            float delay,ActionType group = ActionType.NoGroup, bool isBlocking =false, EaseType easeType = EaseType.Linear )
            :base(0.0f, delay, group, isBlocking, easeType)
        {
            isUI = true;
            targetUI = target;
            newText = text;
        }
        public SetText(TMPro.TextMeshPro target, string text,
    float delay, ActionType group = ActionType.NoGroup, bool isBlocking = false, EaseType easeType = EaseType.Linear)
    : base(0.0f, delay, group, isBlocking, easeType)
        {
            isUI = false;
            targetWorld = target;
            newText = text;
        }

        public override void End()
        {
            if (isUI) { targetUI.text = newText; }
            else { targetWorld.text = newText; }
            base.End();
        }
    }
}