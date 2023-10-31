using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    public bool executing = false;
    Coroutine curMove;
    Vector2 targetPos;
    float moveSpeed = 2;

    public void Execute(EW_MoveCommand command)
    {
        if (executing)
        {
            transform.position = targetPos;
            StopCoroutine(curMove);
        }
        Debug.Log("Executing move command");
        curMove = StartCoroutine(MoveTo(command.targetPosition));
    }

    private IEnumerator MoveTo(Vector2 targetPosition)
    {
        executing = true;
        targetPos = targetPosition;
        Vector2 initialPosition = transform.position;
        float distance = Vector2.Distance(initialPosition, targetPosition);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Make sure we hit it exactly
        transform.position = targetPosition;

        executing = false;
        curMove = null;
    }
}
