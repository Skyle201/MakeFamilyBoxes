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
        private List<BoxCreator> _boxCreators;
        public CreateBoxesService()
        {
            _boxCreators = new List<BoxCreator>();
        }

        public void CreateBoxes(GetRevitDocuments getRevitDocuments, DocumentEntity hubDocumentEntity, List<IntersectionEntity> intersections)
        {
            foreach (var intersection in intersections)
            {
                _boxCreators.Add(new BoxCreator(intersection));
            }

            Document hubDocument = getRevitDocuments.GetDocumentFromEntity(hubDocumentEntity);
            using (Transaction tx = new Transaction(hubDocument, "AutoPlacementBoxes"))
            {
                tx.Start();
                foreach (var boxCreator in _boxCreators)
                {
                    boxCreator.CreateBox(hubDocument);
                }
                tx.Commit();
            }
        }
    }
}
