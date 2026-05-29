using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Hashing
{
    static class Opgave8
    {
        public static void Run(int l = 14, int n = 10_000_000)
        {
            Console.WriteLine($"\n=== Opgave 8 — Betydning af m for estimatkvalitet og køretid ===");
            Console.WriteLine($"Parametre: n={n:N0}, l={l} (2^l={1 << l} nøgler)");

            // --- 1. Beregn eksakt S med timing ---
            var rnd = new Random(42);
            byte[] aBytes = new byte[8];
            rnd.NextBytes(aBytes);
            ulong aShift = BitConverter.ToUInt64(aBytes, 0) | 1UL;

            Console.Write("Beregner eksakt S med hashing med chaining ... ");
            var swExact = Stopwatch.StartNew();
            long exactS = Opgave3.ComputeSumOfSquares(
                StreamGenerator.CreateStream(n, l),
                x => Opgave1.MultiplyShift(x, aShift, l),
                l);
            swExact.Stop();
            long exactMs = swExact.ElapsedMilliseconds;
            Console.WriteLine($"S = {exactS:N0}  (tid: {exactMs} ms)");

            // --- 2. Kør 100 eksperimenter for hvert t (m = 2^t) ---
            int[] tValues = { 4, 8, 12 };
            const int numExp = 100;

            long[] sketchTotalMs = new long[tValues.Length];

            for (int ti = 0; ti < tValues.Length; ti++)
            {
                int t = tValues[ti];
                int m = 1 << t;
                double theoreticalVariance = 2.0 * (double)exactS * (double)exactS / m;

                Console.WriteLine($"\n--- m = 2^{t} = {m} tællere ---");
                Console.WriteLine($"Kører {numExp} eksperimenter ...");

                long[] estimates = new long[numExp];
                var swSketch = Stopwatch.StartNew();

                for (int i = 0; i < numExp; i++)
                {
                    BigInteger a0 = Opgave4.GenererKoefficient();
                    BigInteger a1 = Opgave4.GenererKoefficient();
                    BigInteger a2 = Opgave4.GenererKoefficient();
                    BigInteger a3 = Opgave4.GenererKoefficient();

                    var sketch = new CountSketch(t, a0, a1, a2, a3);
                    foreach (var (key, delta) in StreamGenerator.CreateStream(n, l))
                        sketch.Update(key, delta);

                    estimates[i] = sketch.ComputeEstimate();
                }

                swSketch.Stop();
                sketchTotalMs[ti] = swSketch.ElapsedMilliseconds;

                double mse = estimates.Average(x => Math.Pow(x - exactS, 2));
                Console.WriteLine($"Tid total (100 eks.): {sketchTotalMs[ti]} ms  |  per eks.: {sketchTotalMs[ti] / numExp} ms");
                Console.WriteLine($"MSE              = {mse:E4}");
                Console.WriteLine($"2*S²/m           = {theoreticalVariance:E4}");
                Console.WriteLine($"MSE / (2*S²/m)   = {mse / theoreticalVariance:F4}  (forventet ≈ 1)");

                // Sorterede estimater → CSV
                long[] sorted = estimates.OrderBy(x => x).ToArray();
                using (var sw = new StreamWriter($"opgave8_sorted_t{t}.csv"))
                {
                    sw.WriteLine("rank,X,S");
                    for (int i = 0; i < numExp; i++)
                        sw.WriteLine($"{i + 1},{sorted[i]},{exactS}");
                }

                // Median-of-groups (9 grupper af 11, X_100 til overs)
                long[] medians = new long[9];
                for (int g = 0; g < 9; g++)
                {
                    long[] group = estimates.Skip(g * 11).Take(11).ToArray();
                    Array.Sort(group);
                    medians[g] = group[5];
                }
                long[] sortedMedians = medians.OrderBy(x => x).ToArray();

                using (var sw = new StreamWriter($"opgave8_medians_t{t}.csv"))
                {
                    sw.WriteLine("rank,M,S");
                    for (int i = 0; i < 9; i++)
                        sw.WriteLine($"{i + 1},{sortedMedians[i]},{exactS}");
                }

                Console.WriteLine($"Data gemt i 'opgave8_sorted_t{t}.csv' og 'opgave8_medians_t{t}.csv'");
            }

            // --- 3. Køretidssammenligning ---
            Console.WriteLine("\n--- Køretidssammenligning ---");
            Console.WriteLine($"{"Metode",-35} {"Tid (ms)",-12} {"Tid per eks. (ms)",-20}");
            Console.WriteLine(new string('-', 67));
            Console.WriteLine($"{"Hashing m. chaining (eksakt S)",-35} {exactMs,-12} {exactMs,-20}");
            for (int ti = 0; ti < tValues.Length; ti++)
            {
                int t = tValues[ti];
                int m = 1 << t;
                string label = $"Count-Sketch m=2^{t}={m} (100 eks.)";
                Console.WriteLine($"{label,-35} {sketchTotalMs[ti],-12} {sketchTotalMs[ti] / numExp,-20}");
            }

            // Gem køretider til CSV til plot
            using (var sw = new StreamWriter("opgave8_runtime.csv"))
            {
                sw.WriteLine("metode,ms_total,ms_per_exp");
                sw.WriteLine($"Chaining,{exactMs},{exactMs}");
                for (int ti = 0; ti < tValues.Length; ti++)
                    sw.WriteLine($"CS m=2^{tValues[ti]},{sketchTotalMs[ti]},{sketchTotalMs[ti] / numExp}");
            }
            Console.WriteLine("Køretider gemt i 'opgave8_runtime.csv'");
        }
    }
}
