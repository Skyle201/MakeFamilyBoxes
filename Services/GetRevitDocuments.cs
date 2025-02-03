using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;

namespace MakeFamilyBoxes.Services
{
    public class GetRevitDocuments(MakeFamilyBoxesCommand makeFamilyBoxesCommand)
    {
        public MakeFamilyBoxesCommand makeFamilyBoxesCommand = makeFamilyBoxesCommand;
        public List<DocumentEntity> documentEntities = [];
        public List<Document> DocSet = [];
        public UIDocument activeUIDoc = makeFamilyBoxesCommand.uiDoc;
        public UIApplication uiApp = makeFamilyBoxesCommand.uiApp;
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
            if (documentEntity.Title == string.Empty || documentEntity == null) return null;    
            DocSet = makeFamilyBoxesCommand.Docs;
            foreach (Document doc in DocSet)
            {
                if (doc.Title == documentEntity.Title) return doc;
            }
            return null;
        }
    }
}
