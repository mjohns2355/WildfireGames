using System.Collections.Generic;
using UnityEngine;
using System;

public class StringManager : MonoBehaviour
{
    public static StringManager Instance { get; private set; }

    private Dictionary<string, LocalizedString> localizedStrings = new Dictionary<string, LocalizedString>();
    
    public event Action OnStringsLoadedEvent;

    private string currentFileName;
    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneStrings(string fileName)
    {
        IsReady = false;
        currentFileName = fileName;
        LocalizedFileLoader.Load<StringCollection>(fileName, OnStringsLoaded);
    }

    public void RefreshCurrentLanguage()
    {
        if (!string.IsNullOrEmpty(currentFileName))
            LoadSceneStrings(currentFileName);
    }

    private void OnStringsLoaded(StringCollection collection)
    {
        if (collection == null) return;

        localizedStrings.Clear();
        foreach (var item in collection.strings)
        {
            if (!localizedStrings.ContainsKey(item.key))
                localizedStrings.Add(item.key, item);
        }

        IsReady = true;
        Debug.Log($"[StringManager] {currentFileName} is ready.");
        OnStringsLoadedEvent?.Invoke();
    }

    public string GetText(string key)
    {
        if (!IsReady || !localizedStrings.ContainsKey(key))
        {
            return null;
        }

        return localizedStrings[key].GetText(LocalizationManager.CurrentLanguage);
    }
}

