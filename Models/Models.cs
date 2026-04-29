using System.Collections.Generic;

namespace CollectionManagementSystem.Models
{
    public class CustomColumn
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Type { get; set; } 
        public List<string> Options { get; set; } = new List<string>();
    }

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

    public class Collection
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<CustomColumn> CustomColumns { get; set; } = new List<CustomColumn>();
        public List<CollectionItem> Items { get; set; } = new List<CollectionItem>();
    }
}
