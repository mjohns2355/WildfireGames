using UnityEngine;
using UnityEngine.UI;

public class LanguageToggle : MonoBehaviour
{
    private Toggle languageToggle;

    void Awake()
    {
        languageToggle = GetComponent<Toggle>();
    }

    void Start()
    {
        languageToggle.isOn = (LocalizationManager.CurrentLanguage == "es");

        languageToggle.onValueChanged.AddListener(HandleToggle);
    }

    void HandleToggle(bool isSpanish)
    {
        LanguageSelector selector = Object.FindFirstObjectByType<LanguageSelector>();
        if (selector != null)
        {
            if (isSpanish)
                selector.SetLanguageToSpanish();
            else
                selector.SetLanguageToEnglish();
        }
        else
        {
            Debug.LogError("[ToggleConnector] Could not find LanguageSelector in the scene!");
        }
    }
    
    private void OnDestroy()
    {
        languageToggle.onValueChanged.RemoveListener(HandleToggle);
    }
}