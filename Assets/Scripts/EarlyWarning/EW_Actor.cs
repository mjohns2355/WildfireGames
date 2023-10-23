using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    private Queue<MoveCommand> moveQueue = new Queue<MoveCommand>();
    private bool executing = false;

    // Start is called before the first frame update
    void Start()
    {
        EnqueueMoveCommand(3, 2, 1);
        EnqueueMoveCommand(0, 2, 1.5f);
        StartCoroutine(ExecuteWholeQueue());
    }

    public void EnqueueMoveCommand(float x, float y, float duration)
    {
        Vector2 targetPosition = new Vector2(x, y);
        MoveCommand moveCommand = new MoveCommand(targetPosition, duration);
        moveQueue.Enqueue(moveCommand);
    }

    private IEnumerator ExecuteWholeQueue()
    {
        while (moveQueue.Count > 0)
        {
            ExecuteNext();
            yield return new WaitUntil(() => !executing);
        }
    }

    private void ExecuteNext()
    {
        if (moveQueue.Count > 0)
        {
            MoveCommand nextMove = moveQueue.Dequeue();
            StartCoroutine(MoveTo(nextMove.TargetPosition, nextMove.Duration));
        }
    }

    private IEnumerator MoveTo(Vector2 targetPosition, float duration)
    {
        executing = true;

        Vector2 initialPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        executing = false;
    }
}


public class MoveCommand
{
    public Vector2 TargetPosition;
    public float Duration;

    public MoveCommand(Vector2 targetPosition, float duration)
    {
        TargetPosition = targetPosition;
        Duration = duration;
    }
}