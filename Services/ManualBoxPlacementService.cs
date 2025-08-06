using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Models.Entities;
using MakeFamilyBoxes.Models.Helpers;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Media;

namespace MakeFamilyBoxes.Services
{
    public class ManualBoxPlacementService()
    {
        public Element GetElementFromLinkedDocument(Document hostDocument, Reference reference)
        {
            Element element = hostDocument.GetElement(reference);
            if (element is RevitLinkInstance linkInstance)
            {
                Document linkedDocument = linkInstance.GetLinkDocument();
                var linkedElementId = reference.LinkedElementId;

                if (linkedElementId != ElementId.InvalidElementId)
                {
                    return linkedDocument.GetElement(linkedElementId);
                }
            }

            return null;
        }
        public Document GetLinkedDocument(Document hostDocument, Reference reference)
        {
            Element element = hostDocument.GetElement(reference);

            if (element is RevitLinkInstance linkInstance)
            {
                Document linkedDocument = linkInstance.GetLinkDocument();
                return linkedDocument;
            }
            return null;
        }
        public List<IntersectionEntity> ActivateManualBoxPlacement(GetRevitDocuments getRevitDocuments, string minSizeOfSquareBox, string minSizeOfRoundBox)
        {
            UIDocument uiDoc = getRevitDocuments.activeUIDoc;
            Document doc = uiDoc.Document;
            IList<Reference> references = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, "Выберите элементы");

            List<Element> ducts = [];
            List<Element> pipes = [];
            List<Element> cableTrays = [];
            List<Element> walls = [];
            List<Element> floors = [];
            List<Document> StructureDocs = [];
            List<Document> EngineerDocs = [];

            foreach (var reference in references)
            {
                Element element = GetElementFromLinkedDocument(doc,reference);

                if (element == null)
                    continue;

                Category category = element.Category;
                if (category == null)
                    continue;
                Document docum = GetLinkedDocument(getRevitDocuments.activeUIDoc.Document, reference);
                switch (category.Id.IntegerValue)
                {
                    case (int)BuiltInCategory.OST_DuctCurves:
                        ducts.Add(element);
                        if(!EngineerDocs.Contains(docum)) EngineerDocs.Add(docum);
                        break;

                    case (int)BuiltInCategory.OST_PipeCurves:
                        pipes.Add(element);
                        if (!EngineerDocs.Contains(docum)) EngineerDocs.Add(docum); 
                        break;

                    case (int)BuiltInCategory.OST_CableTray:
                        cableTrays.Add(element);
                        if (!EngineerDocs.Contains(docum)) EngineerDocs.Add(docum);
                        break;

                    case (int)BuiltInCategory.OST_Walls:
                        walls.Add(element);
                        if (!StructureDocs.Contains(docum)) StructureDocs.Add(docum);
                        break;

                    case (int)BuiltInCategory.OST_Floors:
                        floors.Add(element);
                        if (!StructureDocs.Contains(docum)) StructureDocs.Add(docum);
                        break;

                }
            }
            IntersectionHelper helper = new();
            double MinSizeOfSquareBox = 0;
            double MinSizeOfRoundBox = 0;
            try
            {
                MinSizeOfSquareBox = Convert.ToDouble(minSizeOfSquareBox);
            }
            catch (Exception) { }
            try
            {
                MinSizeOfRoundBox = Convert.ToDouble(minSizeOfRoundBox);
            }
            catch (Exception) { }
            List<IntersectionEntity> Intersections = helper.FindIntersection(EngineerDocs, StructureDocs, ducts, pipes, cableTrays, walls, floors, MinSizeOfSquareBox, MinSizeOfRoundBox);
            return Intersections;
        }
    }
}
