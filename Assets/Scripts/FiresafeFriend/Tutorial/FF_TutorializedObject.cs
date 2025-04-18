using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class FF_TutorializedObject : MonoBehaviour
{
    public int stepIndex;
    public UnityEvent onClick;
    protected bool canClick = true;
    float clickCooldown = 1f;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            canClick = false;
            DOVirtual.DelayedCall(clickCooldown, () => canClick = true); //cooldown
            onClick.Invoke();
        }
    }

    public virtual void OnTutorialStepStart()
    {
        
    }
    public virtual void OnTutorialStepComplete()
    {
        
        FF_TutorialManager.Instance.tutorialSteps[stepIndex].onStepComplete.Invoke();
        onClick.RemoveAllListeners();
        enabled = false;
        //enabled = false;
    }
    
}
