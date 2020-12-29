using EQx.Game.Scenery;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudienceLines))]
[CanEditMultipleObjects]
public class AudienceEditor : Editor
{
    string str = "";
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.TextField(str);
        if (EditorGUI.EndChangeCheck()) {
            var t = target as AudienceLines;
            t.CalculateRings();
        }
    }
}
