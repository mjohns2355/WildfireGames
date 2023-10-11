using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class popupRandomizer : MonoBehaviour
{
    public string[] messages;
    public TMPro.TextMeshProUGUI msg;
    public RectTransform msgWindow;
    public float timer = 2;

    public float speeder = 0.5f;

    private void Start()
    {
        timer = Random.Range(2, 4);
    }

    public void newPopup()
    {
        int i = Random.Range(0, messages.Length - 1);
        msg.text = messages[i];

        //TODO: populate message preview in one of the message boxes
        //TODO: if message box selected, display full message


      //  Vector3 pos = new Vector3();
       // pos.z = msgWindow.position.z;
       // pos.x = Random.Range(-200, 200);
      //  pos.y = Random.Range(-200, 200);
      //  msgWindow.anchoredPosition = pos;
      //  msgWindow.gameObject.SetActive(true);
    }

    public void SpeedUp()
    {
        timer -= speeder;
        if(speeder < 3)
        {
            speeder += 0.5f;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            if (!msgWindow.gameObject.activeInHierarchy) //TODO: replace with check if not all message boxes are full
            {
                timer = Random.Range(3, 5);
                newPopup();
            }
            else //TODO: if all message boxes full, replace one
            {
                /*
                GameObject g = Instantiate(gameObject);
                g.name = name;
                g.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>().parent);
                g.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                g.GetComponent<RectTransform>().localScale = Vector3.one;
                g.GetComponent<popupRandomizer>().newPopup();
                g.GetComponent<popupRandomizer>().timer = Random.Range(3, 5);
                g.GetComponent<popupRandomizer>().speeder = speeder;
                Destroy(this);
                */
            }
            
        }
    }
}
