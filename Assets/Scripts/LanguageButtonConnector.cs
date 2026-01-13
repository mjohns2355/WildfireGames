using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonConnector : MonoBehaviour
{
    public enum LanguageChoice { English, Spanish }
    public LanguageChoice targetLanguage;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(HandleClick);
    
    }

    void HandleClick()
    {
        LanguageSelector selector = Object.FindFirstObjectByType<LanguageSelector>();

        if (selector != null)
        {
            if (targetLanguage == LanguageChoice.English)
                selector.SetLanguageToEnglish();
            else
                selector.SetLanguageToSpanish();
        }
        else
        {
            Debug.LogError("[Connector] Could not find LanguageSelector in the scene!");
        }
    }
}
