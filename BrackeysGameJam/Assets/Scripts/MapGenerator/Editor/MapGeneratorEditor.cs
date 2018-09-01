using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    MapGenerator r;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        r = (MapGenerator)target;       
        if (GUILayout.Button("Generate"))
        {
            r.Initialize();
        }
        if (GUILayout.Button("Demolish"))
        {
            r.Demolish();
        }
    }
}
