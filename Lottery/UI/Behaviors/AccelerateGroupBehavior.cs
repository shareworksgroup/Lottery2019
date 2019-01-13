using FlysEngine.Sprites;
using Lottery2019.UI.Sprites;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lottery2019.UI.Behaviors
{
    public class AccelerateGroupBehavior : Behavior
    {
        public GroupItemBehavior[][] Groups { get; set; }

        public float Duration { get; set; } = 0.5f;

        private float _dtAll = 0;

        public int CurrentStage { get; set; }

        public override void Update(float dt)
        {
            EnsureGroup();

            _dtAll += dt;
            float stageDuration = Duration / Groups.Length;
            if (_dtAll > stageDuration)
            {
                _dtAll = _dtAll - stageDuration;
                CurrentStage = (CurrentStage + 1) % Groups.Length;
                TurnOffAllExcept(CurrentStage);
            }
        }

        private void TurnOffAllExcept(int stageId)
        {
            const int RowCount = 3;

            foreach (var group in Groups)
                foreach (var item in group) item.IsOn = false;
            for (var i = stageId; i <= stageId + RowCount - 1; ++i)
                foreach (var item in Groups[T(i)]) item.IsOn = true;

            int T(int v) => 
                v < 0              ? v + Groups.Length : 
                v >= Groups.Length ? v % Groups.Length : 
                v;
        }

        private void EnsureGroup()
        {
            if (Groups != null) return;

            var marquees = Sprite.Window.Sprites
                .QueryBehaviorAll<GroupItemBehavior>();

            Groups = marquees
                .GroupBy(x => x.GroupId)
                .OrderBy(x => x.Key)
                .Select(x => x.ToArray())
                .ToArray();
        }
    }

    public class GroupItemBehavior : Behavior
    {
        public int GroupId { get; set; }

        public bool IsOn { get; set; }

        public override void Update(float dt)
        {
            base.Update(dt);

            Sprite.FrameId = IsOn ? 0 : 1;
        }
    }
}
