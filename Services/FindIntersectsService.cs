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
    public class FindIntersectsService(MakeFamilyBoxesCommand makeFamilyBoxesCommand)
    {
        DocumentEntity StructureDocumentEntity;
        DocumentEntity LinkEngineerDocumentEntity;
        private readonly MakeFamilyBoxesCommand makeFamilyBoxesCommand = makeFamilyBoxesCommand;

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
            List<string> results = ["DuctType\tWidth\tHeight\tWallType\tInsulation"];

            foreach (Document EngineerDock in EngineerDocs)
            {
                foreach (Document doc in StructureDocs)
                {
                    List<Element> walls = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .WhereElementIsNotElementType()
                        .ToElements() as List<Element>;

                    List<Element> floors = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Floors)
                        
                        .ToElements().ToList();

                    //Прогоняемся по элементам и ищем пересечения

                    foreach (Element duct in ducts)
                    {
                        foreach (Element wall in walls)
                        {
                          IntersectionEntity intersction = IntersectionEntity.TryCreateEntity(duct, wall, EngineerDock, doc);
                            if (intersction != null)
                            {
                                results.Add($"{intersction.EngineerPipeType}\t{intersction.Width}\t{intersction.Height}\t{intersction.StructureType}\t{intersction.Insulation}\t{intersction.CenterCoordinates}\t{intersction.FromProject}\t{intersction.Thickness}");
                                Intersections.Add(intersction);
                            }
                        }
                        foreach (Element floor in floors)
                        {
                            IntersectionEntity intersction = IntersectionEntity.TryCreateEntity(duct, floor, EngineerDock, doc);
                            if (intersction != null)
                            {
                                results.Add($"{intersction.EngineerPipeType}\t{intersction.Width}\t{intersction.Height}\t{intersction.StructureType}\t{intersction.Insulation}\t{intersction.CenterCoordinates}\t{intersction.FromProject}\t{intersction.Thickness}");
                                Intersections.Add(intersction);
                            }
                        }
                    }

                    foreach (Element pipe in pipes)
                    {
                        foreach (Element wall in walls)
                        {
                            IntersectionEntity intersction = IntersectionEntity.TryCreateEntity(pipe, wall, EngineerDock, doc);
                            if (intersction != null)
                            {
                                results.Add($"{intersction.EngineerPipeType}\t{intersction.Width}\t{intersction.Height}\t{intersction.StructureType}\t{intersction.Insulation}\t{intersction.CenterCoordinates}\t{intersction.FromProject}\t{intersction.Thickness}");
                                Intersections.Add(intersction);
                            }
                        }
                        foreach (Element floor in floors)
                        {
                            IntersectionEntity intersction = IntersectionEntity.TryCreateEntity(pipe, floor, EngineerDock, doc);
                            if (intersction != null)
                            {
                                results.Add($"{intersction.EngineerPipeType}\t{intersction.Width}\t{intersction.Height}\t{intersction.StructureType}\t{intersction.Insulation}\t{intersction.CenterCoordinates}\t{intersction.FromProject}\t{intersction.Thickness}");
                                Intersections.Add(intersction);
                            }
                        }
                    }

                    foreach (Element cableTray in cableTrays)
                    {
                        foreach (Element wall in walls)
                        {
                            IntersectionEntity intersction = IntersectionEntity.TryCreateEntity(cableTray, wall, EngineerDock, doc);
                            if (intersction != null)
                            {
                                results.Add($"{intersction.EngineerPipeType}\t{intersction.Width}\t{intersction.Height}\t{intersction.StructureType}\t{intersction.Insulation}\t{intersction.CenterCoordinates}\t{intersction.FromProject}\t{intersction.Thickness}");
                                Intersections.Add(intersction);
                            }
                        }
                        foreach (Element floor in floors)
                        {
                            IntersectionEntity intersction = IntersectionEntity.TryCreateEntity(cableTray, floor, EngineerDock, doc);
                            if (intersction != null)
                            {
                                results.Add($"{intersction.EngineerPipeType}\t{intersction.Width}\t{intersction.Height}\t{intersction.StructureType}\t{intersction.Insulation}\t{intersction.CenterCoordinates}\t{intersction.FromProject}\t{intersction.Thickness}");
                                Intersections.Add(intersction);
                            }
                        }
                    }
                }
            }

            // Сохранение данных в текстовый файл
            string filePath = @"C:\Users\t.zaruba\Desktop\HolesTask.txt";
            File.WriteAllLines(filePath, results);
            MessageBox.Show("Результат", $"Данные успешно записаны в файл: {filePath}");
            return Intersections;
        }
    }
}
