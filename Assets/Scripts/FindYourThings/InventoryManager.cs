using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance; // static private variable of the component data type
	public static InventoryManager Instance { get { return _instance; } } // public way to access the private variable

	private void Awake() {
        // if there is already a value assigned to the private variable and its not this, destroy this
    	if (_instance != null && _instance != this) {
        	Destroy(this.gameObject);
    	} else { 
        // if there is no value assigned to the private variable, assign this as the reference
        	_instance = this;
        }	
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
