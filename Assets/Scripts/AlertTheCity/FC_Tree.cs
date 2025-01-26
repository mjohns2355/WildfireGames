using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class FC_Tree : MonoBehaviour
{
    public GameObject normal, burnt;
    public List<GameObject> burntModels;
    public bool isNormalTree;
    private bool _isBurnt = false;
    private float startTime;
    private float scaler = 1f;
    private Quaternion rotation;
    public bool IsBurnt
    {
        get => _isBurnt;
        set
        {
            if (_isBurnt != value)
            {
                _isBurnt  = value;
                OnBurnt(_isBurnt);
            }
        }
    }

    public void Start()
    {
        startTime = Random.Range(3f, 10f);
        burnt.SetActive(false);
        float randomY = Random.Range(0f, 360f);
        rotation = Quaternion.Euler(0, randomY, 0);
        burnt.transform.rotation = rotation;
        normal.transform.rotation = rotation;
        if (isNormalTree)
        {
            scaler = Random.Range(0.5f, 1.0f);
            normal.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", GetRandomGreenHSV());

        }
        
        normal.transform.localScale = Vector3.one * scaler;
        burnt.transform.localScale = Vector3.one * scaler;
        var obj = Instantiate(burntModels[Random.Range(0, burntModels.Count)], burnt.transform);
        obj.transform.localPosition = Vector3.zero;
    }

    public void OnBurnt(bool isBurnt)
    {
        StartCoroutine(BurntRoutine(isBurnt));
    }

    IEnumerator BurntRoutine(bool isBurnt)
    {
        yield return new WaitForSeconds(startTime);
        //Debug.Log($"{gameObject.name} is burnt");
        normal.SetActive(!isBurnt);
        if (burnt != null)
            burnt.SetActive(isBurnt);
    }

    Color GetRandomGreenHSV()
    {
        float hue = Random.Range(0.25f, 0.4f); 
        float saturation = Random.Range(0.7f, 1f); 
        float value = Random.Range(0.5f, 0.8f); 
        return Color.HSVToRGB(hue, saturation, value);
    }
}
