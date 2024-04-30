using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureContextMenu : MonoBehaviour
{
    public GameObject backdrop;
    public TextMeshProUGUI text;
    public Button closeButton;
    public Button assignButton;
    public Structure owner;
    // Start is called before the first frame update
    private void Awake()
    {
        assignButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateText(owner.structureInfoDict);
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
