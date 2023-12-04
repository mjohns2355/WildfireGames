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
            Vector2 target = new Vector2(commandData.targetPosition.x, commandData.targetPosition.y);
            EW_MoveCommand command = new EW_MoveCommand(commandData.actorName, target, commandData.final);
            moveCommands.Enqueue(command);
        }
        return moveCommands;
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