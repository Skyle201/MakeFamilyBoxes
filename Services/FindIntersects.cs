using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Services
{
    public class FindIntersects
    {
        public Result Execute(ExternalCommandData commandData, ElementSet elements)
        {
            // Получение активного документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Параметры для входных данных
            List<Document> linkDocs = []; // Сюда можно вручную добавить ссылки на связанные документы

            // Сбор данных из основного документа
            List<Element> ducts = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .ToElements() as List<Element>;

            List<Element> pipes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .ToElements() as List<Element>;

            List<Element> cableTrays = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_CableTray)
                .WhereElementIsNotElementType()
                .ToElements() as List<Element>;

            // Вспомогательные методы
            bool DoesIntersect(Element el1, Element el2)
            {
                BoundingBoxXYZ bbox1 = el1.get_BoundingBox(null);
                BoundingBoxXYZ bbox2 = el2.get_BoundingBox(null);

                if (bbox1 == null || bbox2 == null)
                    return false;

                XYZ min1 = bbox1.Min;
                XYZ max1 = bbox1.Max;

                XYZ min2 = bbox2.Min;
                XYZ max2 = bbox2.Max;

                return (min1.X <= max2.X && max1.X >= min2.X) &&
                       (min1.Y <= max2.Y && max1.Y >= min2.Y) &&
                       (min1.Z <= max2.Z && max1.Z >= min2.Z);
            }

            string GetTypeName(Element el)
            {
                ElementId typeId = el.GetTypeId();
                Element typeElement = doc.GetElement(typeId);
                return typeElement?.Name ?? "Unknown";
            }

            (int Width, int Height) GetDuctOrPipeSize(Element el)
            {
                try
                {
                    double width = el.LookupParameter("Width").AsDouble() * 304.8;
                    double height = el.LookupParameter("Height").AsDouble() * 304.8;
                    return ((int)width, (int)height);
                }
                catch
                {
                    double diameter = el.LookupParameter("Diameter").AsDouble() * 304.8;
                    return ((int)diameter, (int)diameter);
                }
            }

            // Поиск пересечений
            List<string> results = new List<string>();
            results.Add("DuctType\tWidth\tHeight\tWallType\tInsulation");

            foreach (Document linkDoc in linkDocs)
            {
                List<Element> walls = new FilteredElementCollector(linkDoc)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsNotElementType()
                    .ToElements() as List<Element>;

                foreach (Element duct in ducts)
                {
                    foreach (Element wall in walls)
                    {
                        if (DoesIntersect(duct, wall))
                        {
                            string ductType = GetTypeName(duct);
                            string wallType = GetTypeName(wall);
                            var (width, height) = GetDuctOrPipeSize(duct);
                            string insulation = ((double)duct.LookupParameter("Insulation Thickness")?.AsDouble() * 304.8).ToString("F0");
                            results.Add($"{ductType}\t{width}\t{height}\t{wallType}\t{insulation}");
                        }
                    }
                }

                foreach (Element pipe in pipes)
                {
                    foreach (Element wall in walls)
                    {
                        if (DoesIntersect(pipe, wall))
                        {
                            string pipeType = GetTypeName(pipe);
                            string wallType = GetTypeName(wall);
                            var (width, height) = GetDuctOrPipeSize(pipe);
                            string insulation = ((double)pipe.LookupParameter("Insulation Thickness")?.AsDouble() * 304.8).ToString("F0");
                            results.Add($"{pipeType}\t{width}\t{height}\t{wallType}\t{insulation}");
                        }
                    }
                }

                foreach (Element cableTray in cableTrays)
                {
                    foreach (Element wall in walls)
                    {
                        if (DoesIntersect(cableTray, wall))
                        {
                            string cableTrayType = GetTypeName(cableTray);
                            string wallType = GetTypeName(wall);
                            var (width, height) = GetDuctOrPipeSize(cableTray);
                            results.Add($"{cableTrayType}\t{width}\t{height}\t{wallType}\t0");
                        }
                    }
                }
            }

            // Сохранение данных в текстовый файл
            string filePath = @"C:\Users\t.zaruba\Desktop\HolesTask.txt";
            File.WriteAllLines(filePath, results);
            TaskDialog.Show("Результат", $"Данные успешно записаны в файл: {filePath}");

            return Result.Succeeded;
        }
    }
}
