using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizedText : MonoBehaviour
{
    public string key;
    private TMP_Text tmpText;
    private Text uiText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeLocalization());
    }

    private IEnumerator InitializeLocalization()
    {
        while (StringManager.Instance == null)
        {
            yield return null;
        }

        StringManager.Instance.OnStringsLoadedEvent += UpdateText;

        UpdateText();
    }

    private void OnDisable()
    {
        if (StringManager.Instance != null)
            StringManager.Instance.OnStringsLoadedEvent -= UpdateText;
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(key) || StringManager.Instance == null) return;

        string localized = StringManager.Instance.GetText(key);

        if (!string.IsNullOrEmpty(localized))
        {
            if (tmpText != null) tmpText.text = localized;
            if (uiText != null) uiText.text = localized;
        }
    }
}
