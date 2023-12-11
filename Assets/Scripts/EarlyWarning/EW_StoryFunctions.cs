using System.Collections.Generic;
using UnityEngine;

//These are the functions that can be called from the story JSON files
//They must be defined as public and static
//They may take one parameter in the form of a string
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

    public static void ShowEpilogue()
    {
        sceneManager.ShowEpilogue();
    }

    public static void ShowImage(string imageName)
    {
        sceneManager.background.sprite = Resources.Load<Sprite>("EarlyWarning/Art/" + imageName);
    }

    public static void GoBag()
    {
        EW_SceneManager.goBag = true;
        useUpTime(30);
    }

    public static void AldoTalk()
    {
        EW_SceneManager.neighborTalk = true;
        ShowImage("Slide1");
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