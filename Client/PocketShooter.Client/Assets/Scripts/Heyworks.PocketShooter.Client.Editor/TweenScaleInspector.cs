using Heyworks.PocketShooter.UI.Animation;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter
{
    [CustomEditor(typeof(TweenScale))]
    public class TweenScaleInspector : TweenerInspector
    {
        protected override void CustomInspectorGUI()
        {
            var tScale = (TweenScale)tween;

            EditorGUILayout.LabelField("Begin scale");
            GUI.contentColor = defaultContentColor;
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 15f;
            EditorGUIUtility.fieldWidth = 0;

            tScale.BeginScale = EditorTools.DrawVector3(tScale.BeginScale);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("End scale");
            GUI.contentColor = defaultContentColor;
            EditorGUILayout.BeginHorizontal();

            tScale.EndScale = EditorTools.DrawVector3(tScale.EndScale);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));

            EditorGUILayout.EndHorizontal();
        }
    }
}