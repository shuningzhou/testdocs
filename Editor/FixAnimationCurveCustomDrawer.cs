using UnityEditor;
using Parallel;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(FAnimationCurve))]
public class FixAnimationCurveCustomDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string name = property.displayName;
        FAnimationCurve fixAnimationCurve = (FAnimationCurve)property.GetValue();

        EditorGUI.BeginChangeCheck();

        fixAnimationCurve.displayingCurve = EditorGUI.CurveField(position, name, fixAnimationCurve.displayingCurve);

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("Curve updated");
            Debug.Log(fixAnimationCurve.displayingCurve.length);

            fixAnimationCurve.ImportUnityAnimationCurve();


            property.SetValue(fixAnimationCurve);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        //if(displayingCurve.change)
        ////var boxRect = new Rect(position.x + space, position.y + space, position.width - 2 * space, position.height - 2 * space);

        //var fieldRect = new Rect(position.x + space, position.y + space, position.width - 2 * space, EditorGUIUtility.singleLineHeight);
        //var rawRect = new Rect(position.x + space, fieldRect.y + EditorGUIUtility.singleLineHeight, position.width - 2 * space, EditorGUIUtility.singleLineHeight);

        //GUI.Box(position, "");

        //EditorGUI.BeginChangeCheck();
        //long oldRawValue = raw.longValue;
        //FFloat oldValue = FFloat.FromRaw(oldRawValue);
        //float newVal = EditorGUI.FloatField(fieldRect, new GUIContent(name), (float)oldValue);
        //if (EditorGUI.EndChangeCheck())
        //{
        //    FFloat newFixedValue = (FFloat)newVal;
        //    raw.longValue = newFixedValue.Raw;
        //}

        //var style = new GUIStyle(GUI.skin.label);
        //style.alignment = TextAnchor.MiddleRight;
        //style.normal.textColor = Color.gray;
        //EditorGUI.LabelField(rawRect, $"RawValue: {raw.longValue}", style);
    }
}
