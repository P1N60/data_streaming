using System.Numerics;

namespace Hashing
{
    static class Opgave1
    {
        public static ulong MultiplyShift(ulong x, ulong a, int l)
        {
            unchecked
            {
                ulong res = a * x;
                res = res >> (64 - l);
                return res;
            }
        }

        public static ulong MultiplyModPrime(ulong x, BigInteger a, BigInteger b, int l)
        {
            BigInteger p = (BigInteger.One << 89) - 1;

            BigInteger r = (a * x + b);

            BigInteger z = (r>>89) + ( r & p);
            if (z >= p)
            {
                z -= p;
            }

            z = z & ((BigInteger.One << l) - 1);

            return (ulong)z;
        }

    }
}
