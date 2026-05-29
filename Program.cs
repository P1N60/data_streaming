using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Hashing
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunOpgave3();
            //RunOpgave4();
            //RunOpgave5();
            //RunOpgave6();
            RunOpgave7();
        }

        static void RunOpgave3()
        {
            int n = 10_000_000;
            int timeoutMs = 30_000;
            Random rnd = new Random(42);

            // Multiply-Shift parameters
            byte[] aBytes = new byte[8];
            rnd.NextBytes(aBytes);
            ulong aShift = BitConverter.ToUInt64(aBytes, 0) | 1UL;

            // Multiply-Mod-Prime parameters
            BigInteger p = (BigInteger.One << 89) - 1;
            byte[] aPrimeBytes = new byte[12];
            byte[] bPrimeBytes = new byte[12];
            rnd.NextBytes(aPrimeBytes);
            rnd.NextBytes(bPrimeBytes);
            BigInteger aPrime = new BigInteger(aPrimeBytes, isUnsigned: true) % p;
            BigInteger bPrime = new BigInteger(bPrimeBytes, isUnsigned: true) % p;

            Console.WriteLine($"Opgave 3 — Sum of squares, n = {n:N0}");
            Console.WriteLine($"{"l",-4} {"2^l",-12} {"MultiplyShift (ms)",-22} {"MultiplyModPrime (ms)",-24} {"S (Shift)",-18} {"S (Prime)",-18}");
            Console.WriteLine(new string('-', 100));

            for (int l = 1; (1L << l) <= n; l++)
            {
                int capturedL = l;

                // Multiply-Shift
                long sShift = 0;
                long msShift = RunWithTimeout(timeoutMs, () =>
                {
                    var stream = StreamGenerator.CreateStream(n, capturedL);
                    sShift = Opgave3.ComputeSumOfSquares(stream,
                        x => Opgave1.MultiplyShift(x, aShift, capturedL), capturedL);
                });

                if (msShift < 0)
                {
                    Console.WriteLine($"{l,-4} {(1L << l),-12:N0} TIMEOUT");
                    break;
                }

                // Multiply-Mod-Prime
                long sPrime = 0;
                long msPrime = RunWithTimeout(timeoutMs, () =>
                {
                    var stream = StreamGenerator.CreateStream(n, capturedL);
                    sPrime = Opgave3.ComputeSumOfSquares(stream,
                        x => Opgave1.MultiplyModPrime(x, aPrime, bPrime, capturedL), capturedL);
                });

                if (msPrime < 0)
                {
                    Console.WriteLine($"{l,-4} {(1L << l),-12:N0} {msShift,-22} TIMEOUT");
                    break;
                }

                Console.WriteLine($"{l,-4} {(1L << l),-12:N0} {msShift,-22} {msPrime,-24} {sShift,-18:N0} {sPrime,-18:N0}");
            }
        }

        static void RunOpgave4()
        {
            Console.WriteLine("\n=== Opgave 4 — 4-universel hashfunktion ===");

            BigInteger a0 = Opgave4.GenererKoefficient();
            BigInteger a1 = Opgave4.GenererKoefficient();
            BigInteger a2 = Opgave4.GenererKoefficient();
            BigInteger a3 = Opgave4.GenererKoefficient();

            ulong[] keys = { 0UL, 1UL, 123456789UL, ulong.MaxValue };

            Console.WriteLine($"{"x",-25} {"g(x)",-30}");
            Console.WriteLine(new string('-', 55));
            foreach (var x in keys)
            {
                BigInteger gx = Opgave4.G(x, a0, a1, a2, a3);
                Console.WriteLine($"{x,-25} {gx,-30}");
            }

            BigInteger P = (BigInteger.One << 89) - 1;
            bool alleGyldig = keys.All(x => Opgave4.G(x, a0, a1, a2, a3) < P);
            Console.WriteLine($"\nAlle værdier i [0, p): {alleGyldig}");
        }

        static void RunOpgave5()
        {
            Console.WriteLine("\n=== Opgave 5 — h og s hashfunktioner ===");

            BigInteger a0 = Opgave4.GenererKoefficient();
            BigInteger a1 = Opgave4.GenererKoefficient();
            BigInteger a2 = Opgave4.GenererKoefficient();
            BigInteger a3 = Opgave4.GenererKoefficient();

            int t = 4;

            ulong[] keys = { 0UL, 1UL, 123456789UL, ulong.MaxValue };

            Console.WriteLine($"{"x",-25} {"h(x)",-10} {"s(x)",-10}");
            Console.WriteLine(new string('-', 45));
            foreach (var x in keys)
            {
                ulong hx = Opgave5.H(x, t, a0, a1, a2, a3);
                int sx   = Opgave5.S(x, a0, a1, a2, a3);
                Console.WriteLine($"{x,-25} {hx,-10} {sx,-10}");
            }

            bool hGyldig = keys.All(x => Opgave5.H(x, t, a0, a1, a2, a3) < (ulong)(1 << t));
            bool sGyldig = keys.All(x => Opgave5.S(x, a0, a1, a2, a3) == 1 ||
                                         Opgave5.S(x, a0, a1, a2, a3) == -1);
            Console.WriteLine($"\nAlle h(x) i [0, {1 << t}): {hGyldig}");
            Console.WriteLine($"Alle s(x) er 1 eller -1:   {sGyldig}");
        }

        static void RunOpgave7()
        {
            // l = 14 svarer til 2^14 = 16384 forskellige nøgler.
            // Sæt l til den l-1 du fandt som grænsen i Opgave 3.
            // t = 8 giver m = 256 tællere.
            Opgave7.Run(l: 14, t: 8, n: 10_000_000);
        }

        static void RunOpgave6()
        {
            Console.WriteLine("\n=== Opgave 6 — Count-Sketch ===");

            int n = 1_000_000;  // strømlængde
            int l = 10;         // 2^10 = 1024 forskellige nøgler
            int t = 8;          // m = 2^8 = 256 tællere

            // beregn eksakt S med hashing fra del 1
            Random rnd = new Random(42);
            byte[] aBytes = new byte[8];
            rnd.NextBytes(aBytes);
            ulong aShift = BitConverter.ToUInt64(aBytes, 0) | 1UL;

            var streamS = StreamGenerator.CreateStream(n, l);
            long exactS = Opgave3.ComputeSumOfSquares(streamS,
                x => Opgave1.MultiplyShift(x, aShift, l), l);
            Console.WriteLine($"Eksakt S = {exactS:N0}");

            // kør count-sketch og beregn estimat X
            BigInteger a0 = Opgave4.GenererKoefficient();
            BigInteger a1 = Opgave4.GenererKoefficient();
            BigInteger a2 = Opgave4.GenererKoefficient();
            BigInteger a3 = Opgave4.GenererKoefficient();

            var sketch = new CountSketch(t, a0, a1, a2, a3);

            // processér strømmen
            foreach (var par in StreamGenerator.CreateStream(n, l))
                sketch.Update(par.Item1, par.Item2);

            long X = sketch.ComputeEstimate();
            Console.WriteLine($"Count-Sketch estimat X = {X:N0}");
            Console.WriteLine($"Forskel: {Math.Abs(exactS - X):N0}");
            Console.WriteLine($"Relativ fejl: {Math.Abs(exactS - X) / (double)exactS:P2}");
        }

        static long RunWithTimeout(int timeoutMs, Action action)
        {
            var sw = Stopwatch.StartNew();
            var task = Task.Run(action);
            if (!task.Wait(timeoutMs))
                return -1;
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}