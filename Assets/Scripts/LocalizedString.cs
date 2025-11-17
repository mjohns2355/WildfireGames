using System;

[System.Serializable]
public class LocalizedString
{
    public string key;
    public string text_en;
    public string text_es; 

    public string GetText(string lang)
    {
        return lang == "es" ? text_es : text_en;
    }
}
