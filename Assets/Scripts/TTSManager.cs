using UnityEngine;

public static class TTSManager
{
    private const float DefaultVolume = 0.20f;

    public static bool IsEnabled => PlayerPrefs.GetInt("tts_enabled", 1) == 1;

    public static float Volume
    {
        get => PlayerPrefs.GetFloat("tts_volume", DefaultVolume);
        set
        {
            PlayerPrefs.SetFloat("tts_volume", Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

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
