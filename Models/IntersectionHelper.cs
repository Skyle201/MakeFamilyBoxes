using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MakeFamilyBoxes.Models.IntersectionEntity;

namespace MakeFamilyBoxes.Models
{
    public class IntersectionHelper
    {
        public string GetTypeName(Element el, Document engdoc, Document strucdoc)
        {
            ElementId typeId = el.GetTypeId();
            Element typeElement = engdoc.GetElement(typeId) ?? strucdoc.GetElement(typeId);
            return typeElement?.Name ?? "Unknown";
        }
        public (double Width, double Height) GetDuctOrPipeSize(Element el)
        {
            if (el == null || el.Category == null)
                return (0, 0);

            BuiltInCategory category = (BuiltInCategory)el.Category.Id.IntegerValue;

            switch (category)
            {
                case BuiltInCategory.OST_DuctCurves:
                    if (el is not Duct duct || duct.DuctType == null)
                        return (0, 0);

                    var ductShape = duct.DuctType.Shape;
                    if (ductShape == ConnectorProfileType.Round)
                    {
                        double diameter = el.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM)?.AsDouble() ?? 0;
                        return (diameter * 304.8, diameter * 304.8);
                    }
                    else if (ductShape == ConnectorProfileType.Rectangular || ductShape == ConnectorProfileType.Oval)
                    {
                        double width = el.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM)?.AsDouble() ?? 0;
                        double height = el.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM)?.AsDouble() ?? 0;
                        return (width * 304.8, height * 304.8);
                    }
                    break;

                case BuiltInCategory.OST_PipeCurves:
                    if (el is not Pipe pipe || pipe.PipeType == null)
                        return (0, 0);

                    var pipeShape = pipe.PipeType.Shape;
                    if (pipeShape == ConnectorProfileType.Round)
                    {
                        double diameter = el.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)?.AsDouble() ?? 0;
                        return (diameter * 304.8, diameter * 304.8);
                    }
                    break;

                case BuiltInCategory.OST_CableTray:
                    double cableTrayWidth = el.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM)?.AsDouble() ?? 0;
                    double cableTrayHeight = el.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM)?.AsDouble() ?? 0;
                    return (cableTrayWidth * 304.8, cableTrayHeight * 304.8);
            }

            return (0, 0);
        }
        public bool DoesIntersect(Element el1, Element el2)
        {
            BoundingBoxXYZ bbox1 = el1.get_BoundingBox(null);
            BoundingBoxXYZ bbox2 = el2.get_BoundingBox(null);

            if (bbox1 == null || bbox2 == null)
                return false;

            XYZ min1 = bbox1.Min;
            XYZ max1 = bbox1.Max;

            XYZ min2 = bbox2.Min;
            XYZ max2 = bbox2.Max;

            return (min1.X <= max2.X && max1.X >= min2.X) &&
                   (min1.Y <= max2.Y && max1.Y >= min2.Y) &&
                   (min1.Z <= max2.Z && max1.Z >= min2.Z);
        }
        public ShapeEnum GetShape(Element el)
        {
            if (el == null || el.Category == null)
                return ShapeEnum.Rectangular;

            BuiltInCategory category = (BuiltInCategory)el.Category.Id.IntegerValue;

            switch (category)
            {
                case BuiltInCategory.OST_DuctCurves:
                    if (el is Duct duct && duct.DuctType != null)
                    {
                        return duct.DuctType.Shape == ConnectorProfileType.Round ? ShapeEnum.Round : ShapeEnum.Rectangular;
                    }
                    return ShapeEnum.Rectangular;

                case BuiltInCategory.OST_PipeCurves:
                    if (el is Pipe pipe && pipe.PipeType != null)
                    {
                        return pipe.PipeType.Shape == ConnectorProfileType.Round ? ShapeEnum.Round : ShapeEnum.Rectangular;
                    }
                    return ShapeEnum.Round;

                case BuiltInCategory.OST_CableTray:
                    return ShapeEnum.Rectangular;

                default:
                    return ShapeEnum.Rectangular;
            }
        }
        public XYZ GetIntersectionCenter(Element el1, Element el2)
        {
            BoundingBoxXYZ bbox1 = el1.get_BoundingBox(null);
            BoundingBoxXYZ bbox2 = el2.get_BoundingBox(null);

            if (bbox1 == null || bbox2 == null)
                throw new InvalidOperationException("One or both elements do not have a bounding box.");

            double minX = Math.Max(bbox1.Min.X, bbox2.Min.X);
            double minY = Math.Max(bbox1.Min.Y, bbox2.Min.Y);
            double minZ = Math.Max(bbox1.Min.Z, bbox2.Min.Z);

            double maxX = Math.Min(bbox1.Max.X, bbox2.Max.X);
            double maxY = Math.Min(bbox1.Max.Y, bbox2.Max.Y);
            double maxZ = Math.Min(bbox1.Max.Z, bbox2.Max.Z);

            double centerX = (minX + maxX) / 2;
            double centerY = (minY + maxY) / 2;
            double centerZ = (minZ + maxZ) / 2;

            return new XYZ(centerX, centerY, centerZ);
        }

    }
}
