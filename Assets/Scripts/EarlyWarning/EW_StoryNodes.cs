using System.Collections.Generic;
using UnityEngine;

public interface EW_StoryNode
{
    public int Advance();
    public void Play();
    public void SetNext(int next);
    public void setID(int value);
}

public class EW_MoveNode : EW_StoryNode
{
    Queue<EW_MoveCommand> moves;
    int id, nextNode;
    EW_SceneManager manager;

    public EW_MoveNode(EW_SceneManager _manager, Queue<EW_MoveCommand> commands)
    {
        manager = _manager;
        moves = commands;
    }

    public int Advance()
    {
        // Debug.Log("Advancing move node. CurNode: " + id + " Moves left: " + moves.Count + "  nextNode: " + nextNode);
        if (moves.Count > 0)
        {
            return id;
        }
        return nextNode;
    }

    public void Play()
    {
        DequeueMove();
    }

    public void SetNext(int next)
    {
        nextNode = next;
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

    public int getID()
    {
        return id;
    }

    public void setID(int value)
    {
        id = value;
    }
}

public class EW_DialogueNode : EW_StoryNode
{
    Queue<string> lines; //This might change to a more complex type later
    int id, nextNode;

    public EW_DialogueNode(List<string> _lines)
    {
        lines = new Queue<string>(_lines);
    }

    public int Advance()
    {
        // Debug.Log("Advancing dialogue node. CurNode: " + id + " Lines left: " + lines.Count + "  nextNode: " + nextNode);
        if (lines.Count > 0)
        {
            return id;
        }
        return nextNode;
    }

    public void Play()
    {
        // Debug.Log("Playing dialogue. Current Line: " + lines.Peek() + " Lines: " + lines.ToArray().ToString());
        ReadLine();
    }

    private void ReadLine()
    {
        string line = lines.Dequeue();
        Debug.Log(line);
        EW_EventSystem.InvokeTriggerDialogueEvent(line);
    }

    public void SetNext(int next)
    {
        nextNode = next;
    }

    public int getID()
    {
        return id;
    }

    public void setID(int value)
    {
        id = value;
    }
}

public class EW_ChoiceNode : EW_StoryNode
{
    List<EW_Choice> choices;
    int id;

    public EW_ChoiceNode(List<EW_Choice> choices)
    {
        this.choices = choices;
    }

    public int Advance()
    {
        //A choice node essentially does not implement Advance
        //The function that creates a Choice provides a function for the choice to make happen
        //The scene manager handles the logic of selecting a choice
        return id;
    }

    public void Play()
    {
        EW_EventSystem.InvokeChoiceSetupEvent(choices);
    }

    public void SetNext(int next)
    {
        //SetNext similarly does nothing, since there are sort of multiple next nodes
        return;
    }

    public int getID()
    {
        return id;
    }

    public void setID(int value)
    {
        id = value;
    }
}

public class EW_Choice
{
    public string text { get; }
    public System.Action onSelect { get; }
    public EW_StoryNode nextNode;

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