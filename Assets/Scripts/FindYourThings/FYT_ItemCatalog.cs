using System.Collections.Generic;
using UnityEngine;

public static class FYT_ItemCatalog
{
    public enum ItemTier { Essential, Additional, Unnecessary }

    private static Dictionary<string, FYT_ItemEntry> _lookup;
    private static bool _loaded = false;

    public static void Load()
    {
        if (_loaded) return;
        LocalizedFileLoader.Load<FYT_ItemCatalogData>("FYTItemCatalog.json", (data) =>
        {
            _lookup = new Dictionary<string, FYT_ItemEntry>();
            if (data != null && data.items != null)
            {
                foreach (var entry in data.items)
                {
                    _lookup[entry.name] = entry;
                }
            }
            _loaded = true;
        });
    }

    public static ItemTier GetTier(string itemName)
    {
        if (_lookup != null && _lookup.TryGetValue(itemName, out FYT_ItemEntry entry))
        {
            switch (entry.tier)
            {
                case "Essential":  return ItemTier.Essential;
                case "Additional": return ItemTier.Additional;
                default:           return ItemTier.Unnecessary;
            }
        }
        return ItemTier.Unnecessary;
    }

    public static bool IsUnnecessary(string itemName)
    {
        return GetTier(itemName) == ItemTier.Unnecessary;
    }

    public static bool IsPenaltyItem(string itemName)
    {
        if (_lookup != null && _lookup.TryGetValue(itemName, out FYT_ItemEntry entry))
        {
            return entry.penalty;
        }
        return false;
    }

    public static int GetEssentialCount()
    {
        int count = 0;
        if (_lookup == null) return 0;
        foreach (var entry in _lookup.Values)
        {
            if (entry.tier == "Essential") count++;
        }
        return count;
    }

    public static List<string> GetEssentialNames()
    {
        var names = new List<string>();
        if (_lookup == null) return names;
        foreach (var entry in _lookup.Values)
        {
            if (entry.tier == "Essential") names.Add(entry.name);
        }
        return names;
    }
}
