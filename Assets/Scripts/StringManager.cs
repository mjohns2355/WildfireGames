using System.Collections.Generic;
using UnityEngine;
using System;

public class StringManager : MonoBehaviour
{
    public static StringManager Instance { get; private set; }
    private Dictionary<string, LocalizedString> localizedStrings = new Dictionary<string, LocalizedString>();
    public event Action OnStringsLoadedEvent;
    public bool IsReady { get; private set; } = false;

    private string currentFileName;

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
        //currentFileName = fileName;
        //LocalizedFileLoader.Load<StringCollection>(fileName, OnStringsLoaded);
    }

    public void RefreshCurrentLanguage()
    {
        if (!string.IsNullOrEmpty(currentFileName))
            LoadSceneStrings(currentFileName);
    }

    private void OnStringsLoaded(StringCollection collection)
    {
        if (collection == null || collection.strings == null)
        {
            Debug.LogError($"[StringManager] Failed to load {currentFileName} or file is empty!");
            return;
        }

        var newStrings = new Dictionary<string, LocalizedString>();
        foreach (var item in collection.strings)
        {
            if (!newStrings.ContainsKey(item.key))
                newStrings.Add(item.key, item);
        }
        localizedStrings = newStrings;
        /*localizedStrings.Clear();
        foreach (var item in collection.strings)
        {
            if (!localizedStrings.ContainsKey(item.key))
                localizedStrings.Add(item.key, item);
        }*/
        
        IsReady = true;
        OnStringsLoadedEvent?.Invoke();
    }

    public string GetText(string key)
    {
        if (!IsReady || localizedStrings.Count == 0) return "";
        //if (localizedStrings.Count == 0) return null;

        if (!localizedStrings.ContainsKey(key))
        {
            return $"[Missing: {key}]";
        }

        return localizedStrings[key].GetText(LocalizationManager.CurrentLanguage);
    }

    public string GetAudioPath(string key)
    {
        if (localizedStrings.Count == 0) return null;

        if (!localizedStrings.ContainsKey(key))
        {
            return null;
        }

        return localizedStrings[key].GetAudioPath(LocalizationManager.CurrentLanguage);
    }
}

