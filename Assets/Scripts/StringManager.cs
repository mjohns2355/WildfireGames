using System.Collections.Generic;
using UnityEngine;
using System;

public class StringManager : MonoBehaviour
{
    public static StringManager Instance { get; private set; }

    private Dictionary<string, LocalizedString> localizedStrings = new Dictionary<string, LocalizedString>();

    public event Action OnStringsLoadedEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LocalizedFileLoader.Load<StringCollection>("TextStrings.json", OnStringsLoaded);
    }

    private void OnStringsLoaded(StringCollection collection)
    {
        if (collection == null)
        {
            Debug.LogError("Failed to load TextStrings.json");
            return;
        }

        localizedStrings.Clear();
        foreach (var item in collection.strings)
        {
            if (!localizedStrings.ContainsKey(item.key))
                localizedStrings.Add(item.key, item);
            else
                Debug.LogWarning($"Duplicate key found in JSON: {item.key}");
        }

        Debug.Log($"Loaded {localizedStrings.Count} localized strings from TextStrings.json.");

        OnStringsLoadedEvent?.Invoke();
    }

    public string GetText(string key)
    {
        if (!localizedStrings.ContainsKey(key))
        {
            Debug.LogWarning($"Key not found: {key}");
            return $"[Missing: {key}]";
        }

        string lang = LocalizationManager.CurrentLanguage;
        return localizedStrings[key].GetText(lang);
    }
}

