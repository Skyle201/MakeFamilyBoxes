using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using MakeFamilyBoxes.Models.Entities;

namespace MakeFamilyBoxes.Models.Extensions
{
    public class IntersectionEntityComparer : IEqualityComparer<IntersectionEntity>
    {
        public bool Equals(IntersectionEntity x, IntersectionEntity y)
        {
            if (x == null || y == null)
                return false;

            return x.Height.AlmostEqualTo(y.Height) &&
                   x.Width.AlmostEqualTo(y.Width) &&
                   x.Thickness.AlmostEqualTo(y.Thickness) &&
                   x.CenterCoordinates.X.AlmostEqualTo(y.CenterCoordinates.X) &&
                   x.CenterCoordinates.Y.AlmostEqualTo(y.CenterCoordinates.Y) &&
                   x.CenterCoordinates.Z.AlmostEqualTo(y.CenterCoordinates.Z);
        }

        public int GetHashCode(IntersectionEntity intersection)
        {
            return intersection.Height.GetHashCode() + intersection.Width.GetHashCode() + intersection.Thickness.GetHashCode() + intersection.CenterCoordinates.X.GetHashCode() + intersection.CenterCoordinates.Y.GetHashCode() + intersection.CenterCoordinates.Z.GetHashCode();
        }
        public bool DoubleEqual(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= 1;
        }
    }
}
