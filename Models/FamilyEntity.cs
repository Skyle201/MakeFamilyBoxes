using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Models
{
    public class FamilyEntity(string name, int id)
    {
        public string Name { get; } = name;
        public int Id { get; } = id;
    }
}
