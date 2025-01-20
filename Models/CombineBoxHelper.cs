using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Models
{
    public class CombineBoxHelper
    {
        public (XYZ minPoint, XYZ maxPoint) GetExtremeBoundingBoxPoints(List<Element> elements)
        {
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
            foreach (var element in elements)
            {
                BoundingBoxXYZ bbox = element.get_BoundingBox(null);
                if (bbox == null)
                    continue;
                minX = Math.Min(minX, bbox.Min.X);
                minY = Math.Min(minY, bbox.Min.Y);
                minZ = Math.Min(minZ, bbox.Min.Z);
                maxX = Math.Max(maxX, bbox.Max.X);
                maxY = Math.Max(maxY, bbox.Max.Y);
                maxZ = Math.Max(maxZ, bbox.Max.Z);
            }
            XYZ minPoint = new XYZ(minX, minY, minZ);
            XYZ maxPoint = new XYZ(maxX, maxY, maxZ);
            return (minPoint, maxPoint);
        }

        public XYZ GetCenter(XYZ minPoint, XYZ maxPoint)
        {
            double centerX = (minPoint.X + maxPoint.X) / 2;
            double centerY = (minPoint.Y + maxPoint.Y) / 2;
            double centerZ = (minPoint.Z + maxPoint.Z) / 2;
            return new XYZ(centerX, centerY, centerZ);
        }

        public (double width, double height, double thickness) GetDimensions(XYZ minPoint, XYZ maxPoint)
        {
            double width = maxPoint.X - minPoint.X;
            double height = maxPoint.Y - minPoint.Y;
            double thickness = maxPoint.Z - minPoint.Z;
            return (width, height, thickness);
        }
        public (double width, double height, double thickness) GetDimensionsWithAngle(XYZ minPoint, XYZ maxPoint)
        {
            double width = Math.Sqrt(Math.Pow(maxPoint.X - minPoint.X, 2) + Math.Pow(maxPoint.Y - minPoint.Y, 2));
            double height = Math.Sqrt(Math.Pow(maxPoint.Y - minPoint.Y, 2) + Math.Pow(maxPoint.Z - minPoint.Z, 2));
            double thickness = Math.Sqrt(Math.Pow(maxPoint.X - minPoint.X, 2) + Math.Pow(maxPoint.Z - minPoint.Z, 2));
            return (width, height, thickness);
        }

        public IntersectionEntity GetIntersection(List<Element> elements, double angle)
        {
            var (minPoint, maxPoint) = GetExtremeBoundingBoxPoints(elements);
            XYZ center = GetCenter(minPoint, maxPoint);

            var (width, height, thickness) = GetDimensions(minPoint, maxPoint);
            if (Math.Abs(angle) > 1)
            {
                var (rotatedWidth, rotatedHeight, rotatedThickness) = GetDimensionsWithAngle(minPoint, maxPoint);
                width = rotatedWidth;
                height = rotatedHeight;
                thickness = rotatedThickness;
            }
            return new IntersectionEntity()
            {
                CenterCoordinates = center,
                Width = height * 304.8,
                Height = width * 304.8,
                Thickness = thickness * 304.8,
                FromProject = elements.FirstOrDefault().LookupParameter("Из проекта").AsString(),
                ToProject = elements.FirstOrDefault().LookupParameter("В проект").AsString(),
            };
        }
        public double GetRotationAngle(Element element)
        {
            if (element is FamilyInstance familyInstance)
            {
                Transform transform = familyInstance.GetTransform();
                XYZ xAxis = transform.BasisX;
                double angleInRadians = Math.Atan2(xAxis.Y, xAxis.X);
                double angleInDegrees = angleInRadians * (180 / Math.PI);
                return angleInDegrees;
            }

            return 0;
        }
    }
}
