using System.Numerics;

namespace Hashing
{
    static class Opgave5
    {
        // t bestemmer m = 2^t (antal tællere i count-sketch)
        // g er hashfunktionen fra opgave 4

        public static ulong H(ulong x, int t, 
            BigInteger a0, BigInteger a1, BigInteger a2, BigInteger a3)
        {
            // gx = g(x)
            BigInteger gx = Opgave4.G(x, a0, a1, a2, a3);

            // hx = gx & (k-1) hvor k = 2^t
            // dvs. behold de t mindst betydende bits
            BigInteger k = (BigInteger.One << t) - 1;
            ulong hx = (ulong)(gx & k);

            return hx;
        }

        public static int S(ulong x, 
            BigInteger a0, BigInteger a1, BigInteger a2, BigInteger a3)
        {
            // gx = g(x)
            BigInteger gx = Opgave4.G(x, a0, a1, a2, a3);

            // bx = gx >> (b-1) hvor b = 89
            // dvs. den mest betydende bit af gx
            BigInteger bx = gx >> 88;

            // sx = 1 - 2*bx
            // hvis bx = 0 → sx =  1
            // hvis bx = 1 → sx = -1
            int sx = 1 - 2 * (int)bx;

            return sx;
        }
    }
}