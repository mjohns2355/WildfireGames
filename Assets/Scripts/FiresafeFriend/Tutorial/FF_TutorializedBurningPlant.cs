using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FF_TutorializedBurningPlant : FF_TutorializedObject
{
    public Animator animator;
    public GameObject fireParticle, normalPlant, burningPlant;
    public FF_Plants plant;

    public void StartBurningPlant()
    {
        animator.enabled = true;
        animator.Play("PlantBurningAnim");
        plant.isOnFire = true;
    }

    public void OnAnimFinished()
    {
        onClick.AddListener(() =>
        {
            OnTutorialStepComplete();
            Destroy(normalPlant);
            Destroy(burningPlant);
            Destroy(gameObject);
            animator.enabled = false;
        });

    }
}
