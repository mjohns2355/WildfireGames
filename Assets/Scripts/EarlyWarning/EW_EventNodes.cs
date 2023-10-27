using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface EW_EventNode
{
    public EW_EventNode Next();
    public void Play();
    public void SetNext(EW_EventNode next);
}

public class EW_MoveEvent : EW_EventNode
{
    List<EW_MoveCommand> moves;
    EW_EventNode nextNode;

    public EW_MoveEvent(EW_Actor actor, Vector2 targetPos, float duration)
    {
        moves = new List<EW_MoveCommand>();
        Debug.Log("moveevent constructor");
        moves.Add(new EW_MoveCommand(actor, targetPos, duration));
    }

    public EW_EventNode Next()
    {
        return nextNode;
    }

    public void Play()
    {
        Debug.Log("Play");
        moves.First().Execute();
    }

    public void SetNext(EW_EventNode next)
    {
        nextNode = next;
    }
}

public class EW_MoveCommand
{
    public Vector2 targetPosition;
    public float duration;
    public EW_Actor actor;

    public EW_MoveCommand(EW_Actor _actor, Vector2 _targetPos, float _dur)
    {
        actor = _actor;
        targetPosition = _targetPos;
        duration = _dur;
    }

    public void Execute()
    {
        Debug.Log("movecommand");
        actor.Execute(this);
    }
}