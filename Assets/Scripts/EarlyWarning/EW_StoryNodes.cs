using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EW_StoryNode
{
    public int id, nextNode;
    protected Action enterAction;
    public abstract int Advance();
    public abstract void Play();
    public void SetEnterFunction(Action function)
    {
        enterAction = function;
    }
    public virtual void Enter()
    {
        Debug.Log("Entering node");
        if (enterAction != null)
        {
            enterAction();
        }
    }
}

public class EW_MoveNode : EW_StoryNode
{
    Queue<EW_MoveCommand> moves;
    EW_SceneManager manager;

    public EW_MoveNode(EW_SceneManager _manager, Queue<EW_MoveCommand> commands)
    {
        manager = _manager;
        moves = commands;
    }

    public override int Advance()
    {
        if (moves.Count > 0)
        {
            return id;
        }
        return nextNode;
    }

    public override void Play()
    {
        Debug.Log("playing move node");
        DequeueMove();
    }

    private void DequeueMove()
    {
        Debug.Log("Dequeueing move");
        if (moves.Count == 0)
        {
            return;
        }
        var move = moves.Dequeue();
        bool wasFinal = move.Execute();
        if (wasFinal)
        {
            EW_EventSystem.EndMoveEvent -= DequeueMove;
            EW_EventSystem.EndMoveEvent += FinishNode;
        }
    }

    private void FinishNode()
    {
        EW_EventSystem.EndMoveEvent -= FinishNode;
        manager.ChangeStoryNode(nextNode);
    }

    public override void Enter()
    {
        Debug.Log("Entering move node");
        if (enterAction != null)
        {
            enterAction();
        }
        EW_EventSystem.EndMoveEvent += DequeueMove;
    }
}

public class EW_DialogueNode : EW_StoryNode
{
    Queue<EW_DialogueLine> linesLeft;
    List<EW_DialogueLine> allLines;

    public EW_DialogueNode(List<EW_DialogueLine> _lines)
    {
        allLines = new List<EW_DialogueLine>(_lines);
        linesLeft = new Queue<EW_DialogueLine>(_lines);
    }

    public override int Advance()
    {
        if (linesLeft.Count > 0)
        {
            return id;
        }
        return nextNode;
    }

    public override void Play()
    {
        ReadLine();
    }

    private void ReadLine()
    {
        EW_DialogueLine line = linesLeft.Dequeue();
        EW_EventSystem.InvokeTriggerDialogueEvent(line);
    }

    public override void Enter()
    {
        if (linesLeft.Count == 0) //Allows for repeating dialogue nodes
        {
            linesLeft = new Queue<EW_DialogueLine>(allLines);
        }
        if (enterAction != null)
        {
            enterAction();
        }
    }
}

public class EW_ChoiceNode : EW_StoryNode
{
    List<EW_Choice> choices;

    public EW_ChoiceNode(List<EW_Choice> choices)
    {
        this.choices = choices;
    }

    public override int Advance()
    {
        //A choice node essentially does not implement Advance
        //The scene manager handles the logic of selecting a choice
        return id;
    }

    public override void Play()
    {
        EW_EventSystem.InvokeChoiceSetupEvent(choices);
    }
}

//These two classes don't need constructors, since they are serialized directly from JSON
[Serializable]
public class EW_Choice
{
    public string text;
    public int goesTo;
    public bool repeatable = false;
}

[Serializable]
public class EW_DialogueLine
{
    public string text;
    public string speaker = "";
    public bool important = false;

    public EW_DialogueLine(string _text, string _speaker = "", bool _important = false)
    {
        text = _text;
        speaker = _speaker;
        important = _important;
    }
}

public class EW_MoveCommand
{
    public Vector2 deltaPos;
    public string actorName;
    private bool final;

    public EW_MoveCommand(string _actorName, Vector2 _deltaPos, bool _final = false)
    {
        actorName = _actorName;
        deltaPos = _deltaPos;
        final = _final;
    }

    public bool Execute()
    {
        EW_Actor actor = GameObject.Find(actorName).GetComponent<EW_Actor>();
        if (actor == null)
        {
            Debug.LogError("Invalid actor name " + actorName + " attached to move command!");
        }
        actor.Execute(this);
        return final;
    }
}