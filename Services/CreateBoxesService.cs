using Autodesk.Revit.DB;
using MakeFamilyBoxes.Models;
using System.Diagnostics;
using System.Drawing.Text;

namespace MakeFamilyBoxes.Services
{
    public class CreateBoxesService
    {
        private readonly List<BoxCreator> _boxCreators = [];
        public int counter = 0; 

        public object CreateBoxes
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
            if (RoundBoxEntity == null) RoundBoxEntity = SquareBoxEntity;
            FamilySymbol SquareBox = getFamilyGenericBox.GetFamilySymbolFromEntity(SquareBoxEntity, hubDocumentEntity);
            FamilySymbol RoundBox = getFamilyGenericBox.GetFamilySymbolFromEntity(RoundBoxEntity, hubDocumentEntity);
            foreach (var intersection in intersections)
            {
                _boxCreators.Add(new BoxCreator(intersection, Offset));
            }
            counter = 0;
            using Transaction tx = new(hubDocument, "AutoPlacementBoxes");
            {
                tx.Start();
                if (!SquareBox.IsActive) SquareBox.Activate();
                if (!RoundBox.IsActive) RoundBox.Activate();
                foreach (var boxCreator in _boxCreators)
                {
                    boxCreator.CreateBox(hubDocument, SquareBox, RoundBox, AllBoxesOn);
                    counter++;
                }
                tx.Commit();
            }
            return new object();
        }
    }
}
