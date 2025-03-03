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
    public Sprite playerBGSpriteShort, playerBGSpriteLong, choiceSprite, fireFighterSprite, fireFighterSpriteShort;
    public GameObject namePlate,sendButton;
    public Button messageBox;
    public CanvasGroup canvasGroup;
    //[SerializeField] string message;
    //[SerializeField] bool isSentByUser;

    [Header("Settings")]
    public float minWidth = 200f; // Minimum width for short messages
    public float maxWidth = 500f; // Maximum width before wrapping
    public float padding = 20f;   // Padding inside the background
    public float tailHeight = 30f;  // speech bubble tail height

    private float textWidth, textHeight;
    private bool isSentByUser, isOption = false;
    public void SetupMessage(string message, string name, bool isSentByUser)
    {
        this.isSentByUser = isSentByUser;
        messageText.text = message;
        UpdateBackgroundSize();

        if (isSentByUser)
        {

            // Align to the right
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            //backgroundImage.color = new Color(255, 204, 0);
            namePlate.SetActive(false);
        }
        else
        {
            
            if(name == "Mary"){
                backgroundImage.color = Color.magenta;

            }
            // Align to the left
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
           
            nameText.text = name;
        }
        SetBackgroundImageSprite();
    }

    public void SetupOptionButton(string optionText)
    {
        isOption = true;
        tailHeight = 0;
        messageText.text = optionText;
        UpdateBackgroundSize();
        layoutGroup.childAlignment = TextAnchor.LowerRight;

        messageBox.interactable = true;
        namePlate.SetActive(false);
        sendButton.SetActive(true);
        SetBackgroundImageSprite();
    }

    private void SetBackgroundImageSprite()
    {
        //if (textWidth < minWidth && textWidth < 120f)
        //{
        //    if (isSentByUser)
        //    {
        //        backgroundImage.sprite = playerBGSpriteShort;
        //    }

        //}
        //else
        //{
        //    if (isSentByUser)
        //    {
        //        backgroundImage.sprite = playerBGSpriteLong;
        //    }

        //}

        if (isSentByUser)
        {
            if (textWidth < minWidth || textHeight < 130f)
            {
                Debug.Log("Short");
                backgroundImage.sprite = playerBGSpriteShort;
            }
            else
            {
                backgroundImage.sprite = playerBGSpriteLong;
            }

            
        }

        if (isOption)
        {
            backgroundImage.sprite = choiceSprite;
        }

        else if(isSentByUser == false)
        {
            if (textWidth < minWidth || textHeight < 130f)
            {

                backgroundImage.sprite = fireFighterSpriteShort;
            }
            else
            {
                backgroundImage.sprite = fireFighterSprite;
            }

                
        }
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
        float newHeight = wrappedSize.y + padding * 2 + tailHeight;

        // Update the background size
        background.sizeDelta = new Vector2(newWidth, newHeight);
        textHeight = newHeight;
        textWidth = newWidth;
        Debug.Log(newHeight);
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
        // Offset the text upwards to compensate for the tail
        if(tailHeight != 0)
        {
            textOffsetY = -padding + tailHeight / 2;
        }

        // Apply the offset
        messageText.rectTransform.anchoredPosition = new Vector2(padding + textOffsetX, -padding + textOffsetY);

        // Force TextMeshPro to recalculate layout
        messageText.ForceMeshUpdate();
    }
}


