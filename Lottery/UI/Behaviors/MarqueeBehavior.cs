using FlysEngine.Sprites;
using Lottery2019.UI.Sprites;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lottery2019.UI.Behaviors
{
    public class MarqueeGroupBehavior : Behavior
    {
        public List<MarqueeBehavior> Group1 { get; set; }
        public List<MarqueeBehavior> Group2 { get; set; }

        public override void Update(float dt)
        {
            EnsureGroup();

            foreach (var item in Group1) item.LightStatus = LightStatus.Yellow;
            foreach (var item in Group2) item.LightStatus = LightStatus.Blue;
        }

        private void EnsureGroup()
        {
            if (Group1 != null) return;

            var marquees = Sprite.Window.Sprites
                .QueryBehaviorAll<MarqueeBehavior>();

            Group1 = new List<MarqueeBehavior>();
            Group2 = new List<MarqueeBehavior>();
            
            foreach (MarqueeBehavior marquee in marquees)
            {
                if (marquee.GroupId == 1) Group1.Add(marquee);
                else if (marquee.GroupId == 2) Group2.Add(marquee);
                else throw new IndexOutOfRangeException($"Unknown Group: {marquee.GroupId}.");
            }
        }
    }

    public class MarqueeBehavior : Behavior
    {
        public int GroupId { get; set; }

        public int Status { get; set; }

        public LightStatus LightStatus { get => (LightStatus)Status; set => Status = (int)value; }

        public override void Update(float dt)
        {
            base.Update(dt);

            Sprite.FrameId = MathUtil.Clamp(Status, 0, Sprite.Frames.Length - 1);
        }
    }

    public enum LightStatus
    {
        Off, 
        Yellow, 
        Blue, 
    }
}
