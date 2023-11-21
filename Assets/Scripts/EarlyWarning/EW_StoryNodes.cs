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
        // Debug.Log("Advancing move node. CurNode: " + id + " Moves left: " + moves.Count + "  nextNode: " + nextNode);
        if (moves.Count > 0)
        {
            return id;
        }
        return nextNode;
    }

    public override void Play()
    {
        DequeueMove();
    }

    private void DequeueMove()
    {
        if (moves.Count == 0)
        {
            return;
        }
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
    Queue<string> linesLeft;
    List<string> allLines;

    public EW_DialogueNode(List<string> _lines)
    {
        allLines = new List<string>(_lines);
        linesLeft = new Queue<string>(_lines);
    }

    public override int Advance()
    {
        // Debug.Log("Advancing dialogue node. CurNode: " + id + " Lines left: " + lines.Count + "  nextNode: " + nextNode);
        if (linesLeft.Count > 0)
        {
            return id;
        }
        return nextNode;
    }

    public override void Play()
    {
        // Debug.Log("Playing dialogue. Current Line: " + lines.Peek() + " Lines: " + lines.ToArray().ToString());
        ReadLine();
    }

    private void ReadLine()
    {
        string line = linesLeft.Dequeue();
        //Debug.Log(line);
        EW_EventSystem.InvokeTriggerDialogueEvent(line);
    }

    public override void Enter()
    {
        if (linesLeft.Count == 0)
        {
            linesLeft = new Queue<string>(allLines);
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
        //The function that creates a Choice provides a function for the choice to make happen
        //The scene manager handles the logic of selecting a choice
        return id;
    }

    public override void Play()
    {
        EW_EventSystem.InvokeChoiceSetupEvent(choices);
    }
}

public class EW_Choice
{
    public string text { get; }
    public int goesTo;

    public EW_Choice(string _text, int _goesTo)
    {
        text = _text;
        goesTo = _goesTo;
    }
}

public class EW_MoveCommand
{
    public Vector2 targetPosition;
    public EW_Actor actor;
    private bool final;

    public EW_MoveCommand(string _actorName, Vector2 _targetPos, bool final = false)
    {
        actor = GameObject.Find(_actorName).GetComponent<EW_Actor>();
        if (actor == null)
        {
            Debug.LogError("Invalid actor name found in json!");
        }
        targetPosition = _targetPos;
        this.final = false;
    }

    public bool Execute()
    {
        actor.Execute(this);
        return final;
    }
}