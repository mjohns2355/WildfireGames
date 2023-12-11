using System.Collections;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    public bool executing = false;
    Coroutine curMove;
    Vector2 targetPos;
    float moveSpeed = 4;

    //These should ideally be replaced with animations
    public Sprite upSprite, downSprite, sideSprite;

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
                spriteRenderer.flipX = movementDirection.x < 0;
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

    // This is called when the player skips a move by tapping the screen
    // Only goes to the next waypoint in the move, so a move node may require multiple taps
    private void SkipCurrentMove()
    {
        if (!executing)
        {
            EW_EventSystem.LeaveStoryNodeEvent -= SkipCurrentMove;
            return;
        }

        transform.position = targetPos;
        StopCoroutine(curMove);
        executing = false;
        curMove = null;
    }

    // Moves the actor to a new position over time
    private IEnumerator Move(Vector2 deltaPosition)
    {
        executing = true;
        targetPos = (Vector2)transform.position + deltaPosition;
        Vector2 initialPosition = transform.position;

        //Calculate how long the move should take based on the distance and moveSpeed
        float distance = Vector2.Distance(initialPosition, targetPos);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            previousPosition = transform.position;
            transform.position = Vector2.Lerp(initialPosition, targetPos, t);

            elapsedTime += Time.deltaTime;
            yield return null; //Wait until the next frame
        }
        //When the while loop exits, we're done moving

        //Make sure we hit it exactly
        transform.position = targetPos;

        EW_EventSystem.LeaveStoryNodeEvent -= SkipCurrentMove;
        executing = false;
        curMove = null;
        EW_EventSystem.InvokeEndMoveEvent();
    }
}
