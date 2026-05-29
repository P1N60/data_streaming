using System.Numerics;

namespace Hashing
{
    static class Opgave4
    {
        private static readonly BigInteger P = (BigInteger.One << 89) - 1;

        // Evaluer g(x) = a0 + a1*x + a2*x^2 + a3*x^3 mod p
        // ved brug af Horners metode
        public static BigInteger G(ulong x, BigInteger a0, BigInteger a1, BigInteger a2, BigInteger a3)
        {
            BigInteger X = (BigInteger)x;

            // Horner: a0 + x*(a1 + x*(a2 + x*a3))
            BigInteger result = a3;
            result = ModP(result * X + a2);
            result = ModP(result * X + a1);
            result = ModP(result * X + a0);

            return result;
        }

        // Samme ModP trick som i opgave 1b
        public static BigInteger ModP(BigInteger y)
        {
            while (y >= P)
            {
                BigInteger low  = y & P;
                BigInteger high = y >> 89;
                y = low + high;
            }
            return y;
        }

        // Generer tilfældig koefficient i [0, p)
        public static BigInteger GenererKoefficient()
        {
            Random rnd = new Random();
            while (true)
            {
                byte[] bytes = new byte[12];
                rnd.NextBytes(bytes);
                bytes[11] &= 0x1F;
                BigInteger val = new BigInteger(bytes, isUnsigned: true);
                if (val < P) return val;
            }
        }
    }
}