using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [Tooltip("Key used to fetch localized string from JSON")]
    public string key;

    private TMP_Text tmpText;
    private Text uiText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();

        if (StringManager.Instance != null)
        {
            StringManager.Instance.OnStringsLoadedEvent += UpdateText;

            UpdateText();
        }
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning($"LocalizedText: key not set on {gameObject.name}");
            return;
        }

        string localized = StringManager.Instance.GetText(key);

        if (tmpText != null) tmpText.text = localized;
        if (uiText != null) uiText.text = localized;
    }

    private void OnDestroy()
    {
        if (StringManager.Instance != null)
            StringManager.Instance.OnStringsLoadedEvent -= UpdateText;
    }
}
