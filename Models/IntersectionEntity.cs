using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MakeFamilyBoxes.Models
{
    public class IntersectionEntity
    {
        public XYZ CenterCoordinates { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Insulation { get; set; }
        public string FromProject { get; set; }
        public string ToProject { get; set; }
        public ShapeEnum Shape { get; set; }
        public string EngineerPipeType {  get; set; } 
        public string StructureType { get; set; }
        public bool VerticalOrNot {  get; set; }
        public double WallAngle { get; set; }
        public enum ShapeEnum
        {
            Round,
            Rectangular
        };
        public bool IntersectionCheckDims(IntersectionEntity intersection, double minSizeSquareBox, double minSizeRoundBox)
        {
            if ((intersection.Shape == IntersectionEntity.ShapeEnum.Round) && (intersection.Width < minSizeRoundBox || intersection.Height < minSizeRoundBox))
            {
                return false;
            }
            else if ((intersection.Shape == IntersectionEntity.ShapeEnum.Rectangular) && (intersection.Width < minSizeSquareBox || intersection.Height < minSizeSquareBox))
            {
                return false;
            }
            else return true;
        }
        public IntersectionEntity TryCreateEntity(Element el1, Element el2, Document EngineerDocument, Document StructureDocument)
        {
            try
            {
                IntersectionHelper helper = new();
                if (helper.DoesIntersect(el1, el2))
                {
                    bool verticalornot = false;
                    string ductType = helper.GetTypeName(el1, EngineerDocument, StructureDocument);
                    string wallType = helper.GetTypeName(el2, EngineerDocument, StructureDocument);
                    double thickness = 0;
                    double wallangle = 0;
                    if (el2 is Wall wall)
                    {
                        thickness = wall.Width * 304.8;
                        verticalornot = true;
                        wallangle = helper.GetWallRotationAngle(wall);
                    }
                    else if (el2 is Floor floor)
                    {
                        thickness = floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble() * 304.8;
                    }

                    var (width, height) = helper.GetDuctOrPipeSize(el1);
                    double insulation = 0;
                    if (el1.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS) != null)
                    {
                        insulation = ((double)el1.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS)?.AsDouble() * 304.8);
                    }
                    XYZ center = helper.GetIntersectionCenter(el1, el2);
                    if (center == null) return null;
                    ShapeEnum shape = helper.GetShape(el1);
                    return new IntersectionEntity
                    {
                        CenterCoordinates = center,
                        FromProject = EngineerDocument.Title,
                        ToProject = StructureDocument.Title,
                        Height = height,
                        Width = width,
                        Thickness = thickness,
                        Insulation = insulation,
                        Shape = shape,
                        EngineerPipeType = ductType,
                        StructureType = wallType,
                        VerticalOrNot = verticalornot,
                        WallAngle = wallangle
                    };
                }
                else return null;
            }
            catch (Exception) { return null; }
        }
        }
    }
