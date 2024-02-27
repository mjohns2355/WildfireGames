using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class io_dialog : MonoBehaviour
{
    public string[] dialogs;
    public TextMeshProUGUI textbox;
    private int textCounter;

    public void NextText()
    {
        textCounter++;
        if(textCounter >= dialogs.Length)
        {
            gameObject.SetActive(false);
        } else
        {

            textbox.text = dialogs[textCounter];
        }
    }
}
