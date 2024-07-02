using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureContextMenu : MonoBehaviour
{
    public GameObject backdrop;
    public TextMeshProUGUI title;
    public Transform options;
    public GameObject optionButtonPrefab;
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

        HouseStructure house = (HouseStructure)owner;
        if (house.isMainHouse)
        {
            UpdateMenuForHouse(house.houseInfo);
        }

    }

    private void OnDisable()
    {
        for (int i = 0; i < options.childCount; i++) { 
        
            Destroy(options.GetChild(i).gameObject);
        }
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
        title.text = builder.ToString();
    }

    public void UpdateMenuForHouse(string text)
    {
        //Debug.Log("INFO: " + text);
        char[] delimiterChars = { ':', '|' };
        string[] words = text.Split(delimiterChars);
        //foreach (string word in words) { 
        //    Debug.Log(word);
        //}
        title.text = words[0];
        for (int i = 1; i < words.Length; i++) {
            SpawnOptionButtons(words[i],i == words.Length-1);
        }
    }

    private void SpawnOptionButtons(string text, bool lastOption = false)
    {
        GameObject button = Instantiate(optionButtonPrefab,options);
        var optionButton = button.GetComponent<OptionButton>();
        optionButton.SetOptionButtonText(text);
        if (lastOption)
        {
            optionButton.isLocked = true;
        }
    }
}
