using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static string CurrentLanguage => PlayerPrefs.GetString("language", "en");

    public static void SetLanguage(string langCode)
    {
        PlayerPrefs.SetString("language", langCode);
        PlayerPrefs.Save();
    }
}
