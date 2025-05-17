using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HouseIcon : MonoBehaviour
{
    public HouseType iconHouseType;
    public ATC_DialogTree houseDialog;
    [SerializeField] List<Sprite> lockedSprites;
    [SerializeField] List<Sprite> unlockedSprites;
    [SerializeField] Sprite followed, disregarded;
    [SerializeField] Button button;
    [SerializeField] Image image;


    Sprite lockedSprite;
    Sprite unlockedSprite;
    
    public void InitIcon(HouseType houseType)
    {

        iconHouseType = houseType;
        foreach (var sprite in lockedSprites)
        {
            if (sprite.name == houseType.ToString())
            {
               lockedSprite = sprite;
            }
        }

        foreach (var sprite in unlockedSprites)
        {
            var name = sprite.name.Replace("Unlocked", "");
            if(name == houseType.ToString())
            {
                unlockedSprite = sprite;
            }
        }

        //houseDialog = GameManager.Instance.houseDialogs[0];
        ToggleIconState(true);

    }

    public void ToggleIconState(bool isLocked)
    {

        //Debug.Log("Toggle" + iconHouseType + "'s State to " + isLocked);
        if (isLocked)
        {
            SetIconSprite(lockedSprite);
        }
        else
        {
            SetIconSprite(unlockedSprite);
        }
    }

    public void ToggleIconFollowedState(bool isFollowed)
    {
        button.interactable = false;
        if (isFollowed)
        {
            SetIconSprite(followed);
        }
        else
        {
            SetIconSprite(disregarded);
        }
    }
    void SetIconSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void AddOnClickActions(UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    public void RemoveOnClickAction(UnityAction action)
    {
        button.onClick.RemoveListener(action);
    }

    private void OnEnable()
    {
        button.interactable = true;
    }
}
