using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Services
{
    public class CombineBoxService
    {
        public object FindBoxToCombine(GetRevitDocuments getRevitDocuments, FamilyEntity SquareBoxEntity)
        {
            UIDocument uIDoc = getRevitDocuments.uiApp.ActiveUIDocument;
            Document doc = uIDoc.Document;
           IList<Reference> references = uIDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выберите элементы");
            var Elements = references.Select(reference => doc.GetElement(reference)).Where(element => element != null).ToList();
            CombineBoxHelper combineBoxHelper = new CombineBoxHelper();
            double AngleInDegrees = combineBoxHelper.GetRotationAngle(Elements[0]);
            IntersectionEntity intersect = combineBoxHelper.GetIntersection(Elements, AngleInDegrees);
            GetFamilyGenericBox getFamilyGenericBox = new(getRevitDocuments);
            FamilySymbol SquareBox = getFamilyGenericBox.GetFamilySymbolFromEntity(SquareBoxEntity, new DocumentEntity(doc.Title,0));
            FamilySymbol RoundBox = getFamilyGenericBox.GetFamilySymbolFromEntity(SquareBoxEntity, new DocumentEntity(doc.Title, 0));
            BoxCreator boxCreator = new(intersect,0);
            using Transaction tx = new(doc, "CombineBoxes");
            {
                tx.Start();
                if (!SquareBox.IsActive) SquareBox.Activate();
                if (!RoundBox.IsActive) RoundBox.Activate();
                    boxCreator.CreateBox(doc, SquareBox, RoundBox, false);
                foreach (var element in Elements)
                {
                    doc.Delete(element.Id);
                }
                tx.Commit();
            }
            return new object();

        }
    }
}
