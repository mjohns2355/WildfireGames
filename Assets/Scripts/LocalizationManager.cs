using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LocalizationManager
{
    public static Action<string> OnLanguageChanged;
    public static string CurrentLanguage => PlayerPrefs.GetString("language", "en");

    public static void SetLanguage(string lang)
    {
        PlayerPrefs.SetString("language", lang);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke(lang);
    }
}
