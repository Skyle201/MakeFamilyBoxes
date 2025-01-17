using Autodesk.Revit.DB;
using MakeFamilyBoxes.Models;
using System.Windows;
namespace MakeFamilyBoxes.Services
{
    public class FindIntersectsService()
    {
        DocumentEntity StructureDocumentEntity;
        DocumentEntity LinkEngineerDocumentEntity;

        public List<IntersectionEntity> FindIntersects(GetRevitDocuments getRevitDocuments, DocumentEntity linkdoc, DocumentEntity structDoc,string minSizeOfSquareBox,string minSizeOfRoundBox)
        {
            if (linkdoc == null || structDoc == null) { MessageBox.Show("Не заданы документы"); return null ; }
            StructureDocumentEntity = structDoc;
            LinkEngineerDocumentEntity = linkdoc;
            double MinSizeOfSquareBox = 0;
            double MinSizeOfRoundBox = 0;
            try
            {
                MinSizeOfSquareBox = Convert.ToDouble(minSizeOfSquareBox);
            }
            catch (Exception) { }
            try
            {
                MinSizeOfRoundBox = Convert.ToDouble(minSizeOfRoundBox);
            }
            catch (Exception) { }

            // Получение документа
            List<Document> EngineerDocs = [getRevitDocuments.GetDocumentFromEntity(LinkEngineerDocumentEntity)];

            // Параметры для входных данных
            List<Document> StructureDocs = [getRevitDocuments.GetDocumentFromEntity(StructureDocumentEntity)]; 
            List<Element> Ducts = [];
            List<Element> Pipes = [];
            List<Element> CableTrays = [];


            foreach (Document doc in EngineerDocs)
            {
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
                Ducts.AddRange(ducts);
                Pipes.AddRange(pipes);
                CableTrays.AddRange(cableTrays);
            }
            // Сбор данных о сетях
            
            if (Ducts.Count == 0 && Pipes.Count == 0 && CableTrays.Count == 0) { MessageBox.Show("Не найдено элементов инженерных систем"); return null; };

            List<Element> Walls = [];
            List<Element> Floors = [];

            foreach (Document doc in StructureDocs)
            {
                List<Element> walls = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsNotElementType()
                    .ToElements() as List<Element>;


                List<Element> floors = [.. new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Floors)
                        .ToElements()];
                Walls.AddRange(walls);
                Floors.AddRange(floors);
            }
            IntersectionHelper helper = new();
            List<IntersectionEntity> Intersections = helper.FindIntersection(EngineerDocs, StructureDocs, Ducts, Pipes, CableTrays, Walls, Floors, MinSizeOfSquareBox, MinSizeOfRoundBox);

            return Intersections;
        }
        }
    }
