using System.Collections;
using UnityEngine;

public class EW_Actor : MonoBehaviour
{
    public bool executing = false;
    Coroutine curMove;
    Vector2 targetPos;
    float moveSpeed = 2;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

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
                spriteRenderer.sprite = (movementDirection.x > 0) ? rightSprite : leftSprite;
            else
                spriteRenderer.sprite = (movementDirection.y > 0) ? upSprite : downSprite;
        }
    }


    public void Execute(EW_MoveCommand command)
    {
        SkipCurrentMove();
        EW_EventSystem.LeaveStoryNodeEvent += SkipCurrentMove;
        curMove = StartCoroutine(MoveTo(command.targetPosition));
    }

    private void SkipCurrentMove()
    {
        if (executing)
        {
            transform.position = targetPos;
            StopCoroutine(curMove);
        }
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
            previousPosition = transform.position;
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
