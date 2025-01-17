using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MakeFamilyBoxes.Services
{
    public class SelectByIdService
    {
        public void SelectElementInCurrentDocument(GetRevitDocuments getRevitDocuments, string Id)
    {
            int id = 0;
            try
            {
                id = Convert.ToInt32(Id);
            }
            catch
            {
                MessageBox.Show("Введите цифровой ID");
            }
            UIDocument uidoc = getRevitDocuments.uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;
            ElementId elementId = new(id);
            Element element = doc.GetElement(elementId);
        if (element != null)
        {
            uidoc.Selection.SetElementIds([elementId]);
            TaskDialog.Show("Элемент найден", $"Элемент: {element.Name}\nКатегория: {element.Category?.Name ?? "Без категории"}");
        }
        else
        {
            TaskDialog.Show("Ошибка", "Элемент с указанным ID не найден в текущем документе.");
        }
    }

}
}
