using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableStructure : Structure
{
    [SerializeField] GameObject horseTrailers;

    private void Start()
    {
        horseTrailers.SetActive(false);
    }
    public void RelocateHorse()
    {
        horseTrailers.SetActive(true);
    }
}
