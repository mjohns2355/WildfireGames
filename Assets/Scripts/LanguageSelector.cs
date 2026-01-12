using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public void SetLanguageToEnglish()
    {
        LocalizationManager.SetLanguage("en");
        StringManager.Instance.RefreshCurrentLanguage();
    }

    public void SetLanguageToSpanish()
    {
        LocalizationManager.SetLanguage("es");
        StringManager.Instance.RefreshCurrentLanguage();
    }
}
