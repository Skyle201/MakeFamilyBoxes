using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeFamilyBoxes.Models
{
    public class FamilyEntity
    {
        public string Name { get; }
        public int Id { get; }

        public FamilyEntity(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}
