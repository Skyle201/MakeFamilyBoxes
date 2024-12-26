using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Services;
using MakeFamilyBoxes.ViewModels;
using MakeFamilyBoxes.Views;

namespace MakeFamilyBoxes.Commands
{

    [Transaction(TransactionMode.Manual)]
    public class MakeFamilyBoxesCommand : IExternalCommand
    {
        public DocumentSet Docs;
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {
            var uiApplication = commandData.Application;
            //var uiDoc = commandData.Application.ActiveUIDocument;
            //var doc = uiDoc.Document;
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;
            Docs = app.Documents;

            if (Docs.IsEmpty)
            {
                TaskDialog.Show("No Open Documents", "There are no documents currently open in Revit.");
                return Result.Failed;
            }
            var getRevitDocuments = new GetRevitDocuments(this);
            var getFamilyGenericBox = new GetFamilyGenericBox(this);
            var viewModel = new MakeFamilyBoxesViewModel(getRevitDocuments, getFamilyGenericBox);
            var view = new MakeFamilyBoxesView(viewModel);
            view.Show();
            return Result.Succeeded;
        }
    }
}