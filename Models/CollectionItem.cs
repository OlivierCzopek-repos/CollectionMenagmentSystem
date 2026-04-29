using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManagementSystem.Models
{
    public class CollectionItem
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public int Rating { get; set; } = 5;
        public string Comment { get; set; }
        public Dictionary<string, string> CustomValues { get; set; } = new Dictionary<string, string>();
    }
}
