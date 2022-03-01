using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameInformation))]
public class GameInfoEditor : Editor
{
    GameInformation root;

    private void OnEnable()
    {
        root = (GameInformation)target;
    }

    public override void OnInspectorGUI()
    {


        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Все в редакторе", MessageType.Info);
        EditorGUILayout.Space();
        if (GUILayout.Button("Открыть редактор", GUILayout.Height(35)))
        {
            CardManagerEditor.ShowWindows();
        }
    }
}
