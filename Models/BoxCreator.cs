using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace MakeFamilyBoxes.Models
{
    public class BoxCreator(IntersectionEntity intersection, double Offset)
    {
        public double Thickness { get; set; } = (intersection.Thickness) / 304.8;
        public double Width { get; set; } = (intersection.Width + 2 * Offset) / 304.8;
        public double Height { get; set; } = (intersection.Height + 2 * Offset) / 304.8;
        public XYZ CenterCoordinates { get; set; } = intersection.CenterCoordinates;
        public IntersectionEntity Intersection = intersection;

        public FamilySymbol SelectedBoxFamily { get; set; }

        public void CreateBox(Document HubDocument, FamilySymbol SquareBox, FamilySymbol RoundBox, bool AllBoxesOn)
        { 
            FilteredElementCollector collector = new(HubDocument);
            collector.OfClass(typeof(FamilySymbol));
            FamilySymbol familySymbol;
            if (Intersection.Shape == IntersectionEntity.ShapeEnum.Round && AllBoxesOn == true)
            {
                familySymbol = RoundBox;
                FamilyInstance instance = HubDocument.Create.NewFamilyInstance(CenterCoordinates, familySymbol, StructuralType.NonStructural);
                if (Intersection.VerticalOrNot)
                {
                    Parameter HeightDim = instance.LookupParameter("Высота");// Высота - по высоте
                    Parameter WidthDim = instance.LookupParameter("Ширина"); // Ширина в семействе - в бока
                    Parameter ThicknessDim = instance.LookupParameter("Толщина"); // толщина в семействе - вглубь 
                    Parameter IsVertical = instance.LookupParameter("В перекрытии");
                    Parameter Guid = instance.LookupParameter("ID");
                    Parameter FromProject = instance.LookupParameter("Из проекта");
                    Parameter ToProject = instance.LookupParameter("В проект");
                    if (HeightDim != null || WidthDim != null || ThicknessDim != null)
                    {
                        HeightDim.Set(Thickness);
                        WidthDim.Set(Width);
                        ThicknessDim.Set(Height);
                        try
                        {
                            IsVertical.Set(0);
                            Guid.Set(instance.Id.ToString());
                            FromProject.Set(Intersection.FromProject);
                            ToProject.Set(Intersection.ToProject);
                        }
                        catch (Exception ex) { }

                    }
                    double angleInRadians = (Intersection.WallAngle+90) * (Math.PI / 180);
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
                    Parameter IsVertical = instance.LookupParameter("В перекрытии");
                    Parameter Guid = instance.LookupParameter("ID");
                    Parameter FromProject = instance.LookupParameter("Из проекта");
                    Parameter ToProject = instance.LookupParameter("В проект");

                    if (HeightDim != null || WidthDim != null || ThicknessDim != null)
                    {
                        HeightDim.Set(Thickness);
                        WidthDim.Set(Height);
                        ThicknessDim.Set(Width);
                        IsVertical.Set(1);
                        Guid.Set(instance.Id.ToString());
                        FromProject.Set(Intersection.FromProject);
                        ToProject.Set(Intersection.ToProject); ;
                    }
                }

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
                    Parameter Guid = instance.LookupParameter("ID");
                    Parameter FromProject = instance.LookupParameter("Из проекта");
                    Parameter ToProject = instance.LookupParameter("В проект");
                    if (HeightDim != null || WidthDim != null || ThicknessDim != null)
                    {
                        HeightDim.Set(Height);
                        WidthDim.Set(Width);
                        ThicknessDim.Set(Thickness);
                        Guid.Set(instance.Id.ToString());
                        FromProject.Set(Intersection.FromProject);
                        ToProject.Set(Intersection.ToProject);
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
                    Parameter Id = instance.LookupParameter("ID");
                    Parameter FromProject = instance.LookupParameter("Из проекта");
                    Parameter ToProject = instance.LookupParameter("В проект");

                    if (HeightDim != null || WidthDim != null || ThicknessDim != null)
                    {
                        HeightDim.Set(Thickness);
                        WidthDim.Set(Height);
                        ThicknessDim.Set(Width);
                        Id.Set(instance.Id.ToString());
                        FromProject.Set(Intersection.FromProject);
                        ToProject.Set(Intersection.ToProject);
                    }
                }
            }
        }
    }
}
