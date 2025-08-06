namespace MakeFamilyBoxes.Models.Entities
{
    public class FamilyEntity(string name, int id)
    {
        public string Name { get; } = name;
        public int Id { get; } = id;
    }
}
