using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EW_StoryNode
{
    public int id, nextNode;
    protected Action enterAction;
    //Advance() returns the ID of the node to be played on tap. If the node is not finished, it returns its own ID.
    public abstract int Advance();
    //Play() is called when the node is first entered AND any time the player taps the screen, regardless of whether the node is finished
    public abstract void Play();
    public void SetEnterFunction(Action function)
    {
        enterAction = function;
    }
    //EnterAction is called when the node is first entered, and is used to call StoryFunctions
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
            EW_EventSystem.EndMoveEvent -= DequeueMove;
            EW_EventSystem.EndMoveEvent += FinishNode;
        }
    }

    private void FinishNode()
    {
        EW_EventSystem.EndMoveEvent -= FinishNode;
        manager.ChangeStoryNode(nextNode);
        manager.curNode.Play();
    }

    public override void Enter()
    {
        if (enterAction != null)
        {
            enterAction();
        }
        EW_EventSystem.EndMoveEvent += DequeueMove;
    }
}

public class EW_DialogueNode : EW_StoryNode
{
    protected Queue<EW_DialogueLine> linesLeft;
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
        //A choice node cannot move to the next node via Advance()
        //The ui manager handles the logic of selecting a choice
        return id;
    }

    public override void Play()
    {
        EW_EventSystem.InvokeChoiceSetupEvent(choices);
    }
}

//This should be fleshed out with support for modularized logic to replace what is currently in ShowEpilogue()
public class EW_EpilogueNode : EW_DialogueNode
{
    public EW_EpilogueNode(List<EW_DialogueLine> _lines) : base(_lines)
    {
    }

    public override int Advance()
    {
        if (linesLeft.Count > 0)
        {
            return -2;
        }
        return nextNode;
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