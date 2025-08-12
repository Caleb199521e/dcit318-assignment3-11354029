using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventoryRecordsApp
{
    // Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Immutable record implementing interface
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic InventoryLogger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            try
            {
                using var fs = File.Create(_filePath);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                };
                JsonSerializer.Serialize(fs, _log, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log.Clear();
                if (items != null) _log.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Pen", 100, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Notebook", 50, DateTime.Now.AddDays(-1)));
            _logger.Add(new InventoryItem(3, "Stapler", 10, DateTime.Now.AddDays(-30)));
            _logger.Add(new InventoryItem(4, "Highlighter", 40, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("No items.");
                return;
            }

            foreach (var it in items)
                Console.WriteLine($"{it.Id} - {it.Name} Qty:{it.Quantity} Added:{it.DateAdded:d}");
        }

        public static void Main()
        {
            string path = "inventory_log.json";

            var app = new InventoryApp(path);
            app.SeedSampleData();

            Console.WriteLine("Saving data...");
            app.SaveData();

            // Simulate clearing memory by creating a new instance
            var freshApp = new InventoryApp(path);
            Console.WriteLine("\nLoading data into new session...");
            freshApp.LoadData();
            freshApp.PrintAllItems();
        }
    }
}
