using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Models
{
    public class Intersection
    {
        public XYZ CenterCoordinates { get; }
        public double Diameter { get; }
        public double Height { get; }
        public double Width { get; }
        public string FromProject { get; }
        public enum Shape
        {
            Round,
            Rectangular
        };

    }
}
