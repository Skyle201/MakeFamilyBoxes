namespace MakeFamilyBoxes.Models
{
    public class DocumentEntity(string title, int id)
    {
        public string Title { get; } = title;
        public int Id { get; } = id;
    }
}
