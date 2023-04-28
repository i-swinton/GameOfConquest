using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Actions
{
    public enum ActionType
    {
        NoGroup,
        UI,
        Tiles,
        AllGroup
    }

    public enum EaseType
    {
        Linear,
        FastIn,
        FastOut,
    }

    public abstract class Action
    {
        // ----------------------------------------- Variables ----------------------------------------------
        protected float duration;
        protected float delay;

        float timeElapsed;

        bool isFirstUpdate = true;


        // The unscaled percent done value
        float unscaledPercentDone;

        protected ActionType group;

        protected EaseType easing;

        bool isBlocking;

        // -------------------------------------- Properties ---------------------------------------------

        /// <summary>
        /// The percent finished the action is. This value is adjusted by easing. 
        /// </summary>
        public float PercentDone
        {
            get
            {
                return Ease(unscaledPercentDone);
            }
        }

        /// <summary>
        /// The unscaled finished percent done. This is not effected by easing.
        /// </summary>
        public float UnscaledPercentDone
        {
            get
            {
                return unscaledPercentDone;
            }
        }

        public ActionType Group
        {
            get
            {
                return group;
            }
        }


        /// <summary>
        /// Does this action block other other actions within its group?
        /// </summary>
        public bool IsBlocking
        {
            get
            {
                return isBlocking;
            }
        }

        /// <summary>
        /// The total duration of the action
        /// </summary>
        public float Duration
        {
            get
            {
                return duration;
            }
        }

        /// <summary>
        /// The total time elapsed on the action
        /// </summary>
        public float TimeElapsed
        {
            get
            {
                return timeElapsed;
            }
        }

        /// <summary>
        /// How much time is left for this action before completing
        /// </summary>
        public float TimeLeft
        {
            get
            {
                return duration - timeElapsed;
            }
        }

        /// <summary>
        /// Is this action currently delayed?
        /// </summary>
        public bool IsInDelay
        {
            get
            {
                return delay > 0;
            }
        }

        // -------------------------------------- Public Functions ----------------------------------------
        
        /// <summary>
        /// Creates an action
        /// </summary>
        /// <param name="dur">The length of time the action lasts.</param>
        /// <param name="del">The delay in time before the action begins.</param>
        /// <param name="group">The group in which the action is within</param>
        /// <param name="isBlocking">Can this action block other actions in its group?</param>
        /// <param name="easeType">The type of smoothing which this action utilizes</param>
        public Action(float dur, float del, ActionType group,bool isBlocking, EaseType easeType)
        {
            duration = dur;
            delay = del;
            this.group = group;
            this.isBlocking = isBlocking;
            easing = easeType;
        }

        float Ease(float value)
        {
            switch(easing)
            {
                case EaseType.FastIn:
                    {

                        break;
                    }
                case EaseType.FastOut:
                    {

                        break;
                    }
            }

            // Linear scaling
            return value;
        }

        public virtual void Cancel()
        {

        }

        /// <summary>
        /// Tells the action to perform the opposite of itself instead. Not all actions can reverse
        /// </summary>
        public virtual void Reverse()
        {
            duration = timeElapsed;
            timeElapsed = 0;
            unscaledPercentDone = 0;
        }

        public virtual bool IncrementTime(float dt)
        {
            // If there is a delay, don't push forward
            if(delay > 0)
            {
                delay -= dt;

                return false;
            }


            timeElapsed += dt;

            // Update the percent done
            unscaledPercentDone = timeElapsed / duration;

            return true;
        }

        public bool Update(float dt)
        {
            // If increment time is incomplete, don't update
            if(!IncrementTime(dt))
            {
                return false;
            }    

            // If this is the first update, perform the start updates
            if(isFirstUpdate)
            {
                Start();
                isFirstUpdate = false;
            }

            // If the task is completed, perform the clean up tasks
            if(Execute(dt))
            {
                End();
                // We have completed our action
                return true;
            }

            // If we make it here, we are not yet finished
            return false;
        }

        public virtual void Start()
        {

        }

        /// <summary>
        /// Performs the actions necessary to complete the actions
        /// </summary>
        /// <param name="dt">The time step being passed into the action</param>
        /// <returns>Returns true when the action is complete</returns>
        public virtual bool Execute(float dt)
        {
            return PercentDone >= 1.0f;
        }

        public virtual void End()
        {

        }


        /// <summary>
        /// Syncs up the current action finish when another action would.
        /// </summary>
        /// <param name="other">The other action being synced up to.</param>
        public virtual void Sync(Action other)
        {
            // If it has started updating
            if(other.isFirstUpdate)
            {
                duration = timeElapsed + other.TimeLeft + other.delay;
            }
            else
            {
                delay = other.delay;
                duration = other.duration - other.timeElapsed;
            }
        }
    }

    public class Move : Action
    {
        public Vector3 targetPosition;
        public Vector3 startingPosition;

        bool useCurrentPos;

        public GameObject target;

        public Move(Vector3 targetPos, GameObject target, float dur, 
            float delay = 0.0f, ActionType group = ActionType.NoGroup, 
            bool isBlock = false, EaseType easeType = EaseType.Linear)
            : base(dur, delay,group, isBlock, easeType)
        {
            // Use the target's current position
            useCurrentPos = true;

            targetPosition = targetPos;
            this.target = target;
        }

        public Move(Vector3 targetPos, Vector3 startPos, GameObject target, float dur,
            float delay = 0.0f, ActionType group = ActionType.NoGroup,
            bool isBlock = false, EaseType easeType = EaseType.Linear)
    : base(dur, delay, group, isBlock, easeType)
        {
            targetPosition = targetPos;
            this.target = target;
            startingPosition = startPos;

            useCurrentPos = false;
            
        }

        public override void Start()
        {
            base.Start();

            // Grab the current pos if applicable
            if (useCurrentPos)
            {
                startingPosition = target.transform.position;
            }
        }

        public override bool Execute(float dt)
        {
            // Lerp to the end position
            target.transform.position = Vector3.Lerp(startingPosition, targetPosition, PercentDone);

            return base.Execute(dt);
        }

    

        public override void Reverse()
        {
            base.Reverse();

            // Swap start and end
            Vector3 temp = startingPosition;
            startingPosition = targetPosition;
            targetPosition = temp;
        }

    }
    
    public class Scale : Action
    {
        public Vector3 targetScale;
        public Vector3 startingScale;

        bool useCurrentPos;

        public GameObject target;

        public Scale(Vector3 targetPos, GameObject target, float dur, float delay, ActionType group, bool isBlock, EaseType easeType)
            : base(dur, delay,group, isBlock, easeType)
        {
            // Use the target's current position
            useCurrentPos = true;

            targetScale = targetPos;
            this.target = target;
        }

        public Scale(Vector3 targetPos, Vector3 startPos, GameObject target, float dur, float delay, ActionType group, bool isBlock, EaseType easeType)
            : base(dur, delay, group, isBlock, easeType)
        {
            targetScale = targetPos;
            this.target = target;
            startingScale = startPos;

            useCurrentPos = false;
            
        }

        public override void Start()
        {
            base.Start();

            // Grab the current pos if applicable
            if (useCurrentPos)
            {
                startingScale = target.transform.position;
            }
        }

        public override bool Execute(float dt)
        {
            // Lerp to the end position
            target.transform.localScale = Vector3.Lerp(startingScale, targetScale, PercentDone);

            return base.Execute(dt);
        }

    

        public override void Reverse()
        {
            base.Reverse();

            // Swap start and end
            Vector3 temp = startingScale;
            startingScale = targetScale;
            targetScale = temp;
        }

    }
}

