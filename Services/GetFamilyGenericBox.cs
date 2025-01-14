using Autodesk.Revit.DB;
using MakeFamilyBoxes.Commands;
using MakeFamilyBoxes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Services
{
    public class GetFamilyGenericBox (GetRevitDocuments getRevitDocuments)
    {
        private readonly GetRevitDocuments _getRevitDocuments = getRevitDocuments;
        public List<FamilyEntity> GetFamilyEntities(DocumentEntity _documentEntity)
        {
            Document foundDocument = _getRevitDocuments.GetDocumentFromEntity(_documentEntity);
            var elements = new FilteredElementCollector(foundDocument).OfClass(typeof(Family)).Cast<Family>().ToList().Where(family => family.Name.ToLower().Contains("adsk")).Where(family => family.FamilyCategory?.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel)
                .ToList();
            List<FamilyEntity> BoxFamilies = [];
            foreach (var fam in elements)
            {
                BoxFamilies.Add(new FamilyEntity(fam.Name, int.Parse(fam.Id.ToString())));
            }

            return BoxFamilies;
        }
    }
}
