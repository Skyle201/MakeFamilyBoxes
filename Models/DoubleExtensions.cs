using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Models
{
    public static class DoubleExtensions
    {
        public static bool AlmostEqualTo(this double a, double b) => Math.Abs(a - b) < 0.000001;
    }
}
