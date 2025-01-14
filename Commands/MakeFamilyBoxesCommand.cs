using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Services;
using MakeFamilyBoxes.ViewModels;
using MakeFamilyBoxes.Views;
using System.Windows.Interop;

namespace MakeFamilyBoxes.Commands
{

    [Transaction(TransactionMode.Manual)]
    public class MakeFamilyBoxesCommand : IExternalCommand
    {
        public List<Document> Docs;
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {
            var uiApplication = commandData.Application;
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;
            Docs = [];
            foreach (Document doc in app.Documents)
            {
                if (doc == null) continue;
                Docs.Add(doc);
            }

            if (Docs.Count == 0)
            {
                TaskDialog.Show("No Open Documents", "There are no documents currently open in Revit.");
                return Result.Failed;
            }
            var getRevitDocuments = new GetRevitDocuments(this);
            var getFamilyGenericBox = new GetFamilyGenericBox(this);
            var findIntersectsService = new FindIntersectsService(this);
            var createBoxesService = new CreateBoxesService(this);
            var viewModel = new MakeFamilyBoxesViewModel(getRevitDocuments, getFamilyGenericBox, findIntersectsService, createBoxesService);
            var view = new MakeFamilyBoxesView(viewModel);
            view.ShowDialog();
            return Result.Succeeded;
        }
    }
}