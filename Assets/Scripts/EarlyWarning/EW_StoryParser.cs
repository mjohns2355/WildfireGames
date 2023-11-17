using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.IO;
using System.Linq;

public class EW_StoryParser
{
    public static EW_StoryNode Parse(string jsonFilePath, EW_SceneManager sceneManager, List<EW_StoryNode> nodeList)
    {
        string jsonString = System.IO.File.ReadAllText(jsonFilePath);
        NodeDataWrapper dataWrapper = JsonUtility.FromJson<NodeDataWrapper>(jsonString);
        List<StoryNodeData> nodeDataList = dataWrapper.nodes;

        foreach (var nodeData in nodeDataList)
        {
            EW_StoryNode node = CreateNode(nodeData, sceneManager);
            nodeList.Add(node);
        }

        for (int i = 0; i < nodeDataList.Count; i++)
        {
            ConnectNodes(nodeDataList[i], nodeList, nodeList[i]);
        }

        // Return the first node
        return nodeList.Count > 0 ? nodeList[0] : null;
    }

    private static EW_StoryNode CreateNode(StoryNodeData nodeData, EW_SceneManager sceneManager)
    {
        switch (nodeData.type)
        {
            case "Move":
                Debug.Log("Movecommand lines: " + nodeData.lines);
                return new EW_MoveNode(sceneManager, ParseMoveCommands(nodeData.commands));
            case "Dialogue":
                return new EW_DialogueNode(nodeData.lines);
            case "Choice":
                return new EW_ChoiceNode(ParseChoices(nodeData.choices));
            // Add more cases for additional node types if needed
            default:
                Debug.LogError("Unknown node type: " + nodeData.type);
                return null;
        }
    }

    private static Queue<EW_MoveCommand> ParseMoveCommands(List<MoveCommandData> commands)
    {
        Queue<EW_MoveCommand> moveCommands = new Queue<EW_MoveCommand>();
        foreach (var commandData in commands)
        {
            string name = commandData.actorName;
            Vector2 target = new Vector2(commandData.targetPosition.x, commandData.targetPosition.y);
            bool final = commandData.final;
            EW_MoveCommand command = new EW_MoveCommand(name, target, final);
            moveCommands.Enqueue(command);
        }
        return moveCommands;
    }

    private static List<EW_Choice> ParseChoices(List<ChoiceData> choices)
    {
        List<EW_Choice> parsedChoices = new List<EW_Choice>();
        foreach (var choiceData in choices)
        {
            Debug.Log(choiceData.text);
            Debug.Log(choiceData.onSelect);
            Type sceneMan = typeof(EW_SceneManager);
            MethodInfo method = sceneMan.GetMethod(choiceData.onSelect);
            if (method == null)
            {
                Debug.LogError("Invalid method!");
                break;
            }
            Action action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
            parsedChoices.Add(new EW_Choice(choiceData.text, action));
        }
        return parsedChoices;
    }

    private static void ConnectNodes(StoryNodeData nodeData, List<EW_StoryNode> nodeList, EW_StoryNode fromNode)
    {
        Debug.Log(nodeData.nextNodeID);
        if (nodeData.nextNodeID != -1 && nodeData.nextNodeID < nodeList.Count)
        {
            Debug.Log("From node: " + fromNode);
            fromNode.SetNext(nodeList[nodeData.nextNodeID]);
        }
    }
}

[System.Serializable]
public class NodeDataWrapper
{
    public List<StoryNodeData> nodes;
}

[System.Serializable]
public class StoryNodeData
{
    public int id;
    public string type;
    public List<MoveCommandData> commands;
    public List<string> lines;
    public List<ChoiceData> choices;
    public int nextNodeID;
}

[System.Serializable]
public class MoveCommandData
{
    public Vector2 targetPosition;
    public string actorName;
    public bool final;
}

[System.Serializable]
public class ChoiceData
{
    public string text;
    public string onSelect;
    public int nextNodeID;
}
