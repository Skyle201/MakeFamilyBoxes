using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;
using System.IO;
using System.Windows;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using System.Windows.Media.Media3D;
using System.Xaml;
namespace MakeFamilyBoxes.Services
{
    public class FindIntersectsService()
    {
        DocumentEntity StructureDocumentEntity;
        DocumentEntity LinkEngineerDocumentEntity;

        public List<IntersectionEntity> FindIntersects(GetRevitDocuments getRevitDocuments, DocumentEntity linkdoc, DocumentEntity structDoc)
        {
            if (linkdoc == null || structDoc == null) { MessageBox.Show("Не заданы документы"); return null ; }
            StructureDocumentEntity = structDoc;
            LinkEngineerDocumentEntity = linkdoc;
            // Получение документа
            List<Document> EngineerDocs = [getRevitDocuments.GetDocumentFromEntity(LinkEngineerDocumentEntity)];

            // Параметры для входных данных
            List<Document> StructureDocs = [getRevitDocuments.GetDocumentFromEntity(StructureDocumentEntity)]; 
            List<IntersectionEntity> Intersections = [];
            Document EngineerDoc = EngineerDocs[0];
            // Сбор данных о сетях
            
            List<Element> ducts = new FilteredElementCollector(EngineerDoc)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .ToElements() as List<Element>;
            List<Element> pipes = new FilteredElementCollector(EngineerDoc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .ToElements() as List<Element>;

            List<Element> cableTrays = new FilteredElementCollector(EngineerDoc)
                .OfCategory(BuiltInCategory.OST_CableTray)
                .WhereElementIsNotElementType()
                .ToElements() as List<Element>;


            if (ducts.Count == 0 && pipes.Count == 0 && cableTrays.Count == 0) { MessageBox.Show("Не найдено элементов инженерных систем"); return null; };
    
            // Поиск пересечений
            //List<string> results = ["DuctType\tWidth\tHeight\tWallType\tInsulation"];

            foreach (Document EngineerDock in EngineerDocs)
            {
                foreach (Document doc in StructureDocs)
                {
                    List<Element> walls = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .WhereElementIsNotElementType()
                        .ToElements() as List<Element>;

                    List<Element> floors = [.. new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Floors)
                        
                        .ToElements()];

                    //Прогоняемся по элементам и ищем пересечения

                    foreach (Element duct in ducts)
                    {
                        foreach (Element wall in walls)
                        {
                            IntersectionEntity intersection = new();
                            intersection = intersection.TryCreateEntity(duct, wall, EngineerDock, doc);
                            if (intersection != null)
                            {
                                //results.Add($"{intersection.EngineerPipeType}\t{intersection.Width}\t{intersection.Height}\t{intersection.StructureType}\t{intersection.Insulation}\t{intersection.CenterCoordinates}\t{intersection.FromProject}\t{intersection.Thickness}");
                                Intersections.Add(intersection);
                            }
                        }
                        foreach (Element floor in floors)
                        {
                            IntersectionEntity intersection = new();
                            intersection = intersection.TryCreateEntity(duct, floor, EngineerDock, doc);
                            if (intersection != null)
                            {
                                //results.Add($"{intersection.EngineerPipeType}\t{intersection.Width}\t{intersection.Height}\t{intersection.StructureType}\t{intersection.Insulation}\t{intersection.CenterCoordinates}\t{intersection.FromProject}\t{intersection.Thickness}");
                                Intersections.Add(intersection);
                            }
                        }
                    }

                    foreach (Element pipe in pipes)
                    {
                        foreach (Element wall in walls)
                        {
                            IntersectionEntity intersection = new();
                            intersection = intersection.TryCreateEntity(pipe, wall, EngineerDock, doc);
                            if (intersection != null)
                            {
                                //results.Add($"{intersection.EngineerPipeType}\t{intersection.Width}\t{intersection.Height}\t{intersection.StructureType}\t{intersection.Insulation}\t{intersection.CenterCoordinates}\t{intersection.FromProject}\t{intersection.Thickness}");
                                Intersections.Add(intersection);
                            }
                        }
                        foreach (Element floor in floors)
                        {
                            IntersectionEntity intersection = new();
                            intersection = intersection.TryCreateEntity(pipe, floor, EngineerDock, doc);
                            if (intersection != null)
                            {
                                //results.Add($"{intersection.EngineerPipeType}\t{intersection.Width}\t{intersection.Height}\t{intersection.StructureType}\t{intersection.Insulation}\t{intersection.CenterCoordinates}\t{intersection.FromProject}\t{intersection.Thickness}");
                                Intersections.Add(intersection);
                            }
                        }
                    }

                    foreach (Element cableTray in cableTrays)
                    {
                        foreach (Element wall in walls)
                        {
                            IntersectionEntity intersection = new();
                            intersection = intersection.TryCreateEntity(cableTray, wall, EngineerDock, doc);
                            if (intersection != null)
                            {
                                //results.Add($"{intersection.EngineerPipeType}\t{intersection.Width}\t{intersection.Height}\t{intersection.StructureType}\t{intersection.Insulation}\t{intersection.CenterCoordinates}\t{intersection.FromProject}\t{intersection.Thickness}");
                                Intersections.Add(intersection);
                            }
                        }
                        foreach (Element floor in floors)
                        {
                            IntersectionEntity intersection = new();
                            intersection = intersection.TryCreateEntity(cableTray, floor, EngineerDock, doc);
                            if (intersection != null)
                            {
                                //results.Add($"{intersection.EngineerPipeType}\t{intersection.Width}\t{intersection.Height}\t{intersection.StructureType}\t{intersection.Insulation}\t{intersection.CenterCoordinates}\t{intersection.FromProject}\t{intersection.Thickness}");
                                Intersections.Add(intersection);
                            }
                        }
                    }
                }
            }

            // Сохранение данных в текстовый файл
            //string filePath = @"C:\Users\t.zaruba\Desktop\HolesTask.txt";
            //File.WriteAllLines(filePath, results);
            //MessageBox.Show("Результат", $"Данные успешно записаны в файл: {filePath}");
            return Intersections;
        }
    }
}
