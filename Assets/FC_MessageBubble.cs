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
    public Image backgroundImage,namePlateBackgrondImage;
    public GameObject namePlate,sendButton;
    public Button messageBox;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public float minWidth = 200f; // Minimum width for short messages
    public float maxWidth = 500f; // Maximum width before wrapping
    public float padding = 20f;   // Padding inside the background
    public float tailHeight = 30f;  // speech bubble tail height
    public float namePlatePadding = 5f; // Extra space for name plate messages
    public float sendButtonPadding = 50f; // Extra space for the send button in option bubbles

    private float textWidth, textHeight;
    private bool isSentByUser, isOption = false;

    public void SetupMessage(string message, string name, bool isSentByUser)
    {

        this.isSentByUser = isSentByUser;
        isOption = false; // Reset option flag
        bool hasNamePlate = !string.IsNullOrEmpty(name);

        messageText.text = message;
        nameText.text = name;
        namePlate.SetActive(hasNamePlate);

        // Align the message depending on the sender
        layoutGroup.childAlignment = isSentByUser ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;

        // Update bubble size & background
        UpdateBackgroundSize(hasNamePlate);
        SetBackgroundImageSprite();
    }

    public void SetupTypingIndicator(bool isSentByUser, string name)
    {
        var config = MessageBubbleConfig.Instance;
        layoutGroup.childAlignment = isSentByUser ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
        messageText.gameObject.SetActive(false);
        Debug.Log(name);
        backgroundImage.sprite = name switch
        {
            "Mary" or "pet" => config.petTypingIndicator,
            "Firefighter" => config.firefighterTypingIndicator,
            "Owner of 2 Cars" => config.twoCarTypingIndicator,
            "Worried Parents" => config.kidsTypingIndicator,
            "WUI Resident" => config.wuiTypingIndicator,
            "Older Resident" => config.elderlyTypingIndicator,
            _ => config.playerTypingIndicator,//Debug.LogWarning($"No matching background found for name: {name}");
        };
    }
    public void SetupOptionButton(string optionText)
    {
        isOption = true;
        tailHeight = 0; // No tail for option bubbles
        messageText.text = optionText;
        layoutGroup.childAlignment = TextAnchor.MiddleRight;
        messageBox.interactable = true;
        namePlate.SetActive(false);
        sendButton.SetActive(true);

        UpdateBackgroundSize(false);
        SetBackgroundImageSprite();
    }

    private void SetBackgroundImageSprite()
    {
        var config = MessageBubbleConfig.Instance;
        if (isOption)
        {
            backgroundImage.sprite = config.choiceSprite;
            return;
        }

        if (isSentByUser)
        {
            backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.playerBGShort : config.playerBGLong;
        }
        else
        {
            var name = nameText.text;
            switch (name)
            {
                case "Mary":
                    backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.petHomeBGShort : config.petHomeBGLong;
                    namePlateBackgrondImage.sprite = config.petHomeNamePlate;
                    break;

                case "Firefighter":
                    backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.firefighterBGShort : config.firefighterBGLong;
                    namePlateBackgrondImage.sprite = config.firefighterNamePlate;
                    break;

                case "Bob":
                    backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.twoCarsHomeBGShort : config.twoCarsHomeBGLong;
                    namePlateBackgrondImage.sprite = config.twoCarsHomeNamePlate;
                    break;

                case "David":
                    backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.kidsHomeBGShort : config.kidsHomeBGLong;
                    namePlateBackgrondImage.sprite = config.kidsHomeNamePlate;
                    break;

                case "Annie":
                    backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.wuiHomeBGShort : config.wuiHomeBGLong;
                    namePlateBackgrondImage.sprite = config.wuiHomeNamePlate;
                    break;

                case "Alice":
                    backgroundImage.sprite = (textWidth < minWidth || textHeight < 130f) ? config.elderlyHomeBGShort : config.elderlyHomeBGLong;
                    namePlateBackgrondImage.sprite = config.elderlyHomeNamePlate;
                    break;

                default:
                    Debug.LogWarning($"No matching background found for name: {name}");
                    break;
            }


        }
    }
    private void UpdateBackgroundSize(bool hasNamePlate)
    {
        // Get the preferred width and height for the text
        Vector2 preferredValues = messageText.GetPreferredValues(messageText.text);

        // Calculate the new width and height with padding
        float extraPadding = isOption ? sendButtonPadding : 0; // Add padding for the send button if it's an option
        float newWidth = preferredValues.x + padding * 2 + extraPadding;
        newWidth = Mathf.Clamp(newWidth, minWidth, maxWidth);
        // Force TextMeshPro to wrap within max width
        messageText.rectTransform.sizeDelta = new Vector2(newWidth - padding * 2 - extraPadding, preferredValues.y);

        // Calculate new height after wrapping
        Vector2 wrappedSize = messageText.GetPreferredValues(messageText.text, newWidth - padding * 2 - extraPadding, Mathf.Infinity);
        float newHeight = wrappedSize.y + padding * 2 + tailHeight;

        // Add extra space if the message has a name plate
        if (hasNamePlate)
        {
            newHeight += namePlatePadding;
        }

        // Apply new size
        background.sizeDelta = new Vector2(newWidth, newHeight);
        textWidth = newWidth;
        textHeight = newHeight;

        ResetTextPosition();

    }

    private void ResetTextPosition()
    {
        messageText.rectTransform.pivot = new Vector2(0, 1);
        messageText.rectTransform.anchorMin = new Vector2(0, 1);
        messageText.rectTransform.anchorMax = new Vector2(0, 1);

        float textOffsetX = 3f;
        float textOffsetY = -3f;
        if (tailHeight != 0)
        {
            textOffsetY = -padding + (tailHeight / 2);
        }
        

        // Adjust for send button in option bubbles
        float xOffset = isOption ? -sendButtonPadding / 2 : 0;

        messageText.rectTransform.anchoredPosition = new Vector2(padding + textOffsetX + xOffset, -padding + textOffsetY);
        messageText.ForceMeshUpdate();
    }

}


