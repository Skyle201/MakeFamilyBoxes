using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Services;
using MakeFamilyBoxes.ViewModels;
using MakeFamilyBoxes.Views;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;


namespace MakeFamilyBoxes.Commands
{

    [Autodesk.Revit.Attributes.TransactionAttribute(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class MakeFamilyBoxesCommand : IExternalCommand
    {
        public List<Document> Docs;
        public UIDocument uiDoc;
        public UIApplication uiApp;
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {
            uiApp = commandData.Application;
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
            uiDoc = uiApp.ActiveUIDocument;
            var getRevitDocuments = new GetRevitDocuments(this);
            var viewModel = new MakeFamilyBoxesViewModel(getRevitDocuments);
            var saver = new VMSaver();
            var view = new MakeFamilyBoxesView(viewModel);
            view.Show();
            view.Topmost = true;
            saver.LoadVM(viewModel);
            return Result.Succeeded;
        
        }
    }
}