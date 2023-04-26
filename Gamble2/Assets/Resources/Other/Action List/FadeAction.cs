using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Actions
{
    public class Fade : Actions.Action
    {
        enum FadeTargetType
        {
            SpriteRender,
            TextMesh,
            TextMeshUI,
            Image
        }

        FadeTargetType targetType;
        SpriteRenderer targetSR;
        TMPro.TextMeshPro targetTMP;
        TMPro.TextMeshProUGUI targetTMPUI;
        Image targetImage;

        bool useStartingValue;
        float initialValue;
        float endValue;

        //-------------------------------------------------------- Public Functions --------------------------------------------------------
        #region Constructors

        public Fade(SpriteRenderer target,
            float startAlpha, float endAlpha,
            float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
            bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetSR = target;
            targetType = FadeTargetType.SpriteRender;
            initialValue = startAlpha;
            endValue = endAlpha;
        }

        public Fade(SpriteRenderer target,
         float endAlpha,
        float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
        bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetSR = target;
            targetType = FadeTargetType.SpriteRender;
            useStartingValue = true;
            endValue = endAlpha;
        }

        public Fade(TMPro.TextMeshPro target,
            float startAlpha, float endAlpha,
            float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
            bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetTMP = target;
            targetType = FadeTargetType.TextMesh;
            initialValue = startAlpha;
            endValue = endAlpha;
        }

        public Fade(TMPro.TextMeshPro target,
         float endAlpha,
        float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
        bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetTMP = target;
            targetType = FadeTargetType.TextMesh;
            useStartingValue = true;
            endValue = endAlpha;
        }

        public Fade(TMPro.TextMeshProUGUI target,
        float startAlpha, float endAlpha,
        float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
        bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetTMPUI = target;
            targetType = FadeTargetType.TextMeshUI;
            initialValue = startAlpha;
            endValue = endAlpha;
        }

        public Fade(TMPro.TextMeshProUGUI target,
         float endAlpha,
        float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
        bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetTMPUI = target;
            targetType = FadeTargetType.TextMeshUI;
            useStartingValue = true;
            endValue = endAlpha;
        }


        public Fade(Image target,
    float startAlpha, float endAlpha,
    float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
    bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetImage = target;
            targetType = FadeTargetType.Image;
            initialValue = startAlpha;
            endValue = endAlpha;
        }

        public Fade(Image target,
         float endAlpha,
        float dur, float delay = 0, Actions.ActionType group = Actions.ActionType.NoGroup,
        bool isBlocking = false, Actions.EaseType easeType = Actions.EaseType.Linear) : base(dur, delay, group, isBlocking, easeType)
        {
            targetImage = target;
            targetType = FadeTargetType.Image;
            useStartingValue = true;
            endValue = endAlpha;
        }

        #endregion


        public override void Start()
        {
            // If we are using the starting value, get the starting alpha
            if (useStartingValue)
            {
                initialValue = GetStartAlpha();
            }
            base.Start();
        }

        public override bool Execute(float dt)
        {
            // Lerp the alpha
            float newAlpha = Mathf.Lerp(initialValue, endValue, PercentDone);

            // Update the alpha on the object
            LerpAlpha(newAlpha);

            return base.Execute(dt);
        }

        //------------------------------------------- Private Functions -------------------------------------------------------------------

        /// <summary>
        /// Gets the starting alpha from the target object
        /// </summary>
        /// <returns>Returns the value of the alpha as a float</returns>
        private float GetStartAlpha()
        {
            float alpha = 0.0f;

            switch (targetType)
            {
                case FadeTargetType.Image:
                    {
                        // Get the alpha of the image
                        alpha = targetImage.color.a;

                        break;
                    }
                case FadeTargetType.TextMesh:
                    {
                        alpha = targetTMP.alpha;
                        break;
                    }
                case FadeTargetType.TextMeshUI:
                    {
                        alpha = targetTMPUI.alpha;
                        break;
                    }
                case FadeTargetType.SpriteRender:
                    {
                        alpha = targetSR.color.a;
                        break;
                    }
            }

            return alpha;
        }

        private void LerpAlpha(float newAlpha)
        {
            switch (targetType)
            {
                case FadeTargetType.Image:
                    {
                        // Get the color of the image
                        Color clone = targetImage.color;
                        // Change the alpha
                        clone.a = newAlpha;
                        // Replace the original color
                        targetImage.color = clone;

                        break;
                    }
                case FadeTargetType.TextMesh:
                    {
                        targetTMP.alpha = newAlpha;
                        break;
                    }
                case FadeTargetType.TextMeshUI:
                    {
                        targetTMPUI.alpha = newAlpha;
                        break;
                    }
                case FadeTargetType.SpriteRender:
                    {
                        // Get the color of the SR
                        Color clone = targetSR.color;
                        // Change the alpha
                        clone.a = newAlpha;
                        // Replace the original color
                        targetSR.color = clone;
                        break;
                    }
            }
        }

    }
}