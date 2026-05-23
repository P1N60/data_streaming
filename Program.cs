using System;
using System.Diagnostics;
using System.Numerics;

namespace Hashing
{
    class Program
    {
        static void Main(string[] args)
        {
            TestHashingPerformance(1000000, 16);
            TestHashingPerformance(10000000, 20);
        }

        static void TestHashingPerformance(int n, int l)
        {
            Console.WriteLine($"\nRunning test with n = {n}, l = {l} (Number of distinct keys = 2^{l})");

            // Generate random parameters for Multiply-Shift
            Random rnd = new Random();
            byte[] aShiftBytes = new byte[8];
            rnd.NextBytes(aShiftBytes);
            ulong aShift = BitConverter.ToUInt64(aShiftBytes, 0) | 1UL; // Ensure it's odd

            // Generate random parameters for Multiply-Mod-Prime
            byte[] aPrimeBytes = new byte[12]; // ~96 bits, will mask down to 89
            byte[] bPrimeBytes = new byte[12];
            rnd.NextBytes(aPrimeBytes);
            rnd.NextBytes(bPrimeBytes);
            
            BigInteger p = (BigInteger.One << 89) - 1;
            BigInteger aPrime = new BigInteger(aPrimeBytes, isUnsigned: true) % p;
            BigInteger bPrime = new BigInteger(bPrimeBytes, isUnsigned: true) % p;
            
            // Generate stream
            var stream = StreamGenerator.CreateStream(n, l);

            // Test Multiply-Shift
            ulong sumShift = 0;
            Stopwatch swShift = Stopwatch.StartNew();
            foreach (var item in stream)
            {
                sumShift += Opgave1.MultiplyShift(item.Item1, aShift, l);
            }
            swShift.Stop();
            Console.WriteLine($"Multiply-Shift: Time = {swShift.ElapsedMilliseconds} ms, Sum = {sumShift}");

            // Regenerate stream
            stream = StreamGenerator.CreateStream(n, l);

            // Test Multiply-Mod-Prime
            ulong sumPrime = 0;
            Stopwatch swPrime = Stopwatch.StartNew();
            foreach (var item in stream)
            {
                sumPrime += Opgave1.MultiplyModPrime(item.Item1, aPrime, bPrime, l);
            }
            swPrime.Stop();
            Console.WriteLine($"Multiply-Mod-Prime: Time = {swPrime.ElapsedMilliseconds} ms, Sum = {sumPrime}");
        }
    }
}