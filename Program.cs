namespace Hashing
{
    class Program
    {
        static void Main(string[] args)
        {
            ulong a = 13845678901234567891UL;
            ulong x = 123456789UL;
            int l = 64;

            ulong result = Opgave1.MultiplyShift(x, a, l);
            Console.WriteLine($"h({x}) = {result}");
        }
    }
}