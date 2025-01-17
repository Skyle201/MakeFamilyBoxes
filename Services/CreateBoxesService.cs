using Autodesk.Revit.DB;
using MakeFamilyBoxes.Models;

namespace MakeFamilyBoxes.Services
{
    public class CreateBoxesService
    {
        private readonly List<BoxCreator> _boxCreators = [];

        public void CreateBoxes
            (GetRevitDocuments getRevitDocuments,
            DocumentEntity hubDocumentEntity,
            List<IntersectionEntity> intersections,
            FamilyEntity SquareBoxEntity,
            FamilyEntity RoundBoxEntity,
            string offset)
        {
            double Offset = 0;
            try
            {
                Offset = Convert.ToDouble(offset);

            }

            catch (Exception) { }
            Document hubDocument = getRevitDocuments.GetDocumentFromEntity(hubDocumentEntity);
            GetFamilyGenericBox getFamilyGenericBox = new(getRevitDocuments);
            bool AllBoxesOn = true;
            if (SquareBoxEntity == null || RoundBoxEntity == null)
            {
                if (SquareBoxEntity == null) SquareBoxEntity = RoundBoxEntity;
                else RoundBoxEntity = SquareBoxEntity;
                AllBoxesOn = false;
            }
            FamilySymbol SquareBox = getFamilyGenericBox.GetFamilySymbolFromEntity(SquareBoxEntity, hubDocumentEntity);
            FamilySymbol RoundBox = getFamilyGenericBox.GetFamilySymbolFromEntity(RoundBoxEntity, hubDocumentEntity);
            foreach (var intersection in intersections)
            {
                _boxCreators.Add(new BoxCreator(intersection, Offset));
            }
            using Transaction tx = new(hubDocument, "AutoPlacementBoxes");
            {
                tx.Start();
                foreach (var boxCreator in _boxCreators)
                {
                    boxCreator.CreateBox(hubDocument, SquareBox, RoundBox, AllBoxesOn);
                }
                tx.Commit();
            }
        }
    }
}
