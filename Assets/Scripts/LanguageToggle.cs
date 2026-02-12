using UnityEngine;
using UnityEngine.UI;

public class LanguageToggle : MonoBehaviour
{
    private Toggle languageToggle;

    void Awake()
    {
        languageToggle = GetComponent<Toggle>();
    }

    void OnEnable()
    {
        SyncToggleVisual(LocalizationManager.CurrentLanguage);
        LocalizationManager.OnLanguageChanged += SyncToggleVisual;
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= SyncToggleVisual;
    }

    void Start()
    {
        //languageToggle.isOn = (LocalizationManager.CurrentLanguage == "es");

        languageToggle.onValueChanged.AddListener(HandleToggle);
    }

    private void SyncToggleVisual(string lang)
    {
        languageToggle.onValueChanged.RemoveListener(HandleToggle);
        
        languageToggle.isOn = (lang == "es");
        
        languageToggle.onValueChanged.AddListener(HandleToggle);
    }

    void HandleToggle(bool isSpanish)
    {
        string newLang = isSpanish ? "es" : "en";
        
        LocalizationManager.SetLanguage(newLang);

        LanguageSelector selector = Object.FindFirstObjectByType<LanguageSelector>();
        if (selector != null)
        {
            if (isSpanish) selector.SetLanguageToSpanish();
            else selector.SetLanguageToEnglish();
        }
        //For Reference
        /*LanguageSelector selector = Object.FindFirstObjectByType<LanguageSelector>();
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
        }*/
    }
    
    private void OnDestroy()
    {
        languageToggle.onValueChanged.RemoveListener(HandleToggle);
    }
}