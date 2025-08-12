using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    // Marker interface for inventory items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Electronic item
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"[E] {Id} - {Name} ({Brand}) Qty: {Quantity} Warranty: {WarrantyMonths}mo";
    }

    // Grocery item
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() => $"[G] {Id} - {Name} Qty: {Quantity} Expiry: {ExpiryDate:d}";
    }

    // Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // Generic InventoryRepository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // Warehouse manager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            try
            {
                _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "Acer", 24));
                _electronics.AddItem(new ElectronicItem(2, "Smartphone", 10, "Samsung", 12));
                _groceries.AddItem(new GroceryItem(101, "Rice", 50, DateTime.Now.AddMonths(6)));
                _groceries.AddItem(new GroceryItem(102, "Milk", 20, DateTime.Now.AddDays(10)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed error: {ex.Message}");
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var list = repo.GetAllItems();
            foreach (var item in list) Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var existing = repo.GetItemById(id);
                int newQty = checked(existing.Quantity + quantity);
                repo.UpdateQuantity(id, newQty);
                Console.WriteLine($"Increased item {id} to {newQty}");
            }
            catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException || ex is OverflowException)
            {
                Console.WriteLine($"Error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item {id}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }

        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("Grocery items:");
            manager.PrintAllItems(manager._groceries);

            Console.WriteLine("\nElectronic items:");
            manager.PrintAllItems(manager._electronics);

            // Try to add duplicate
            try
            {
                manager._groceries.AddItem(new GroceryItem(101, "Sugar", 5, DateTime.Now.AddYears(1)));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"\nDuplicate add caught: {ex.Message}");
            }

            // Remove non-existent
            manager.RemoveItemById(manager._electronics, 999);

            // Update with invalid quantity
            try
            {
                manager._electronics.UpdateQuantity(1, -10);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"\nInvalid quantity caught: {ex.Message}");
            }

            Console.WriteLine("\nFinal inventories:");
            manager.PrintAllItems(manager._groceries);
            manager.PrintAllItems(manager._electronics);
        }
    }
}
