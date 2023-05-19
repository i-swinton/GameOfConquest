using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Actions {
    public class ColorLerpActions : Action
    {
        Image image;
        Color EndColor;
        Color StartColor;
        public ColorLerpActions(Image image, Color endColor, float dur, float del, ActionType group = ActionType.NoGroup, bool isBlocking = false, EaseType easeType = EaseType.Linear) : base(dur,del,group,isBlocking,easeType)
        {
            this.image = image;

            EndColor = endColor;            
        }
        public override void Start()
        {
            StartColor = image.color;
            base.Start();
        }
        public override bool Execute(float dt)
        {
            image.color = Color.Lerp(StartColor, EndColor, PercentDone);
            return base.Execute(dt);
        }
        public override void Cancel()
        {
            image.color = StartColor;
            base.Cancel();
        }
    }
}