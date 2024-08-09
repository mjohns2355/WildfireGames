using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HouseIcon : MonoBehaviour
{
    public HouseType iconHouseType;
    [SerializeField] List<Sprite> lockedSprites;
    [SerializeField] List<Sprite> unlockedSprites;
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

        ToggleIconState(true);
    }

    public void ToggleIconState(bool isLocked)
    {
        //Debug.Log("Toggle" + iconHouseType + "'s State to " + isLocked);
        if(isLocked)
        {
            SetIconSprite(lockedSprite);
        }
        else
        {
            SetIconSprite(unlockedSprite);
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
}
