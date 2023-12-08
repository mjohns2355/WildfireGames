using System.Collections.Generic;
using UnityEngine;
public class EW_StoryFunctions
{
    public static EW_SceneManager sceneManager;
    public static EW_UIManager uiManager;

    private static void useUpTime(int time)
    {
        EW_SceneManager.minutesRemaining -= time;
        uiManager.updateTimer(EW_SceneManager.minutesRemaining);

        if (EW_SceneManager.minutesRemaining <= 0)
        {
            EW_EventSystem.LeaveStoryNodeEvent += sceneManager.TimesUp;
        }
    }

    public static void Use30Minutes()
    {
        useUpTime(30);
    }

    public static void ShowTimer()
    {
        useUpTime(0);
    }

    public static void ShowImage(string imageName)
    {
        sceneManager.background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/" + imageName);
    }

    public static void ShowImage1()
    {
        Sprite sprite = Resources.Load<Sprite>("EarlyWarning/Art/Slide2");
        Debug.Log(sprite);
        sceneManager.background.sprite = sprite;
    }

    public static void ShowImage2()
    {
        sceneManager.background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/IntroDraft2");
    }

    public static void ShowImage3()
    {
        sceneManager.background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/IntroDraft3");
    }

    public static void ShowImage4()
    {
        sceneManager.background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/Slide1");
    }

    public static void GoBag()
    {
        EW_SceneManager.goBag = true;
        useUpTime(30);
    }

    public static void AldoTalk()
    {
        EW_SceneManager.neighborTalk = true;
        useUpTime(30);
    }

    public static void CutLawn()
    {
        EW_SceneManager.cutLawn = true;
        useUpTime(30);
    }

    public static void CutTree()
    {
        EW_SceneManager.cutTree = true;
        useUpTime(30);
    }

    public static void CleanGutters()
    {
        EW_SceneManager.cleanedGutters = true;
        useUpTime(30);
    }

    public static void MakeBreakfast()
    {
        EW_SceneManager.madeBreakfast = true;
        useUpTime(30);
    }

    public static void GoToArea(string areaName)
    {
        sceneManager.GoToArea(areaName);
    }
}