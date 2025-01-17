using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Models;

namespace MakeFamilyBoxes.Services
{
    public class ManualBoxPlacementService(GetRevitDocuments getRevitDocuments)
    {
        public void ActivateManualBoxPlacement(FamilyEntity familyEntity)
        {
            UIDocument uiDoc = getRevitDocuments.activeUIDoc;
            Document doc = uiDoc.Document;
            DocumentEntity documentEntity = new(doc.Title,123);
            GetFamilyGenericBox getFamilyGenericBox = new(getRevitDocuments);
            FamilySymbol familySymbol = getFamilyGenericBox.GetFamilySymbolFromEntity(familyEntity, documentEntity);
            IList<Reference> references = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, "Выберите элементы");

            }
    }
}
