namespace Hashing
{
    class CountSketch
    {
        private long[] _C;      // tæller-array
        private int _t;         // m = 2^t

        // koefficienter til g(x)
        private System.Numerics.BigInteger _a0, _a1, _a2, _a3;

        public CountSketch(int t,
            System.Numerics.BigInteger a0,
            System.Numerics.BigInteger a1,
            System.Numerics.BigInteger a2,
            System.Numerics.BigInteger a3)
        {
            _t  = t;
            _a0 = a0;
            _a1 = a1;
            _a2 = a2;
            _a3 = a3;
            _C  = new long[1 << t]; // m = 2^t tællere, alle 0
        }

        // behandl et element (x, d) fra strømmen
        public void Update(ulong x, int d)
        {
            ulong h = Opgave5.H(x, _t, _a0, _a1, _a2, _a3);
            int   s = Opgave5.S(x, _a0, _a1, _a2, _a3);

            // C[h(x)] += s(x) * d
            _C[h] += s * d;
        }

        // beregn estimatet X = sum af C[y]^2
        public long ComputeEstimate()
        {
            long X = 0;
            foreach (long c in _C)
                X += c * c;
            return X;
        }
    }
}