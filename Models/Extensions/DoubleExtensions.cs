using Autodesk.Revit.DB.Mechanical;

namespace MakeFamilyBoxes.Models.Extensions
{
    public static class DoubleExtensions
    {
        public static bool AlmostEqualTo(this double a, double b) => Math.Abs(a - b) < 0.000001;
        public static double RoundUpFive(this double a)
        {
            return (double)(Math.Ceiling(a / 5.0) * 5);
        }
    }
}
