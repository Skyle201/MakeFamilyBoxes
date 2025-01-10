using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;

namespace MakeFamilyBoxes.Services
{
    public class GetRevitDocuments(MakeFamilyBoxesCommand makeFamilyBoxesCommand)
    {
        public MakeFamilyBoxesCommand makeFamilyBoxesCommand = makeFamilyBoxesCommand;
        public List<DocumentEntity> documentEntities = [];
        public DocumentSet DocSet = new();

        public List<DocumentEntity> GetRevitDocs()
        {
            DocSet = makeFamilyBoxesCommand.Docs;
            foreach (Document doc in DocSet)
            {
                if (!doc.IsFamilyDocument)
                {
                    documentEntities.Add(new DocumentEntity(doc.Title, doc.GetHashCode()));
                }
            }
            return documentEntities;
        }
        public Document GetDocumentFromEntity(DocumentEntity documentEntity)
        {
            DocSet = makeFamilyBoxesCommand.Docs;
            foreach (Document doc in DocSet)
            {
                if (doc.Title == documentEntity.Title) return doc;
            }
            return null;
        }
    }
}
