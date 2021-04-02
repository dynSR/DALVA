using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntityStats))]
public class CharacterStatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EntityStats m_Target = (EntityStats)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh Stats"))
        {
            m_Target.RefreshCharacterStats();
        }
    }
}
