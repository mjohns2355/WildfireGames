using UnityEngine;
using UnityEngine.UI;

public class TTSButtonConnector : MonoBehaviour
{
    void Start()
    {
        // Try Toggle first
        Toggle toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.isOn = TTSManager.IsEnabled;
            toggle.onValueChanged.AddListener(TTSManager.SetEnabled);
            return;
        }

        // Fall back to Button
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(TTSManager.Toggle);
        }
    }
}
