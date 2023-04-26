using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{

    public class Destroy : Action
    {
        GameObject target;


        public Destroy(GameObject target, float delay,ActionType group = ActionType.NoGroup,
            bool isBlocking = false, EaseType easeType = EaseType.Linear):base(0.0f, delay, group,isBlocking,easeType)
        {
            this.target = target;
        }

        public override void End()
        {
            // Destroy the target gameObject at the end of the wait
            GameObject.Destroy(target);
            base.End();
        }
    }
}