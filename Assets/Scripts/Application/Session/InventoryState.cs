using System;
using System.Collections.Generic;

namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Minimal inventory state for VS01 loops.
    /// </summary>
    [Serializable]
    public class InventoryState
    {
        public List<ItemStack> items = new();

        public int GetCount(string itemId)
        {
            var stack = items.Find(i => i.itemId == itemId);
            return stack?.count ?? 0;
        }

        public void Add(string itemId, int amount)
        {
            var stack = items.Find(i => i.itemId == itemId);
            if (stack == null)
            {
                items.Add(new ItemStack { itemId = itemId, count = amount });
                return;
            }

            stack.count += amount;
        }

        public bool TryRemove(string itemId, int amount)
        {
            var stack = items.Find(i => i.itemId == itemId);
            if (stack == null || stack.count < amount)
            {
                return false;
            }

            stack.count -= amount;
            return true;
        }
    }

    [Serializable]
    public class ItemStack
    {
        public string itemId = string.Empty;
        public int count;
    }
}
