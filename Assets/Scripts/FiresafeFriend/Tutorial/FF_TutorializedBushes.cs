using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FF_TutorializedBushes : FF_TutorializedObject
{
    public List<FF_Plants> bushes;
    int bushesNeededToRemove;
    public override void Start()
    {
        bushesNeededToRemove = bushes.Count;
        foreach (var bush in bushes)
        {
            
            bush.OnCombustibleDestroyed += () =>
            {
                bushesNeededToRemove--;
                if (bushesNeededToRemove <= 0)
                {
                    bushes.Clear();
                    OnTutorialStepComplete();
                }
            };
        }
    }

}
