using System.Collections.Generic;
using UnityEngine;

public static class FYT_ItemCatalog
{
    public enum ItemTier { Essential, Additional }

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
        Load();
        if (_lookup != null && _lookup.TryGetValue(itemName, out FYT_ItemEntry entry))
        {
            switch (entry.tier)
            {
                case "Essential":  return ItemTier.Essential;
                default:           return ItemTier.Additional;
            }
        }
        return ItemTier.Additional;
    }


    public static int GetEssentialCount()
    {
        Load();
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
        Load();
        var names = new List<string>();
        if (_lookup == null) return names;
        foreach (var entry in _lookup.Values)
        {
            if (entry.tier == "Essential") names.Add(entry.name);
        }
        return names;
    }

    public static bool EnablesRadio(string itemName)
    {
        return itemName == "Radio";
    }

    public static bool IsKey(string itemName)
    {
        return itemName == "Car Key";
    }

    public static bool IsEssential(string itemName)
    {
        return GetTier(itemName) == ItemTier.Essential;
    }
}
