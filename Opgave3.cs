using System;
using System.Collections.Generic;

namespace Hashing
{
    static class Opgave3
    {
        public static long ComputeSumOfSquares(IEnumerable<Tuple<ulong, int>> stream, Func<ulong, ulong> hash, int l)
        {
            var table = new HashTableChaining(hash, l);
            foreach (var item in stream)
                table.Increment(item.Item1, item.Item2);
            long sum = 0;
            foreach (var v in table.AllValues())
                sum += v * v;
            return sum;
        }
    }
}
