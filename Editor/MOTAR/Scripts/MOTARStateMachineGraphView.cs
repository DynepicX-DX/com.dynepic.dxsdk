using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental;
using UnityEngine.Experimental;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

public class MOTARStateMachineGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 100);
    public MOTARStateMachineGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        AddElement(GenerateEntryPointNode());
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(funcCall: (port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        }));
        return compatiblePorts;
    }
    private Port GeneratePort(MOTARStateMachineNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(bool));
    }
    private MOTARStateMachineNode GenerateEntryPointNode()
    {
        var node = new MOTARStateMachineNode
        {
            title = "START",
            nodeGUID = Guid.NewGuid().ToString(),
            description = "ENTRYPOINT",
            EntryPoint = true

        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.SetPosition(new Rect(100, 200, 100, 150));
        node.outputContainer.Add(generatedPort);
        node.RefreshExpandedState();
        node.RefreshPorts();
        return node;
    }
    public void CreateNode(string nodeName)
    {
        AddElement(CreateMOTARStateMachineNode(nodeName));
    }
    public MOTARStateMachineNode CreateMOTARStateMachineNode(string nodeName)
    {
        var motarStateNode = new MOTARStateMachineNode
        {
            title = nodeName,
            description = nodeName,
            nodeGUID = Guid.NewGuid().ToString()

        };

        var inputPort = GeneratePort(motarStateNode,Direction.Input, capacity: Port.Capacity.Multi);

        inputPort.portName = "Input";
        motarStateNode.inputContainer.Add(inputPort);
        motarStateNode.RefreshExpandedState();
        motarStateNode.RefreshPorts();
        motarStateNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return motarStateNode;

    }

    
}
