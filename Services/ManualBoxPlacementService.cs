using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
            }

                uiDoc.PromptForFamilyInstancePlacement(familySymbol);

            }
    }
}
