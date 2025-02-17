using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_TutorializedObject : MonoBehaviour
{
    public int stepIndex;
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnTutorialStepStart()
    {
        
    }
    public virtual void OnTutorialStepComplete()
    {
        
        FF_TutorialManager.Instance.tutorialSteps[stepIndex].onStepComplete.Invoke();
        //enabled = false;
    }
}
