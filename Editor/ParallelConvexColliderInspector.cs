using UnityEngine;
using UnityEditor;
using Parallel;

[CustomEditor(typeof(ParallelConvexCollider3D))]
public class ParallelConvexColliderInspector : Editor
{
    ParallelConvexCollider3D editObject;

    private void OnEnable()
    {
        editObject = target as ParallelConvexCollider3D;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isTrigger"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createUnityPhysicsCollider"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_friction"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_bounciness"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("limit"));

        if (GUILayout.Button("Calculate Convex Data"))
        {
            if(editObject.limit < 4)
            {
                editObject.limit = 4;
            }
            else if(editObject.limit > 128)
            {
                editObject.limit = 128;
            }

            editObject.BuildConvexData();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}

