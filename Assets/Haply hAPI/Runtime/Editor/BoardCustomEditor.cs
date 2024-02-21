using System.IO.Ports;
using Haply.hAPI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Board))]
public class BoardCustomEditor : Editor
{
    private SerializedProperty portProperty;
    public override void OnInspectorGUI()
    {
        string[] availablePorts = GetAvailablePorts();

        if (availablePorts.Length > 0)
        {
            portProperty = serializedObject.FindProperty("targetPort");
            int selectedPortIndex = GetSelectedPortIndex(availablePorts, portProperty.stringValue);
            int newPortIndex = EditorGUILayout.Popup("Active Ports", selectedPortIndex, availablePorts);
            if (newPortIndex != selectedPortIndex)
            {
                portProperty.stringValue = availablePorts[newPortIndex];
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No available devices detected, please enter port manually", MessageType.Info);
            portProperty = serializedObject.FindProperty("targetPort");
            EditorGUILayout.PropertyField(portProperty, new GUIContent("Port"));
        }
        serializedObject.ApplyModifiedProperties();
        DrawDefaultInspector();
    }
    
    private int GetSelectedPortIndex(string[] ports, string selectedPort)
    {
        for (int i = 0; i < ports.Length; i++)
        {
            if (ports[i] == selectedPort)
            {
                return i;
            }
        }
        return 0;
    }
    
    private string [] GetAvailablePorts()
    {
        return SerialPort.GetPortNames();
    }
}
