using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MakeFamilyBoxes.Models
{
    public class BoxCreator
    {
        public double Thickness { get; set; }
        public double Width { get; set; }
        public double Height {  get; set; }
        public Guid Id { get; set; }
        public XYZ CenterCoordinates { get; set; }
        public IntersectionEntity Intersection;

        public FamilySymbol SelectedBoxFamily { get; set; }
        public BoxCreator(IntersectionEntity intersection) 
        { 
            Intersection = intersection;
            Thickness = intersection.Thickness / 304.8;
            Width = intersection.Width / 304.8;
            Height = intersection.Height / 304.8;
            CenterCoordinates = intersection.CenterCoordinates;
            Id = new Guid();
        }

        public void CreateBox(Document HubDocument, FamilySymbol SquareBox, FamilySymbol RoundBox)
        { 
            FilteredElementCollector collector = new(HubDocument);
            collector.OfClass(typeof(FamilySymbol));
            FamilySymbol familySymbol;
            if (Intersection.Shape == IntersectionEntity.ShapeEnum.Round)
            {
                familySymbol = RoundBox;
            }
            else
            {
                familySymbol = SquareBox;
                FamilyInstance instance = HubDocument.Create.NewFamilyInstance(CenterCoordinates, familySymbol, StructuralType.NonStructural);
                if (Intersection.VerticalOrNot)
                {
                    Parameter HeightDim = instance.LookupParameter("Высота");// Высота - по высоте
                    Parameter WidthDim = instance.LookupParameter("Ширина"); // Ширина в семействе - в бока
                    Parameter ThicknessDim = instance.LookupParameter("Толщина"); // толщина в семействе - вглубь 
                    if (HeightDim != null || WidthDim != null || ThicknessDim != null)
                    {
                        HeightDim.Set(Height);
                        WidthDim.Set(Width);
                        ThicknessDim.Set(Thickness);
                    }
                        double angleInRadians = Intersection.WallAngle * (Math.PI / 180);
                        XYZ startPoint = Intersection.CenterCoordinates;
                        XYZ endPoint = Intersection.CenterCoordinates + XYZ.BasisZ;
                        Line rotationAxis = Line.CreateBound(startPoint, endPoint);
                        ElementTransformUtils.RotateElement(HubDocument, instance.Id, rotationAxis, angleInRadians);
                }
                else
                {
                    Parameter HeightDim = instance.LookupParameter("Высота"); // Высота - по высоте
                    Parameter WidthDim = instance.LookupParameter("Ширина");  // Ширина в семействе - в бока
                    Parameter ThicknessDim = instance.LookupParameter("Толщина"); // Толщина в семействе - вглубь 

                    if (HeightDim != null || WidthDim != null || ThicknessDim != null)
                    {
                        HeightDim.Set(Thickness);
                        WidthDim.Set(Height);
                        ThicknessDim.Set(Width);
                    }
                }
            }
            //FamilySymbol familySymbol = collector
            //    .Cast<FamilySymbol>()
            //    .FirstOrDefault(symbol => symbol.Name.Equals("Кубик", StringComparison.OrdinalIgnoreCase));
        }
    }
}
