using FlysEngine.Sprites;
using Lottery.UI.Behaviors.Effects;
using Lottery2019.UI.Sprites;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Lottery.UI.Behaviors.Effects.MarqueeEffects;

namespace Lottery2019.UI.Behaviors
{
    public class MarqueeGroupBehavior : Behavior
    {
        public MarqueeBehavior[][] Groups { get; set; }

        private int _length;

        const float Duration = 1.0f;
        float _dtAll = 0;
        int _effectId = 0;
        private Func<int, IEnumerable<ulong>>[] _effects = new Func<int, IEnumerable<ulong>>[]
        {
            FallDown, FlashLight, Around3
        };
        IEnumerator<ulong> CurrentEffect { get; set; }

        public override void Update(float dt)
        {
            EnsureGroup();
            float stage = Duration / _length;
            _dtAll += dt;
            if (_dtAll > stage)
            {
                _dtAll = _dtAll - stage;
                if (CurrentEffect.MoveNext())
                {
                    Commit(CurrentEffect.Current);
                }
                else
                {
                    SetEffect(_effectId = (_effectId + 1) % _effects.Length);
                }
            }
        }

        void SetEffect(int effectId)
        {
            CurrentEffect = _effects[effectId](_length).GetEnumerator();
        }

        void Commit(ulong buffer)
        {
            for (var i = 0; i < _length; ++i)
                Groups[0][i].Status = buffer.ReadBit(i);
            for (var i = 0; i < _length; ++i)
                Groups[1][^(i + 1)].Status = buffer.ReadBit(i) == 1 ? 2 : 0;
        }

        void EnsureGroup()
        {
            if (Groups != null) return;

            Groups = Sprite.Window.Sprites
                .QueryBehaviorAll<MarqueeBehavior>()
                .GroupBy(x => x.GroupId)
                .OrderBy(x => x.Key)
                .Select(x => x.ToArray())
                .ToArray();

            Debug.Assert(Groups.Length == 2);
            Debug.Assert(Groups[0].Length == Groups[1].Length);

            _length = Groups[0].Length;
            SetEffect(_effectId = 0);
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
