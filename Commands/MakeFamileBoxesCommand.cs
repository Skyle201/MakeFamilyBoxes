﻿using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.ViewModels;
using MakeFamilyBoxes.Views;

namespace MakeFamilyBoxes.Commands
{

    [Transaction(TransactionMode.Manual)]
    public class MakeFamileBoxesCommand : IExternalCommand
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
            var viewModel = new MakeFamilyBoxesViewModel();
            var view = new MakeFamilyBoxesView(viewModel);
            view.ShowDialog();
            return Result.Succeeded;
        }
    }
}