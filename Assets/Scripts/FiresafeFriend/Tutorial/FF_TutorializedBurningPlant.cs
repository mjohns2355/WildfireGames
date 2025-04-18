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
        //DOVirtual.DelayedCall(3f, () =>
        //{
        //    onClick.AddListener(() =>
        //    {
        //        OnTutorialStepComplete();
        //        Destroy(normalPlant);
        //        Destroy(burningPlant);
        //        Destroy(gameObject);
        //        animator.enabled = false;
        //    });

        //});

        onClick.AddListener(() =>
        {
            OnTutorialStepComplete();
            Destroy(normalPlant);
            Destroy(burningPlant);
            Destroy(gameObject);
            animator.enabled = false;
        });
        animator.enabled = true;
        animator.Play("PlantBurningAnim");
        plant.isOnFire = true;
    }

    //public void OnAnimFinished()
    //{
    //    //plant.gameObject.SetActive(false);
    //    OnTutorialStepComplete();
    //    Destroy(normalPlant);
    //    Destroy(burningPlant);
    //    Destroy(gameObject);

    //}
}
