using FlysEngine.Sprites;
using Lottery2019.UI.Sprites;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            FallDown, FlashLight, InOrder, ReverseOrder, InOrder, ReverseOrder, InOrder, ReverseOrder
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
                Groups[0][i].Status = ReadBit(buffer, i);
            for (var i = 0; i < _length; ++i)
                Groups[1][^(i + 1)].Status = ReadBit(buffer, i) == 1 ? 2 : 0;
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

        static IEnumerable<ulong> InOrder(int length)
        {
            yield return 0b_0001;
            yield return 0b_0011;
            for (ulong data = 0b_0111; ReadBit(data, length + 1) != 1; data <<= 1)
                yield return data;
        }

        static IEnumerable<ulong> ReverseOrder(int length)
        {
            yield return F(length) - F(length - 1);
            yield return F(length) - F(length - 2);
            for (ulong data = F(length) - F(length - 3); data != 0; data >>= 1)
                yield return data;
        }

        static IEnumerable<ulong> FlashLight(int length)
        {
            const int Times = 6;

            for (var times = 0; times < Times; ++times)
            {
                for (var i = 0; i < 3; ++i)
                    yield return F(length);
                for (var i = 0; i < 3; ++i)
                    yield return 0;
            }
        }

        static IEnumerable<ulong> FallDown(int length)
        {
            var data = 0x1UL;
            for (var i = length; i >= 1; --i)
            {
                for (var b = 1; b <= i; ++b)
                {
                    ResetBit(ref data, b - 1);
                    SetBit(ref data, b);
                    Debug.WriteLine(Convert.ToString((int)data, 2));
                    yield return data;
                }
            }
        }

        // F(8) -> 0b1111_1111
        // F(3) -> 0b0000_0111
        static ulong F(int L) => (1UL << L + 1) - 1;

        static int ReadBit(ulong data, int bit) => (int)((data >> (bit + 1)) & 0x1);

        static void SetBit(ref ulong data, int bit) => data |= 1UL << bit - 1;

        static void ResetBit(ref ulong data, int bit) => data &= ~(1UL << bit - 1);
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
