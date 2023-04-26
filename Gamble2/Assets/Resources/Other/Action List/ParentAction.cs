using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class Parent : Action
    {
        GameObject target;
        GameObject newParent;

        /// <summary>
        /// Creates an action which will parent itself to another gameObject.
        /// </summary>
        /// <param name="target">The object being parented to another.</param>
        /// <param name="newParent">The object which will be the parent of the target.</param>
        /// <param name="delay">The time we wait before the parenting occurs.</param>
        /// <param name="group">The group of actions which this action is a part of.</param>
        /// <param name="isBlock">Does this action block actions of its group behind it?</param>
        /// <param name="easeType">The type easing used by this action. </param>
        public Parent(GameObject target, GameObject newParent, float delay,
            ActionType group = ActionType.NoGroup,
            bool isBlock = false, EaseType easeType = EaseType.Linear) : base(0.0f,delay,group, isBlock, easeType)
        {
            this.target = target;
            this.newParent = newParent;
        }

        public override void End()
        {
            // Set the new parent
            target.transform.SetParent(newParent.transform);

            base.End();
        }
    }
}
