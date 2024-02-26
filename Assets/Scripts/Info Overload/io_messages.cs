using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class io_messages : MonoBehaviour
{

    public string[] messages;
    public TextMeshProUGUI messageText;
    private int messageCounter = 0;
    public int messageTime = 4;
    public GameObject alert;
    private float messageTimer;

    // Start is called before the first frame update
    void Start()
    {
        messageTimer = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alert.activeInHierarchy && !messageText.gameObject.activeInHierarchy)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer < 0)
            {
                messageTimer = messageTime;
                messageText.text = messages[messageCounter];
                messageCounter++;
                if(messageCounter >= messages.Length)
                {
                    messageCounter = 0;
                }
                alert.SetActive(true);
            }
        }

    }
}
