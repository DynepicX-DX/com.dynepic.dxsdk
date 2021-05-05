using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class MOTARStateMachineGraph : EditorWindow
{
    private static MOTARStateMachineGraphView _graphView;

   
    public static void OpenMOTARStateMachineGraphWindow()
    {
        var window = GetWindow<MOTARStateMachineGraph>();
        
    }
    // Start is called before the first frame update

    private void OnEnable()
    {

        
        ConstructGraphView();
        GenerateToolar();

    }

    private void ConstructGraphView()
    {
        _graphView = new MOTARStateMachineGraphView
        {
            name = "MOTAR State Machine Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolar()
    {
        var toolbar = new Toolbar();

        var nodeCreateButton = new Button(clickEvent: () => { _graphView.CreateNode("State Machine Node"); });

        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(toolbar);
    }
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void OnDestroy()
    {
        
    }
}
