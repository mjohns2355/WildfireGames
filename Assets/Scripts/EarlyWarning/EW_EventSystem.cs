using System;
using System.Collections.Generic;

public static class EW_EventSystem
{
    public delegate void DialogueDelegate(string line);
    public static event DialogueDelegate TriggerDialogueEvent, SkipDialogueEvent;

    public delegate void StoryNodeDelegate(EW_StoryNode node);
    public static event StoryNodeDelegate ChangeStoryNodeEvent;
    public static event Action LeaveStoryNodeEvent;

    public delegate void ChoicesDelegate(List<EW_Choice> choices);
    public static event ChoicesDelegate ChoiceSetupEvent;

    public static void InvokeChangeStoryNodeEvent(EW_StoryNode node)
    {
        ChangeStoryNodeEvent?.Invoke(node);
    }

    /// <summary>
    /// We send out a message when a node is finished, so if it finished early we can wrap things up that are still in process
    /// </summary>
    public static void InvokeLeaveStoryNodeEvent()
    {
        LeaveStoryNodeEvent?.Invoke();
    }

    public static void InvokeTriggerDialogueEvent(string line)
    {
        TriggerDialogueEvent?.Invoke(line);
    }

    public static void InvokeSkipDialogueEvent(string line)
    {
        SkipDialogueEvent?.Invoke(line);
    }

    public static void InvokeChoiceSetupEvent(List<EW_Choice> choices)
    {
        ChoiceSetupEvent?.Invoke(choices);
    }
}