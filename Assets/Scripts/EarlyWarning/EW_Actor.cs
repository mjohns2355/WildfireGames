using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    public bool executing = false;
    Coroutine curMove;
    Vector2 targetPos;

    public void Execute(EW_MoveCommand command)
    {
        if (executing)
        {
            transform.position = targetPos;
            StopCoroutine(curMove);
        }
        Debug.Log("Executing move command");
        curMove = StartCoroutine(MoveTo(command.targetPosition, command.duration));
    }

    private IEnumerator MoveTo(Vector2 targetPosition, float duration)
    {
        executing = true;
        targetPos = targetPosition;

        Vector2 initialPosition = transform.position;
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
