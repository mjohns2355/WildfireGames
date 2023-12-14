using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class EW_StoryParser
{
    public static EW_StoryNode Parse(string jsonFilePath, EW_SceneManager sceneManager)
    {
        TextAsset textData = Resources.Load<TextAsset>(jsonFilePath);
        string jsonString = textData.text;
        NodeDataWrapper dataWrapper = JsonUtility.FromJson<NodeDataWrapper>(jsonString);
        List<StoryNodeData> nodeDataList = dataWrapper.nodes;

        foreach (var nodeData in nodeDataList)
        {
            EW_StoryNode node = CreateNode(nodeData, sceneManager);
            node.id = nodeData.id;
            EW_SceneManager.nodeDict.Add(node.id, node);
        }

        foreach (var nodeData in nodeDataList)
        {
            var data = nodeDataList.Find(item => item.id == nodeData.id);
            EW_SceneManager.nodeDict[nodeData.id].nextNode = data.nextNodeID;
        }

        // Return the first node
        return EW_SceneManager.nodeDict.Count > 0 ? EW_SceneManager.nodeDict[0] : null;
    }

    private static EW_StoryNode CreateNode(StoryNodeData nodeData, EW_SceneManager sceneManager)
    {
        EW_StoryNode node;
        switch (nodeData.type)
        {
            case "Move":
                node = new EW_MoveNode(sceneManager, ParseMoveCommands(nodeData.commands));
                break;
            case "Dialogue":
                node = new EW_DialogueNode(nodeData.lines);
                break;
            case "Choice":
                node = new EW_ChoiceNode(nodeData.choices);
                break;
            default:
                Debug.LogError("Unknown node type: " + nodeData.type);
                return null;
        }
        if (nodeData.function != null)
        {
            SetFunction(node, nodeData.function, nodeData.argument);
        }
        return node;
    }

    //If you provide a "function" item in the JSON, it will call the function with the name provided
    //An "argument" item in the JSON will pass that as a string argument to the function
    private static void SetFunction(EW_StoryNode node, string functionName, string argument = null)
    {
        Type funcClass = typeof(EW_StoryFunctions);
        MethodInfo method = funcClass.GetMethod(functionName);

        var args = argument != null ? new object[] { argument } : null;
        Action bound = () => method.Invoke(null, args);
        node.SetEnterFunction(bound);
    }

    private static Queue<EW_MoveCommand> ParseMoveCommands(List<MoveCommandData> commands)
    {
        Queue<EW_MoveCommand> moveCommands = new Queue<EW_MoveCommand>();
        for (int i = 0; i < commands.Count; i++)
        {
            var commandData = commands[i];
            Vector2 target = new Vector2(commandData.targetPosition.x, commandData.targetPosition.y);
            EW_MoveCommand command = new EW_MoveCommand(commandData.actorName, target, i == commands.Count - 1);
            moveCommands.Enqueue(command);
        }

        return moveCommands;
    }
}

// These classes are purely data containers used for deserializing the JSON
[Serializable]
public class NodeDataWrapper
{
    public List<StoryNodeData> nodes;
}

//A StoryNodeData contains all possible fields for a node, 
//but only the ones relevant to the node type will be used
[Serializable]
public class StoryNodeData
{
    public int id;
    public string type;
    public string function = null;
    public string argument = null;
    public List<MoveCommandData> commands;
    public List<EW_DialogueLine> lines;
    public List<EW_Choice> choices;
    public int nextNodeID;
}

[Serializable]
public class MoveCommandData
{
    public Vector2 targetPosition;
    public string actorName;
    public bool final;
}