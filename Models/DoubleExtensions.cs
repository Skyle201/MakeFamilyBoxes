namespace MakeFamilyBoxes.Models
{
    public static class DoubleExtensions
    {
        public static bool AlmostEqualTo(this double a, double b) => Math.Abs(a - b) < 0.000001;
    }
}
