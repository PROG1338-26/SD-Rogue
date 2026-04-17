using System.Collections.Generic;

namespace RogueLib.Dungeon
{
    public class Inventory
    {
        private readonly List<Item> _items = new List<Item>();
       // private readonly Dictionary<string, int> inventory;
        public void Add(Item item)
        {
            if (item == null) return;
            _items.Add(item);
        }
        public bool Remove(Item item) => _items.Remove(item);

        public IReadOnlyList<Item> Items => _items.AsReadOnly();
        class InventoryItem
        {
            public required Item Item;
            public int Quantity;
        }
    }
}
