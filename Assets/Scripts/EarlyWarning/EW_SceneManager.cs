using System.Collections.Generic;
using UnityEngine;

public class EW_SceneManager : MonoBehaviour
{
    public EW_StoryNode curNode = null;
    public EW_Actor actor;
    private bool done = false;
    public EW_UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        EW_EventSystem.ChangeStoryNodeEvent += ChangeStoryNode;

        SetUpNodeList();
    }

    void SetUpNodeList()
    {
        Queue<EW_MoveCommand> moveQueue = new Queue<EW_MoveCommand>(new[] {
            new EW_MoveCommand(actor, new Vector2(0, 3)),
            new EW_MoveCommand(actor, new Vector2(5, 3), true)
        });

        EW_DialogueNode dNode = new EW_DialogueNode(new[] {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
            "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
        });

        List<EW_Choice> choices = new List<EW_Choice>
        {
            new EW_Choice("Choice 1", () =>
            {
                Debug.Log("Choice 1 selected");
                ChangeStoryNode(new EW_DialogueNode(new[] {
                    "Choice 1 selected"
                }));
            }),
            new EW_Choice("Choice 2", () =>
            {
                Debug.Log("Choice 2 selected");
                ChangeStoryNode(new EW_DialogueNode(new[] {
                    "Choice 2 selected"
                }));
            }),
            new EW_Choice("Choice 3", () =>
            {
                Debug.Log("Choice 3 selected");
                ChangeStoryNode(new EW_DialogueNode(new[] {
                    "Choice 3 selected"
                }));
            })
        };
        EW_ChoiceNode choiceNode = new EW_ChoiceNode(choices);

        curNode = new EW_MoveNode(this, moveQueue);
        curNode.SetNext(dNode);
        dNode.SetNext(choiceNode);

        Debug.Log(curNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && !done)
        {
            if (uiManager.typingCoroutine != null)
            {
                uiManager.SkipTyping();
                return;
            }

            ChangeStoryNode(curNode.Advance());

            Debug.Log("Curnode: " + curNode);
        }
    }

    public void ChangeStoryNode(EW_StoryNode node)
    {
        if (curNode != node)
        {
            EW_EventSystem.InvokeLeaveStoryNodeEvent();
            curNode = node;
            if (curNode == null)
            {
                done = true;
                return;
            }
            curNode.Play();
        }
    }
}