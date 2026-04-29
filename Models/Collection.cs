using System.Collections.Generic;

namespace CollectionManagementSystem.Models
{
  

   

    public class Collection
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<CustomColumn> CustomColumns { get; set; } = new List<CustomColumn>();
        public List<CollectionItem> Items { get; set; } = new List<CollectionItem>();
    }
}
