using UnityEngine;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Utility
{
    public class LinearCongruentialGenerator
    {
        private const long m = 4294967296; // aka 2^32
        private const long a = 1664525;
        private const long c = 1013904223;
        private long _last;
        public long LastStep { get => _last; }

        public LinearCongruentialGenerator(long stamp)
        {
            _last = stamp;
        }

        public long Next()
        {
            _last = ((a * _last) + c) % m;

            return _last;
        }

        public long Next(long maxValue)
        {
            if (maxValue % 2 == 0)
            {
                return (Next() % (maxValue + 1)) % maxValue;
            }
            return Next() % maxValue;
        }
    }
}