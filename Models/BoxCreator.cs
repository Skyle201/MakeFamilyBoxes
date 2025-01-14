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
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height {  get; set; }
        public string Id { get; set; }
        public XYZ CenterCoordinates { get; set; }
        public IntersectionEntity Intersection;

        public FamilySymbol SelectedBoxFamily { get; set; }
        public BoxCreator(IntersectionEntity intersection) 
        { 
            Intersection = intersection;
            CenterCoordinates = intersection.CenterCoordinates;
        }

        public void CreateBox(Document HubDocument)
        {
            FilteredElementCollector collector = new(HubDocument);
            collector.OfClass(typeof(FamilySymbol));

            FamilySymbol familySymbol = collector
                .Cast<FamilySymbol>()
                .FirstOrDefault(symbol => symbol.Name.Equals("Кубик", StringComparison.OrdinalIgnoreCase));

            HubDocument.Create.NewFamilyInstance(CenterCoordinates, familySymbol, StructuralType.NonStructural);
        }
    }
}
