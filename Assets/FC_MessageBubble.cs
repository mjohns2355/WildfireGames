using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FC_MessageBubble : MonoBehaviour
{
    public TextMeshProUGUI messageText,nameText;
    public HorizontalLayoutGroup layoutGroup;
    public RectTransform background;
    public Image backgroundImage;
    public Sprite npcSprite, playerSprite;
    public GameObject namePlate,sendButton;
    public Button messageBox;
    public CanvasGroup canvasGroup;
    //[SerializeField] string message;
    //[SerializeField] bool isSentByUser;

    [Header("Settings")]
    public float minWidth = 200f; // Minimum width for short messages
    public float maxWidth = 500f; // Maximum width before wrapping
    public float padding = 20f;   // Padding inside the background
    public void SetupMessage(string message, string name, bool isSentByUser)
    {
        if (isSentByUser)
        {
            // Align to the right
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            backgroundImage.color = new Color(0.19f, 0.56f, 0.93f);
            namePlate.SetActive(false);
        }
        else
        {
            // Align to the left
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            backgroundImage.color = new Color(0.9f, 0.9f, 0.9f);
            nameText.text = name;
        }
        messageText.text = message;
        UpdateBackgroundSize();
    }

    public void SetupOptionButton(string optionText)
    {
        layoutGroup.childAlignment = TextAnchor.LowerRight;
        messageText.text = optionText;
        messageBox.interactable = true;
        namePlate.SetActive(false);
        sendButton.SetActive(true);
        UpdateBackgroundSize();
    }
    private void UpdateBackgroundSize()
    {
        // Get the preferred width and height for the text
        Vector2 preferredValues = messageText.GetPreferredValues(messageText.text);

        // Calculate the new width and height with padding
        float newWidth = preferredValues.x + padding * 2;
        newWidth = Mathf.Clamp(newWidth, minWidth, maxWidth);

        // Force the TextMeshPro to wrap within the max width
        messageText.rectTransform.sizeDelta = new Vector2(newWidth - padding * 2, preferredValues.y);

        // Calculate the new height after text wrapping
        Vector2 wrappedSize = messageText.GetPreferredValues(messageText.text, newWidth - padding * 2, Mathf.Infinity);
        float newHeight = wrappedSize.y + padding * 2;

        // Update the background size
        background.sizeDelta = new Vector2(newWidth, newHeight);

        ResetTextPosition();
    }

    private void ResetTextPosition()
    {
        // Align Text to Top Left
        messageText.rectTransform.pivot = new Vector2(0, 1);
        messageText.rectTransform.anchorMin = new Vector2(0, 1);
        messageText.rectTransform.anchorMax = new Vector2(0, 1);

        // Compensate for internal padding
        float textOffsetX = 3f; 
        float textOffsetY = -3f;

        // Apply the offset
        messageText.rectTransform.anchoredPosition = new Vector2(padding + textOffsetX, -padding + textOffsetY);

        // Force TextMeshPro to recalculate layout
        messageText.ForceMeshUpdate();
    }
}


