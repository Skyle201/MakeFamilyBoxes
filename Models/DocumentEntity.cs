using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Models
{
    public class DocumentEntity(string title, int id)
    {
        public string Title { get; } = title;
        public int Id { get; } = id;
    }
}
