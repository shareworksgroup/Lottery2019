using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lottery.UI.Behaviors.Effects.BitExtensions;

namespace Lottery.UI.Behaviors.Effects
{
    public static class MarqueeEffects
    {
        public static IEnumerable<ulong> InOrder(int length)
        {
            yield return 0b_0001;
            yield return 0b_0011;
            for (ulong data = 0b_0111; data.ReadBit(length + 1) != 1; data <<= 1)
                yield return data;
        }

        public static IEnumerable<ulong> ReverseOrder(int length)
        {
            yield return FullBit(length) - FullBit(length - 1);
            yield return FullBit(length) - FullBit(length - 2);
            for (ulong data = FullBit(length) - FullBit(length - 3); data != 0; data >>= 1)
                yield return data;
        }

        public static IEnumerable<ulong> Around3(int length)
        {
            for (var i = 0; i < 3; ++i)
            {
                foreach (var item in InOrder(length)) yield return item;
                foreach (var item in ReverseOrder(length)) yield return item;
            }
        }

        public static IEnumerable<ulong> FlashLight(int length)
        {
            const int Times = 6;

            for (var times = 0; times < Times; ++times)
            {
                for (var i = 0; i < 3; ++i)
                    yield return FullBit(length);
                for (var i = 0; i < 3; ++i)
                    yield return 0;
            }
        }

        public static IEnumerable<ulong> FallDown(int length)
        {
            ulong data = 0x1UL;
            for (var i = length; i >= 1; --i)
            {
                for (var b = 1; b <= i; ++b)
                {
                    data.ResetBit(b - 1);
                    data.SetBit(b);
                    yield return data;
                }
            }
        }
    }

    public static class BitExtensions
    {
        public static int ReadBit(this ulong data, int bit) => (int)((data >> (bit + 1)) & 0x1);

        public static void SetBit(this ref ulong data, int bit) => data |= 1UL << bit - 1;

        public static void ResetBit(this ref ulong data, int bit) => data &= ~(1UL << bit - 1);

        public static ulong FullBit(int L) => (1UL << L + 1) - 1;
    }
}
