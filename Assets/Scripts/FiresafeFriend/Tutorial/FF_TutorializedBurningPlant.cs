using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_TutorializedBurningPlant : FF_TutorializedObject
{
    public Animator animator;
    public GameObject fireParticle;
    public FF_Plants plant;
    public Renderer meshRender;

    public override void Start()
    {
        base.Start();
        animator.enabled = false;
        FF_TutorialManager.Instance.tutorialSteps[stepIndex].onStepStart.AddListener(() =>
        {
            animator.enabled = true;
            animator.Play("PlantBurningAnim");
            plant.isOnFire = true;
        });
    }


    public void OnAnimFinished()
    {
        //plant.gameObject.SetActive(false);
        base.OnTutorialStepComplete();
        
    }
}
