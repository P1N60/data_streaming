using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Hashing
{
    static class Opgave7
    {
        // l vælges lig den l-1 I fandt som limit i Opgave 3 (her sat til 14 som default).
        // t bestemmer antal tællere m = 2^t i Count-Sketch.
        public static void Run(int l = 14, int t = 8, int n = 10_000_000)
        {
            Console.WriteLine($"\n=== Opgave 7 — Count-Sketch eksperimenter ===");
            Console.WriteLine($"Parametre: n={n:N0}, l={l} (2^l={1 << l} nøgler), t={t} (m=2^t={1 << t} tællere)");

            // --- 1. Beregn eksakt S med hashing med chaining (Opgave 3) ---
            var rnd = new Random(42);
            byte[] aBytes = new byte[8];
            rnd.NextBytes(aBytes);
            ulong aShift = BitConverter.ToUInt64(aBytes, 0) | 1UL;

            Console.Write("Beregner eksakt S med hashing med chaining ... ");
            long exactS = Opgave3.ComputeSumOfSquares(
                StreamGenerator.CreateStream(n, l),
                x => Opgave1.MultiplyShift(x, aShift, l),
                l);
            Console.WriteLine($"S = {exactS:N0}");

            double theoreticalVariance = 2.0 * (double)exactS * (double)exactS / (1 << t);
            Console.WriteLine($"Teoretisk varians 2*S²/m = {theoreticalVariance:E4}");

            // --- 2. Kør 100 Count-Sketch eksperimenter med nye tilfældige bits ---
            const int numExp = 100;
            long[] estimates = new long[numExp];

            Console.WriteLine($"Kører {numExp} eksperimenter ...");
            for (int i = 0; i < numExp; i++)
            {
                // Nye tilfældige koefficienter i hvert eksperiment (nye tilfældige bits)
                BigInteger a0 = Opgave4.GenererKoefficient();
                BigInteger a1 = Opgave4.GenererKoefficient();
                BigInteger a2 = Opgave4.GenererKoefficient();
                BigInteger a3 = Opgave4.GenererKoefficient();

                var sketch = new CountSketch(t, a0, a1, a2, a3);
                foreach (var (key, delta) in StreamGenerator.CreateStream(n, l))
                    sketch.Update(key, delta);

                estimates[i] = sketch.ComputeEstimate();
            }
            Console.WriteLine("Færdig.");

            // --- 3. Sorterede estimater X_(1) <= ... <= X_(100) ---
            long[] sorted = estimates.OrderBy(x => x).ToArray();

            double mse = estimates.Average(x => Math.Pow(x - exactS, 2));
            Console.WriteLine($"\nMSE = sum((X_i - S)^2)/100 = {mse:E4}");
            Console.WriteLine($"MSE / (2*S²/m)            = {mse / theoreticalVariance:F4}  (forventet ≈ 1)");

            // Gem til CSV til plot
            using (var sw = new StreamWriter("opgave7_sorted.csv"))
            {
                sw.WriteLine("rank,X,S");
                for (int i = 0; i < numExp; i++)
                    sw.WriteLine($"{i + 1},{sorted[i]},{exactS}");
            }
            Console.WriteLine("Data til plot 1 gemt i 'opgave7_sorted.csv'");

            // --- 4. Median-of-groups: 9 grupper af 11, X_100 til overs ---
            // Grupper bruger de USORTEREDE estimater X_1,...,X_100
            long[] medians = new long[9];
            for (int g = 0; g < 9; g++)
            {
                long[] group = estimates.Skip(g * 11).Take(11).ToArray();
                Array.Sort(group);
                medians[g] = group[5]; // medianen af 11 er index 5 (0-baseret)
            }

            long[] sortedMedians = medians.OrderBy(m => m).ToArray();

            Console.WriteLine("\nSorterede medianer M_(1) <= ... <= M_(9):");
            for (int i = 0; i < 9; i++)
                Console.WriteLine($"  M_({i + 1}) = {sortedMedians[i]:N0}  (afvigelse fra S: {sortedMedians[i] - exactS:+#;-#;0})");

            using (var sw = new StreamWriter("opgave7_medians.csv"))
            {
                sw.WriteLine("rank,M,S");
                for (int i = 0; i < 9; i++)
                    sw.WriteLine($"{i + 1},{sortedMedians[i]},{exactS}");
            }
            Console.WriteLine("Data til plot 2 gemt i 'opgave7_medians.csv'");
        }
    }
}
