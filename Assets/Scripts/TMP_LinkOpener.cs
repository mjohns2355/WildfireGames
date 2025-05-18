using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TMP_LinkOpener : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    private Camera uiCamera;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        uiCamera = Camera.main; // Set to your UI camera if needed
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, mousePos, uiCamera);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
                string linkID = linkInfo.GetLinkID();
                Debug.Log("Opening URL: " + linkID);
                Application.OpenURL(linkID);
            }
        }
    }
}
