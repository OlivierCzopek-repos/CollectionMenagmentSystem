using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CollectionManagementSystem.Models;

namespace CollectionManagementSystem.Services
{
    public static class DataManager
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");

        public static void LogFilePath()
        {
            Debug.WriteLine($"[DATA] Zapis/Odczyt kolekcji odbywa sie w pliku: {FilePath}");
        }

        public static void SaveData(List<Collection> collections)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(FilePath, false);
                foreach (var collection in collections)
                {
                    // Zapis kolekcji
                    sw.WriteLine($"COLLECTION|^|{collection.Id}|^|{collection.Name}");

                    // Zapis niestandardowych kolumn
                    foreach (var col in collection.CustomColumns)
                    {
                        var optionsStr = string.Join("~", col.Options);
                        sw.WriteLine($"COLUMN|^|{col.Id}|^|{col.Name}|^|{col.Type}|^|{optionsStr}");
                    }

                    // Zapis elementów kolekcji
                    foreach (var item in collection.Items)
                    {
                        sw.WriteLine($"ITEM|^|{item.Id}|^|{item.Name}|^|{item.Price}|^|{item.Status}|^|{item.Rating}|^|{item.Comment}");

                        // Zapis wartoœci niestandardowych elementu
                        foreach (var kvp in item.CustomValues)
                        {
                            sw.WriteLine($"CUSTOMVALUE|^|{item.Id}|^|{kvp.Key}|^|{kvp.Value}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Blad przy zapisie: " + ex.Message);
            }
        }

        public static List<Collection> LoadData()
        {
            var collections = new List<Collection>();
            LogFilePath();

            if (!File.Exists(FilePath))
                return collections;

            try
            {
                Collection currentCollection = null;
                CollectionItem currentItem = null;

                foreach (var line in File.ReadAllLines(FilePath))
                {
                    var parts = line.Split(new[] { "|^|" }, StringSplitOptions.None);
                    if (parts.Length == 0) continue;

                    string type = parts[0];

                    if (type == "COLLECTION" && parts.Length >= 3)
                    {
                        currentCollection = new Collection
                        {
                            Id = parts[1],
                            Name = parts[2],
                            Items = new List<CollectionItem>(),
                            CustomColumns = new List<CustomColumn>()
                        };
                        collections.Add(currentCollection);
                    }
                    else if (type == "COLUMN" && currentCollection != null && parts.Length >= 5)
                    {
                        var col = new CustomColumn
                        {
                            Id = parts[1],
                            Name = parts[2],
                            Type = parts[3],
                            Options = parts[4].Split('~').Where(o => !string.IsNullOrEmpty(o)).ToList()
                        };
                        currentCollection.CustomColumns.Add(col);
                    }
                    else if (type == "ITEM" && currentCollection != null && parts.Length >= 7)
                    {
                        currentItem = new CollectionItem
                        {
                            Id = parts[1],
                            Name = parts[2],
                            Price = double.TryParse(parts[3], out var p) ? p : 0,
                            Status = parts[4],
                            Rating = int.TryParse(parts[5], out var r) ? r : 1,
                            Comment = parts[6],
                            CustomValues = new Dictionary<string, string>()
                        };
                        currentCollection.Items.Add(currentItem);
                    }
                    else if (type == "CUSTOMVALUE" && parts.Length >= 4)
                    {
                        if (currentItem != null && currentItem.Id == parts[1])
                        {
                            currentItem.CustomValues[parts[2]] = parts[3];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Blad przy odczycie: " + ex.Message);
            }

            return collections;
        }
    }
}
