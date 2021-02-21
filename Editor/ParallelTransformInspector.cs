using UnityEngine;
using UnityEditor;
using Parallel;

//[CustomEditor(typeof(ParallelTransform)), CanEditMultipleObjects]
//public class ParallelTransformInspector : Editor
//{
//    ParallelTransform[] editObjects;

//    private void OnEnable()
//    {
//        editObjects = new ParallelTransform[targets.Length];
//        for (int i = 0; i < targets.Length; i++)
//        {
//            editObjects[i] = targets[i] as ParallelTransform;
//        }
//    }

//    public override void OnInspectorGUI()
//    {
//        base.DrawDefaultInspector();

//        foreach(ParallelTransform pt in editObjects)
//        {
//            if(pt.transform.hasChanged)
//            {
//                pt.transform.hasChanged = false;
//                pt.ImportFromUnity();
//                EditorUtility.SetDirty(pt);
//            }
//        }
//    }
//}
