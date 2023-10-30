using System.Collections.Generic;
using UnityEngine;

public interface EW_StoryNode
{
    public EW_StoryNode Next();
    public void Play();
    public void SetNext(EW_StoryNode next);
}

public class EW_MoveEvent : EW_StoryNode
{
    Queue<EW_MoveCommand> moves;
    EW_StoryNode nextNode;
    EW_SceneManager manager;

    public EW_MoveEvent(EW_SceneManager _manager, Queue<EW_MoveCommand> commands)
    {
        manager = _manager;
        moves = commands;
        Debug.Log("moveEvent constructor");
    }

    public EW_StoryNode Next()
    {
        if (moves.Count > 0)
        {
            return this;
        }
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
            manager.MoveToNode(nextNode);
        }
    }
}

public class EW_DialogueEvent : EW_StoryNode
{
    Queue<string> lines; //This might change to a more complex type later
    EW_StoryNode nextNode;

    public EW_DialogueEvent(Queue<string> lines)
    {
        this.lines = lines;
    }

    public EW_StoryNode Next()
    {
        if (lines.Count > 0)
        {
            ReadLine();
            return this;
        }
        return nextNode;
    }

    public void Play()
    {
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
