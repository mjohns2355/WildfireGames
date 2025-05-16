using UnityEngine;
using TMPro;

public class TMP_LinkOpener : MonoBehaviour
{
    private TextMeshProUGUI tmpText;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}
