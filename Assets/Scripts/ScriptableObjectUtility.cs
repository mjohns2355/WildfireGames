using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptableObjectUtility
{
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        Debug.Log("Clone");
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}
