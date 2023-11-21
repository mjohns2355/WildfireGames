using System;
using UnityEngine;

public class EW_StoryFunctions
{
    public static EW_SceneManager sceneManager;
    public static EW_UIManager uiManager;

    public void useUpTime(int time)
    {
        EW_SceneManager.minutesRemaining -= time;
        uiManager.updateTimer(EW_SceneManager.minutesRemaining);
    }

    public void ShowTimer()
    {
        useUpTime(0);
    }

    public void HandleWakeUp()
    {
        sceneManager.background.sprite = sceneManager.livingRoomSprite;
        sceneManager.paulSprite.enabled = true;
    }

    public void HandleBreakfast()
    {
        useUpTime(30);
        sceneManager.ChangeStoryNode(7);
    }

    public void HandleGoBag()
    {
        useUpTime(30);
        EW_EventSystem.InvokeChangeStoryNodeEvent(9);
    }

    public void HandleCheckYard()
    {
        Debug.Log("Paul checks the yard");
        EW_EventSystem.InvokeChangeStoryNodeEvent(11);
    }

    public void HandleCheckBackyard()
    {
        Debug.Log("Paul checks the backyard");
        EW_EventSystem.InvokeChangeStoryNodeEvent(19);
    }

    public void HandleCheckFrontyard()
    {
        Debug.Log("Paul checks the frontyard");
        EW_EventSystem.InvokeChangeStoryNodeEvent(13);
    }

    public void HandleCheckDownstairs()
    {
        Debug.Log("Paul checks downstairs");
        EW_EventSystem.InvokeChangeStoryNodeEvent(5);
    }

    public void HandleCutLawn()
    {
        useUpTime(30);
        Debug.Log("Paul cuts the lawn");
        EW_EventSystem.InvokeChangeStoryNodeEvent(15);
    }

    public void HandleAldoTalk()
    {
        useUpTime(30);
        Debug.Log("Paul talks to Aldo");
        EW_EventSystem.InvokeChangeStoryNodeEvent(17);
    }

    public void HandleCutTree()
    {
        useUpTime(30);
        Debug.Log("Paul cuts the tree");
        EW_EventSystem.InvokeChangeStoryNodeEvent(-1);
    }

    public void HandleCleanGutter()
    {
        useUpTime(30);
        Debug.Log("Paul cleans the gutter");
        EW_EventSystem.InvokeChangeStoryNodeEvent(-1);
    }
}