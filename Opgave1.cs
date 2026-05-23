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
    }
}
