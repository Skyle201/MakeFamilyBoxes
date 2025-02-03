using MakeFamilyBoxes.ViewModels;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Linq;
using MakeFamilyBoxes.Models;
using System.Windows.Markup;

public class VMSaver
{
    private readonly string _filePath;

    public VMSaver()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        _filePath = Path.Combine(assemblyPath, "vmsettings.json");
    }

    public void SaveVM(MakeFamilyBoxesViewModel viewModel)
    {
        var familyEntitiesNames = viewModel.FamilyEntities.Select(family => family.Name).ToList();

        var safeData = new SavedData
        {
            MinSizeOfRoundBox = viewModel.MinSizeOfRoundBox,
            MinSizeOfSquareBox = viewModel.MinSizeOfSquareBox,
            BoxId = viewModel.BoxId,
            OffsetFromCuttingEdge = viewModel.OffsetFromCuttingEdge,
            SelectedRoundBoxName = viewModel.SelectedFamilyRoundBox?.Name,
            SelectedSquareBoxName = viewModel.SelectedFamilySquareBox?.Name,
            HubDocumentTitle = viewModel.SelectedHubDocument?.Title,
            FamilyEntitiesNames = familyEntitiesNames
        };

        try
        {
            string jsonStr = JsonConvert.SerializeObject(safeData, Formatting.Indented);
            File.WriteAllText(_filePath, jsonStr);
        }
        catch (Exception) { };
    }

    public void LoadVM(MakeFamilyBoxesViewModel viewModel)
    {
        if (!File.Exists(_filePath))
            return;

        string jsonStr = File.ReadAllText(_filePath);
        var data = JsonConvert.DeserializeObject<SavedData>(jsonStr);

        if (data != null)
        {
            viewModel.MinSizeOfRoundBox = data.MinSizeOfRoundBox;
            viewModel.MinSizeOfSquareBox = data.MinSizeOfSquareBox;
            viewModel.BoxId = data.BoxId;
            viewModel.OffsetFromCuttingEdge = data.OffsetFromCuttingEdge;


            viewModel.SelectedHubDocument = viewModel.DocumentEntities
                .FirstOrDefault(doc => doc.Title == data.HubDocumentTitle);

            if (data.FamilyEntitiesNames != null)
            {
                viewModel.FamilyEntities = data.FamilyEntitiesNames
                    .Select(name => new FamilyEntity(name, 0))
                    .ToList();
            }
            viewModel.SelectedFamilyRoundBox = viewModel.FamilyEntities
                .FirstOrDefault(family => family.Name == data.SelectedRoundBoxName);
            viewModel.SelectedFamilySquareBox = viewModel.FamilyEntities
                .FirstOrDefault(family => family.Name == data.SelectedSquareBoxName);
        }
    }
    public void FactoryReset(MakeFamilyBoxesViewModel viewModel)
    {
        viewModel.MinSizeOfRoundBox = null;
        viewModel.MinSizeOfSquareBox = null;
        viewModel.BoxId = null;
        viewModel.OffsetFromCuttingEdge = null;
        viewModel.SelectedFamilyRoundBox = null;
        viewModel.SelectedFamilySquareBox = null;
        viewModel.SelectedEngineersDocument = null;
        viewModel.SelectedModelDocument = null;
        viewModel.SelectedHubDocument = null;
    }
}
    

public class SavedData
{
    public string MinSizeOfRoundBox { get; set; }
    public string MinSizeOfSquareBox { get; set; }
    public string BoxId { get; set; }
    public string OffsetFromCuttingEdge { get; set; }
    public string SelectedRoundBoxName { get; set; }
    public string SelectedSquareBoxName { get; set; }
    public string HubDocumentTitle { get; set; }
    public List<string> FamilyEntitiesNames { get; set; }
}
