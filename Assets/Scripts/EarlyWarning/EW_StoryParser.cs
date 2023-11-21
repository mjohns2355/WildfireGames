using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.IO;

public class EW_StoryParser
{
    public static EW_StoryNode Parse(string jsonFilePath, EW_SceneManager sceneManager, List<EW_StoryNode> nodeList)
    {
        string jsonString = File.ReadAllText(jsonFilePath);
        NodeDataWrapper dataWrapper = JsonUtility.FromJson<NodeDataWrapper>(jsonString);
        List<StoryNodeData> nodeDataList = dataWrapper.nodes;

        foreach (var nodeData in nodeDataList)
        {
            EW_StoryNode node = CreateNode(nodeData, sceneManager);
            node.SetID(nodeData.id);
            nodeList.Add(node);
        }

        for (int i = 0; i < nodeDataList.Count; i++)
        {
            if (nodeDataList[i].nextNodeID < nodeList.Count)
            {
                nodeList[i].SetNext(nodeDataList[i].nextNodeID);
            }
        }

        // Return the first node
        return nodeList.Count > 0 ? nodeList[0] : null;
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
                node = new EW_ChoiceNode(ParseChoices(nodeData.choices));
                break;
            default:
                Debug.LogError("Unknown node type: " + nodeData.type);
                return null;
        }
        if (nodeData.function != null)
        {
            SetFunction(node, nodeData.function);
        }
        return node;
    }

    private static void SetFunction(EW_StoryNode node, string functionName)
    {
        Type functionClass = typeof(EW_StoryFunctions);
        MethodInfo method = functionClass.GetMethod(functionName);
        if (method == null)
        {
            Debug.LogError("No method found with name " + functionName);
        }
        Action action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
        node.SetEnterFunction(action);
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
            Type functionClass = typeof(EW_StoryFunctions);
            MethodInfo method = functionClass.GetMethod(choiceData.onSelect);
            if (method == null)
            {
                Debug.LogError("No method found with name " + choiceData.onSelect);
            }
            Action action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);
            parsedChoices.Add(new EW_Choice(choiceData.text, action));
        }
        return parsedChoices;
    }
}

[Serializable]
public class NodeDataWrapper
{
    public List<StoryNodeData> nodes;
}

[Serializable]
public class StoryNodeData
{
    public int id;
    public string type;
    public string function = null;
    public List<MoveCommandData> commands;
    public List<string> lines;
    public List<ChoiceData> choices;
    public int nextNodeID;
}

[Serializable]
public class MoveCommandData
{
    public Vector2 targetPosition;
    public string actorName;
    public bool final;
}

[Serializable]
public class ChoiceData
{
    public string text;
    public string onSelect;
    public int nextNodeID;
}
