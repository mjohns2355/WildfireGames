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
            EW_EventSystem.LeaveStoryNodeEvent += sceneManager.EndPrefirePhase;
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



    public static void GoBag()
    {
        EW_SceneManager.goBag = true;
        Debug.Log("GoBag()_called");
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

    public static void GoToLivingRoom()
    {
        sceneManager.GoToArea("PaulLivingRoom");
    }

    public static void GoToBackYard()
    {
        sceneManager.GoToArea("PaulBackyard");
    }

    public static void GoToFrontYard()
    {
        sceneManager.GoToArea("PaulFrontYard");
    }
}