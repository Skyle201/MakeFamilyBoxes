using Autodesk.Revit.DB;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MakeFamilyBoxes.Services
{
    public class CreateBoxesService
    {
        private readonly List<BoxCreator> _boxCreators = [];

        public void CreateBoxes(GetRevitDocuments getRevitDocuments, DocumentEntity hubDocumentEntity, List<IntersectionEntity> intersections,FamilyEntity SquareBoxEntity, FamilyEntity RoundBoxEntity)
        {
            Document hubDocument = getRevitDocuments.GetDocumentFromEntity(hubDocumentEntity);
            GetFamilyGenericBox getFamilyGenericBox = new(getRevitDocuments);
            FamilySymbol SquareBox = getFamilyGenericBox.GetFamilySymbolFromEntity(SquareBoxEntity, hubDocumentEntity);
            FamilySymbol RoundBox = getFamilyGenericBox.GetFamilySymbolFromEntity(RoundBoxEntity, hubDocumentEntity);
            foreach (var intersection in intersections)
            {
                _boxCreators.Add(new BoxCreator(intersection));
            }
            using Transaction tx = new(hubDocument, "AutoPlacementBoxes");
            {
                tx.Start();
                foreach (var boxCreator in _boxCreators)
                {
                    boxCreator.CreateBox(hubDocument, SquareBox, RoundBox);
                }
                tx.Commit();
            }
        }
    }
}
