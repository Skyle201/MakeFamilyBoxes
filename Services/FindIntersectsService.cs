using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;
using System.IO;
using System.Windows;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;
namespace MakeFamilyBoxes.Services
{
    public class FindIntersectsService(MakeFamilyBoxesCommand makeFamilyBoxesCommand)
    {
        DocumentEntity StructureDocumentEntity;
        DocumentEntity LinkEngineerDocumentEntity;
        MakeFamilyBoxesCommand makeFamilyBoxesCommand = makeFamilyBoxesCommand;

        public void FindIntersects(GetRevitDocuments getRevitDocuments, DocumentEntity linkdoc, DocumentEntity structDoc)
        {

            if (linkdoc == null || structDoc == null) { MessageBox.Show("Не заданы документы"); return; }
            StructureDocumentEntity = structDoc;
            LinkEngineerDocumentEntity = linkdoc;
            // Получение документа
            List<Document> EngineerDocs = [getRevitDocuments.GetDocumentFromEntity(LinkEngineerDocumentEntity)];

            // Параметры для входных данных
            List<Document> StructureDocs = [getRevitDocuments.GetDocumentFromEntity(StructureDocumentEntity)]; 

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

            if (ducts.Count == 0 && pipes.Count == 0 && cableTrays.Count == 0) { MessageBox.Show("Не найдено элементов инженерных систем"); return; };
    
            // Поиск пересечений
            List<string> results = new List<string>();
            results.Add("DuctType\tWidth\tHeight\tWallType\tInsulation");

            foreach (Document EngineerDock in EngineerDocs)
            {
                foreach (Document doc in StructureDocs)
                {
                    List<Element> walls = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Walls)
                        .WhereElementIsNotElementType()
                        .ToElements() as List<Element>;

                    foreach (Element duct in ducts)
                    {
                        foreach (Element wall in walls)
                        {
                            if (DoesIntersect(duct, wall))
                            {
                                string ductType = GetTypeName(duct, EngineerDoc, doc);
                                string wallType = GetTypeName(wall, EngineerDoc, doc);
                                var (width, height) = GetDuctOrPipeSize(duct);
                                string insulation = ((double)duct.LookupParameter("Толщина изоляции")?.AsDouble() * 304.8).ToString("F0");
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
                                string pipeType = (pipe.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsElementId() is ElementId typeId) ? pipe.Document.GetElement(typeId)?.Name : "Unknown Type";
                                string wallType = (wall.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM)?.AsElementId() is ElementId typeIdd) ? wall.Document.GetElement(typeIdd)?.Name : "Unknown Type";
                                var (width, height) = GetDuctOrPipeSize(pipe);
                                string insulation = ((double)pipe.LookupParameter("Толщина изоляции")?.AsDouble() * 304.8).ToString("F0");
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
                                string cableTrayType = GetTypeName(cableTray, EngineerDoc, doc);
                                string wallType = GetTypeName(wall, EngineerDoc, doc);
                                var (width, height) = GetDuctOrPipeSize(cableTray);
                                results.Add($"{cableTrayType}\t{width}\t{height}\t{wallType}\t0");
                            }
                        }
                    }
                }
            }

            // Сохранение данных в текстовый файл
            string filePath = @"C:\Users\t.zaruba\Desktop\HolesTask.txt";
            File.WriteAllLines(filePath, results);
            TaskDialog.Show("Результат", $"Данные успешно записаны в файл: {filePath}");
            
            // Функции
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

            (double Width, double Height) GetDuctOrPipeSize(Element el)
            {
                if (el == null || el.Category == null)
                    return (0, 0);

                BuiltInCategory category = (BuiltInCategory)el.Category.Id.IntegerValue;

                switch (category)
                {
                    case BuiltInCategory.OST_DuctCurves:
                        var duct = el as Duct;
                        if (duct == null || duct.DuctType == null)
                            return (0, 0);

                        var ductShape = duct.DuctType.Shape;
                        if (ductShape == ConnectorProfileType.Round)
                        {
                            double diameter = el.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM)?.AsDouble() ?? 0;
                            return (diameter * 304.8, diameter * 304.8);
                        }
                        else if (ductShape == ConnectorProfileType.Rectangular || ductShape == ConnectorProfileType.Oval)
                        {
                            double width = el.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM)?.AsDouble() ?? 0;
                            double height = el.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM)?.AsDouble() ?? 0;
                            return (width * 304.8, height * 304.8);
                        }
                        break;

                    case BuiltInCategory.OST_PipeCurves:
                        var pipe = el as Pipe;
                        if (pipe == null || pipe.PipeType == null)
                            return (0, 0);

                        var pipeShape = pipe.PipeType.Shape;
                        if (pipeShape == ConnectorProfileType.Round)
                        {
                            double diameter = el.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM)?.AsDouble() ?? 0;
                            return (diameter * 304.8, diameter * 304.8);
                        }
                        break;

                    case BuiltInCategory.OST_CableTray:
                        double cableTrayWidth = el.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM)?.AsDouble() ?? 0;
                        double cableTrayHeight = el.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM)?.AsDouble() ?? 0;
                        return (cableTrayWidth * 304.8, cableTrayHeight * 304.8);
                }

                return (0, 0);
            }

            string GetTypeName(Element el, Document engdoc, Document strucdoc)
            {
                ElementId typeId = el.GetTypeId();
                Element typeElement = engdoc.GetElement(typeId) ?? strucdoc.GetElement(typeId);
                return typeElement?.Name ?? "Unknown";
            }
        }
    }
}
