using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    private Queue<EW_MoveCommand> moveQueue = new Queue<EW_MoveCommand>();
    private bool executing = false;

    // Start is called before the first frame update
    void Start()
    {
        EnqueueMoveCommand(3, 2, 1);
        EnqueueMoveCommand(0, 2, 1.5f);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(ExecuteWholeQueue());
        }
    }

    public void EnqueueMoveCommand(float x, float y, float duration)
    {
        Vector2 targetPosition = new Vector2(x, y);
        EW_MoveCommand moveCommand = new EW_MoveCommand(targetPosition, duration);
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
            EW_MoveCommand nextMove = moveQueue.Dequeue();
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


public class EW_MoveCommand
{
    public Vector2 TargetPosition;
    public float Duration;

    public EW_MoveCommand(Vector2 targetPosition, float duration)
    {
        TargetPosition = targetPosition;
        Duration = duration;
    }
}