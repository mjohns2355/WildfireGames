using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EW_SceneManager : MonoBehaviour
{
    private EW_StoryNode curNode = null;
    public EW_Actor actor;
    public bool done = false;

    [SerializeField]
    private Image dialogueBox;
    [SerializeField]
    private TextMeshProUGUI textComponent;
    private float timeBetweenLetters = 0.04f;
    private string curTargetText;
    private Coroutine typingCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");

        dialogueBox.enabled = false;
        textComponent.text = "";

        EW_EventSystem.TriggerDialogueEvent += BeginDialogue;

        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;

        SetUpNodeList();
    }

    void SetUpNodeList()
    {
        Queue<EW_MoveCommand> moveQueue = new Queue<EW_MoveCommand>(new[] {
            new EW_MoveCommand(actor, new Vector2(0, 3)),
            new EW_MoveCommand(actor, new Vector2(5, 3), true)
        });

        curNode = new EW_MoveNode(this, moveQueue);

        Queue<string> lines = new Queue<string>(new[] {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
            "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        });

        curNode.SetNext(new EW_DialogueNode(lines));

        Debug.Log(curNode);
    }

    int counter = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && !done)
        {
            Debug.Log("space");

            if (typingCoroutine != null)
            {
                SkipTyping();
                return;
            }

            ChangeStoryNode(curNode.Advance());

            Debug.Log("Curnode: " + curNode);
        }
    }

    public void ChangeStoryNode(EW_StoryNode node)
    {
        curNode = node;
        if (curNode == null)
        {
            done = true;
            return;
        }
    }

    public void BeginDialogue(string textToType)
    {
        curTargetText = textToType;
        dialogueBox.enabled = true;
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    public void EndDialogue()
    {
        curTargetText = "";
        dialogueBox.enabled = false;
        ChangeStoryNode(curNode.Advance());
    }

    private void SkipTyping()
    {
        textComponent.text = curTargetText;
        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private IEnumerator TypeTextCoroutine()
    {
        textComponent.text = ""; // Clear the text

        for (int i = 0; i < curTargetText.Length; i++)
        {
            textComponent.text += curTargetText[i]; // Add one character at a time

            // Wait for a short duration to control the typing speed
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        typingCoroutine = null;
    }
}

public static class EW_EventSystem
{
    public delegate void DialogueDelegate(string line);
    public static event DialogueDelegate TriggerDialogueEvent, SkipDialogueEvent;

    public delegate void StoryNodeDelegate(EW_StoryNode node);
    public static event StoryNodeDelegate ChangeStoryNodeEvent;

    public static void InvokeEndStoryNodeEvent(EW_StoryNode node)
    {
        ChangeStoryNodeEvent?.Invoke(node);
    }

    public static void InvokeTriggerDialogueEvent(string line)
    {
        TriggerDialogueEvent?.Invoke(line);
    }

    public static void InvokeSkipDialogueEvent(string line)
    {
        SkipDialogueEvent?.Invoke(line);
    }
}