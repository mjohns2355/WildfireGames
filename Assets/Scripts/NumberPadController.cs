using UnityEngine;
using UnityEngine.UI;

public class NumberPadController : MonoBehaviour
{
    public InputField zipInput;
    public GameObject numberPadPanel;
    public Button fetchButton;

    void Start()
    {
        numberPadPanel.SetActive(false);
        fetchButton.onClick.AddListener(HideNumberPad);
    }

    // Called via EventTrigger → Pointer Down on InputField
    public void ShowNumberPad()
    {
        numberPadPanel.SetActive(true);
    }

    public void HideNumberPad()
    {
        numberPadPanel.SetActive(false);
    }

    public void AddDigit(string digit)
    {
        if (zipInput.text.Length < 5)
        {
            zipInput.text += digit;
        }
    }

    public void Backspace()
    {
        if (zipInput.text.Length > 0)
        {
            zipInput.text = zipInput.text.Substring(0, zipInput.text.Length - 1);
        }
    }

    public void Clear()
    {
        zipInput.text = "";
    }
}
