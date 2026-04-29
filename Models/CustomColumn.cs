using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManagementSystem.Models
{
    public class CustomColumn
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }
}
