using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OptionButton : MonoBehaviour
{
    public bool isLocked = false;
    [SerializeField]TextMeshProUGUI optionText;
    [SerializeField] Button button;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        button.interactable = !isLocked;
        optionText.gameObject.SetActive(!isLocked);
    }

    public void SetOptionButtonText(string text)
    {
        optionText.text = text;
    }
}
