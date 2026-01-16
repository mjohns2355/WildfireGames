using UnityEngine;

public static class TTSManager
{
    public static bool IsEnabled => PlayerPrefs.GetInt("tts_enabled", 1) == 1;

    public static void SetEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("tts_enabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void Toggle()
    {
        SetEnabled(!IsEnabled);
    }
}
