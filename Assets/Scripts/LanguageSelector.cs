using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public void SetLanguageToEnglish()
    {
        LocalizationManager.SetLanguage("en");
    }

    public void SetLanguageToSpanish()
    {
        LocalizationManager.SetLanguage("es");
    }
}
