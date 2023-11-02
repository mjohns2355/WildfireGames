using System.Collections.Generic;
using UnityEngine;

public interface EW_StoryNode
{
    public EW_StoryNode Advance();
    public void Play();
    public void SetNext(EW_StoryNode next);
}

public class EW_MoveNode : EW_StoryNode
{
    Queue<EW_MoveCommand> moves;
    EW_StoryNode nextNode;
    EW_SceneManager manager;

    public EW_MoveNode(EW_SceneManager _manager, Queue<EW_MoveCommand> commands)
    {
        manager = _manager;
        moves = commands;
        Debug.Log("moveEvent constructor");
    }

    public EW_StoryNode Advance()
    {
        Debug.Log("Advance");
        if (moves.Count > 0)
        {
            Play();
            return this;
        }
        nextNode?.Play();
        return nextNode;
    }

    public void Play()
    {
        Debug.Log("Play");
        DequeueMove();
    }

    public void SetNext(EW_StoryNode next)
    {
        nextNode = next;
    }

    private void DequeueMove()
    {
        var move = moves.Dequeue();
        bool wasFinal = move.Execute();
        if (wasFinal)
        {
            manager.ChangeStoryNode(nextNode);
        }
    }
}

public class EW_DialogueNode : EW_StoryNode
{
    Queue<string> lines; //This might change to a more complex type later
    EW_StoryNode nextNode;

    public EW_DialogueNode(Queue<string> lines)
    {
        this.lines = lines;
    }

    public EW_StoryNode Advance()
    {
        Debug.Log("Advance");
        if (lines.Count > 0)
        {
            ReadLine();
            return this;
        }
        nextNode?.Play();
        return nextNode;
    }

    public void Play()
    {
        Debug.Log("Play");
        ReadLine();
    }

    private void ReadLine()
    {
        string line = lines.Dequeue();
        Debug.Log(line);
        EW_EventSystem.InvokeTriggerDialogueEvent(line);
    }

    public void SetNext(EW_StoryNode next)
    {
        nextNode = next;
    }
}

public class EW_ChoiceNode : EW_StoryNode
{
    List<EW_Choice> choices;

    public EW_ChoiceNode(List<EW_Choice> choices)
    {
        this.choices = choices;
    }

    public EW_StoryNode Advance()
    {
        //A choice node essentially does not implement Advance
        //The function that creates a Choice provides a function for the choice to make happen
        //The scene manager handles the logic of selecting a choice
        return this;
    }

    public void Play()
    {
        EW_EventSystem.InvokeChoiceSetupEvent(choices);
    }

    public void SetNext(EW_StoryNode next)
    {
        //SetNext similarly does nothing for , since there are multiple next nodes
        Debug.LogError("SetNext called on choice node!");
        return;
    }
}

public class EW_Choice
{
    public string text { get; }
    public System.Action onSelect { get; }

    public EW_Choice(string _text, System.Action _onSelect)
    {
        text = _text;
        onSelect = _onSelect;
    }
}

public class EW_MoveCommand
{
    public Vector2 targetPosition;
    public EW_Actor actor;
    private bool final;

    public EW_MoveCommand(EW_Actor _actor, Vector2 _targetPos, bool final = false)
    {
        actor = _actor;
        targetPosition = _targetPos;
        this.final = false;
    }

    public bool Execute()
    {
        Debug.Log("movecommand");
        actor.Execute(this);
        return final;
    }
}