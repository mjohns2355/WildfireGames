using System.Collections;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    public bool executing = false;
    Coroutine curMove;
    Vector2 targetPos;
    float moveSpeed = 4;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite sideSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 previousPosition;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movementDirection = currentPosition - previousPosition;

        if (movementDirection != Vector3.zero)
        {
            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.y))
            {
                spriteRenderer.sprite = sideSprite;
                if (movementDirection.x < 0)
                    spriteRenderer.flipX = true;
                else
                    spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.sprite = (movementDirection.y > 0) ? upSprite : downSprite;
            }
        }
    }


    public void Execute(EW_MoveCommand command)
    {
        SkipCurrentMove();
        EW_EventSystem.LeaveStoryNodeEvent += SkipCurrentMove;
        curMove = StartCoroutine(Move(command.deltaPos));
    }

    private void SkipCurrentMove()
    {
        if (executing)
        {
            transform.position = targetPos;
            StopCoroutine(curMove);
            executing = false;
            curMove = null;
        }
        EW_EventSystem.LeaveStoryNodeEvent -= SkipCurrentMove;
    }

    private IEnumerator Move(Vector2 deltaPosition)
    {
        Debug.Log("Move()");
        executing = true;
        targetPos = (Vector2)transform.position + deltaPosition;
        Vector2 initialPosition = transform.position;
        float distance = Vector2.Distance(initialPosition, targetPos);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        Debug.LogFormat("Moving from {0} to {1} in {2} seconds", initialPosition, targetPos, duration);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            previousPosition = transform.position;
            transform.position = Vector2.Lerp(initialPosition, targetPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Make sure we hit it exactly
        transform.position = targetPos;

        EW_EventSystem.LeaveStoryNodeEvent -= SkipCurrentMove;
        executing = false;
        curMove = null;
        EW_EventSystem.InvokeEndMoveEvent();
    }
}
