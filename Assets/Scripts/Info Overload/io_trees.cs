using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class io_trees : MonoBehaviour
{
    public Vector3 startPos;
    public RectTransform endPos;
    public float speed = 2;
    private float lerpVal = 0;
    private Vector3 scale;
    private Vector3 finalScale;
    private RectTransform myRect;
    public bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        scale = GetComponent<RectTransform>().localScale;
        finalScale = scale * 4;
        myRect = GetComponent<RectTransform>();
        startPos = myRect.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            lerpVal += Time.deltaTime;
            myRect.position = Vector3.Lerp(startPos, endPos.position, lerpVal / speed);
            transform.localScale = Vector3.Lerp(scale, finalScale, lerpVal / speed);
            if (lerpVal > speed)
            {
                Destroy(gameObject);
            }
        }
    }
}
