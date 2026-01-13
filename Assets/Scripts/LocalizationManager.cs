using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    public static string CurrentLanguage => PlayerPrefs.GetString("language", "en");

    public static void SetLanguage(string lang)
    {
        PlayerPrefs.SetString("language", lang);
        PlayerPrefs.Save();
    }
}
