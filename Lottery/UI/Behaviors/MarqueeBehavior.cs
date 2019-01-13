using FlysEngine.Sprites;
using Lottery2019.UI.Sprites;
using SharpDX;
using System.Diagnostics;
using System.Linq;

namespace Lottery2019.UI.Behaviors
{
    public class MarqueeGroupBehavior : Behavior
    {
        public MarqueeBehavior[][] Groups { get; set; }

        private int _groupItems;

        const float Duration = 0.5f;
        int StageId { get; set; } = 0;
        float _dtAll = 0;
        ulong _buffer = 0;

        public override void Update(float dt)
        {
            EnsureGroup();
            float stage = Duration / _groupItems;
            _dtAll += dt;
            if (_dtAll > stage)
            {
                _dtAll = _dtAll - stage;
                StageId += 1;
                _buffer <<= 1;
                Commit();
            }
        }

        private void Commit()
        {
            for (var i = 0; i < _groupItems; ++i)
                Groups[0][i].Status = ReadAt(_buffer, i);
            for (var i = 0; i < _groupItems; ++i)
                Groups[1][^(i + 1)].Status = ReadAt(_buffer, i);

            if (ReadAt(_buffer, _groupItems) == 1) _buffer = 0x_0001;

            int ReadAt(ulong data, int bit) => (int)((data >> (bit + 1)) & 0x1);
        }

        private void EnsureGroup()
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

            _groupItems = Groups[0].Length;
            _buffer = 0b_0001;
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
