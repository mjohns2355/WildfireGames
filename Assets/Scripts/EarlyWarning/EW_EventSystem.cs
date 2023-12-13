using System;
using System.Collections.Generic;
using UnityEngine;

public class EW_EventSystem
{
    //Events are used for indirect communication between objects
    //When something happens, it sends out an event, 
    //and any object that is listening for that event will respond

    public static event Action EndMoveEvent;

    public delegate void DialogueDelegate(EW_DialogueLine line);
    public static event DialogueDelegate TriggerDialogueEvent, SkipDialogueEvent;

    public static event Action LeaveStoryNodeEvent;

    public delegate void ChoicesDelegate(List<EW_Choice> choices);
    public static event ChoicesDelegate ChoiceSetupEvent;

    public delegate void ChangeStoryNodeDelegate(int node);
    public static event ChangeStoryNodeDelegate ChangeStoryNodeEvent;

    //Called in SceneManager.Start to make sure leftover subscriptions are cleared if you are reentering the scene
    public static void Clear()
    {
        EndMoveEvent = null;
        TriggerDialogueEvent = null;
        SkipDialogueEvent = null;
        LeaveStoryNodeEvent = null;
        ChoiceSetupEvent = null;
        ChangeStoryNodeEvent = null;
    }

    public static void InvokeChangeStoryNodeEvent(int node)
    {
        ChangeStoryNodeEvent?.Invoke(node);
    }

    //Send out a message when a node is finished, so if it finished early we can wrap things up that are still in process
    public static void InvokeLeaveStoryNodeEvent()
    {
        LeaveStoryNodeEvent?.Invoke();
    }

    //Send out a message when dialogue is started
    public static void InvokeTriggerDialogueEvent(EW_DialogueLine line)
    {
        TriggerDialogueEvent?.Invoke(line);
    }

    //Send out a message when dialogue is skipped
    public static void InvokeSkipDialogueEvent(EW_DialogueLine line)
    {
        SkipDialogueEvent?.Invoke(line);
    }

    //Send out a message when we enter a choice node
    public static void InvokeChoiceSetupEvent(List<EW_Choice> choices)
    {
        ChoiceSetupEvent?.Invoke(choices);
    }

    //Send out a message when an actor is finished moving from one waypoint to another
    public static void InvokeEndMoveEvent()
    {
        EndMoveEvent?.Invoke();
    }
}