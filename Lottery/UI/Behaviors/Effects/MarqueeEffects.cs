using System.Collections.Generic;
using static Lottery.UI.Behaviors.Effects.BitExtensions;

namespace Lottery.UI.Behaviors.Effects
{
    public static class MarqueeEffects
    {
        public static IEnumerable<ulong> InOrder(int length)
        {
            yield return 0b_0000;
            yield return 0b_0001;
            yield return 0b_0011;
            for (ulong data = 0b_0111; data.ReadBit(length + 2) != 1; data <<= 1)
                yield return data;
        }

        public static IEnumerable<ulong> ReverseOrder(int length)
        {
            yield return 0;
            yield return FullBit(length) - FullBit(length - 1);
            yield return FullBit(length) - FullBit(length - 2);
            for (ulong data = FullBit(length) - FullBit(length - 3); data != 0; data >>= 1)
                yield return data;
        }

        public static IEnumerable<ulong> FlashLight(int length)
        {
            for (var i = 0; i < 4; ++i)
                yield return FullBit(length);
            for (var i = 0; i < 4; ++i)
                yield return 0;
        }

        public static IEnumerable<ulong> FallDown(int length)
        {
            ulong data = 0x1UL;
            for (var i = length; i >= 0; --i)
            {
                for (var b = 0; b <= i; ++b)
                {
                    data.ResetBit(b - 1);
                    data.SetBit(b);
                    yield return data;
                }
            }
        }

        public static IEnumerable<ulong> OddEvenFlash(int length)
        {
            ulong oddBit = 0;
            ulong evenBit = 0;
            for (var i = 0; i < length; i += 2)
                evenBit.SetBit(i);
            for (var i = 1; i < length; i += 2)
                oddBit.SetBit(i);

            for (var i = 0; i < 4; ++i)
                yield return oddBit;
            for (var i = 0; i < 4; ++i)
                yield return evenBit;
        }

        public static IEnumerable<ulong> All(int length)
        {
            for (var i = 0; i < 3; ++i)
            {
                foreach (var item in InOrder(length)) yield return item;
                foreach (var item in ReverseOrder(length)) yield return item;
            }

            for (var i = 0; i < 3; ++i)
                foreach (var item in FlashLight(length)) yield return item;

            foreach (var item in FallDown(length)) yield return item;

            for (var i = 0; i < 3; ++i)
                foreach (var item in OddEvenFlash(length)) yield return item;
        }
    }

    public static class BitExtensions
    {
        public static int ReadBit(this ulong data, int bit) => (int)((data >> bit) & 0x1);

        public static void SetBit(this ref ulong data, int bit) => data |= 1UL << bit - 1;

        public static void ResetBit(this ref ulong data, int bit) => data &= ~(1UL << bit - 1);

        public static ulong FullBit(int L) => (1UL << L) - 1;
    }
}
