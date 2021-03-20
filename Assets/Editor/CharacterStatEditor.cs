using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterStat))]
public class CharacterStatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterStat m_Target = (CharacterStat)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh Stats"))
        {
            m_Target.RefreshCharacterStats();
        }
    }
}
