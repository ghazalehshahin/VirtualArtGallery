using Haply.hAPI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Board))][InitializeOnLoad]
public class BoardCustomEditor : Editor
{
    private int targetPortIndex = 0;
    private SerializedProperty portProperty;
    public override void OnInspectorGUI()
    {
        Board board = target as Board;

        string[] availablePorts = board.GetAvailablePorts();

        if (availablePorts.Length > 0)
        {
            portProperty = serializedObject.FindProperty("targetPort");
            targetPortIndex = EditorGUILayout.Popup("Active Ports", targetPortIndex, availablePorts);
            portProperty.stringValue = availablePorts[targetPortIndex];
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
}
