using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Structure : MonoBehaviour
{
    public enum StructureType { House, Shelter}
    public int memberNum;
    public int petNum;
    public int carNum;
    public bool hasElder;
    public bool hasPet;
    public StructureType structureType;
    public GameObject contextMenu;
    [SerializeField]
    GameObject roof;

    private void Start()
    {
       
    }
    public void OnStructureClick()
    {
        
        roof.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}
