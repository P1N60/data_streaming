using System.Numerics;

namespace Hashing
{
    class Program
    {
        static void Main(string[] args)
        {
            ulong x = 123456789UL;
            int l = 20;

            ulong msA = 13845678901234567891UL;
            ulong msResult = Opgave1.MultiplyShift(x, msA, l);
            Console.WriteLine($"MultiplyShift: h({x}) = {msResult}");

            BigInteger p = (BigInteger.One << 89) - 1;
            BigInteger mmpA = p - 1;
            BigInteger mmpB = 42;
            ulong mmpResult = Opgave1.MultiplyModPrime(x, mmpA, mmpB, l);
            Console.WriteLine($"MultiplyModPrime: h({x}) = {mmpResult}");
            Console.WriteLine($"Burde være 275220");
        }
    }
}