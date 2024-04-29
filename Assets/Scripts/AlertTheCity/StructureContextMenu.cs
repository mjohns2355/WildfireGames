using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StructureContextMenu : MonoBehaviour
{
    public GameObject backdrop;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    public void UpdateText(Dictionary<string,int> structureInfo)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var item in structureInfo)
        {
            builder.AppendLine(item.Key + ":" + item.Value + "\n");
        }
        text.text = builder.ToString();
    }
}
