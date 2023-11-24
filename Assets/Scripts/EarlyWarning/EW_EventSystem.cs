using System;
using System.Collections.Generic;

public static class EW_EventSystem
{
    public delegate void DialogueDelegate(EW_DialogueLine line);
    public static event DialogueDelegate TriggerDialogueEvent, SkipDialogueEvent;

    public delegate void StoryNodeDelegate(EW_StoryNode node);
    public static event Action LeaveStoryNodeEvent;

    public delegate void ChoicesDelegate(List<EW_Choice> choices);
    public static event ChoicesDelegate ChoiceSetupEvent;

    public delegate void ChangeStoryNodeDelegate(int node);
    public static event ChangeStoryNodeDelegate ChangeStoryNodeEvent;

    public static void InvokeChangeStoryNodeEvent(int node)
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

    public static void InvokeTriggerDialogueEvent(EW_DialogueLine line)
    {
        TriggerDialogueEvent?.Invoke(line);
    }

    public static void InvokeSkipDialogueEvent(EW_DialogueLine line)
    {
        SkipDialogueEvent?.Invoke(line);
    }

    public static void InvokeChoiceSetupEvent(List<EW_Choice> choices)
    {
        ChoiceSetupEvent?.Invoke(choices);
    }
}