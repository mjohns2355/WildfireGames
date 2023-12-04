using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SD_AQIBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;
    private static SD_AQIBar instance;    
    
    
    public static SD_AQIBar Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SD_AQIBar>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void SetMaxAQI(float AQI)
    {
        slider.maxValue = AQI;
        slider.value = 0;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetAQI(float AQI)
    {
        slider.value = AQI;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
