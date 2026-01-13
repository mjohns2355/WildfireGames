using System;

[System.Serializable]
public class LocalizedString
{
    public string key;
    public string text_en;
    public string text_es; 

    public string GetText(string lang)
    {
        if (lang == "es")
        {
            return text_es;
        }
        return text_en;
    }
}
