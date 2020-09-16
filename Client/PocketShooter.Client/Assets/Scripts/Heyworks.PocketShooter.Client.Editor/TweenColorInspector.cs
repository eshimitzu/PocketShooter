using Heyworks.PocketShooter.UI.Animation;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TweenColor))]
    public class TweenColorInspector : TweenerInspector
    {
        protected override void CustomInspectorGUI()
        {
            var tColor = (TweenColor)tween;

            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("RGBA channels mask", true, GUILayout.Width(150f));
            Color defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            tColor.UseChanelMask[0] = EditorGUILayout.Toggle(tColor.UseChanelMask[0], GUILayout.Width(20f));
            GUI.backgroundColor = Color.green;
            tColor.UseChanelMask[1] = EditorGUILayout.Toggle(tColor.UseChanelMask[1], GUILayout.Width(20f));
            GUI.backgroundColor = Color.blue;
            tColor.UseChanelMask[2] = EditorGUILayout.Toggle(tColor.UseChanelMask[2], GUILayout.Width(20f));
            GUI.backgroundColor = Color.white;
            tColor.UseChanelMask[3] = EditorGUILayout.Toggle(tColor.UseChanelMask[3], GUILayout.Width(20f));
            GUI.backgroundColor = defaultColor;
            EditorGUILayout.EndHorizontal();
            if (tColor.IsBeginStateSet)
            {
                GUI.contentColor = Color.green;
            }

            EditorGUILayout.LabelField("Begin " + (tColor.IsOnlyAlphaTween ? "alpha" : "color"));
            GUI.contentColor = defaultContentColor;
            EditorGUILayout.BeginHorizontal();
            if (EditorTools.DrawButton("R", "Reset alpha value", IsResetColorValid(tColor.BeginColor), 20f))
            {
                EditorTools.RegisterUndo("Reset alpha value", tColor);
                tColor.BeginColor.a = 0f;
            }

            if (tColor.IsOnlyAlphaTween)
            {
                tColor.BeginColor.a = EditorGUILayout.Slider(tColor.BeginColor.a, 0f, 1f);
            }
            else
            {
                tColor.BeginColor = EditorGUILayout.ColorField(tColor.BeginColor);
            }

            if (EditorTools.DrawButton(
                "S",
                ("Set current " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")),
                IsSetColorValid(tColor.BeginColor, tColor.CurrentColor, tColor.IsOnlyAlphaTween),
                20f))
            {
                EditorTools.RegisterUndo(("Set begin " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")), tColor);
                if (tColor.IsOnlyAlphaTween)
                {
                    tColor.BeginColor.a = tColor.CurrentColor.a;
                }
                else
                {
                    tColor.BeginColor = tColor.CurrentColor;
                }
            }

            EditorGUILayout.EndHorizontal();
            if (tColor.IsEndStateSet)
            {
                GUI.contentColor = Color.green;
            }

            EditorGUILayout.LabelField("End " + (tColor.IsOnlyAlphaTween ? "alpha" : "color"));
            GUI.contentColor = defaultContentColor;
            EditorGUILayout.BeginHorizontal();
            if (EditorTools.DrawButton("R", "Reset alpha value", IsResetColorValid(tColor.EndColor), 20f))
            {
                EditorTools.RegisterUndo("Reset alpha value", tColor);
                tColor.EndColor.a = 0f;
            }

            if (tColor.IsOnlyAlphaTween)
            {
                tColor.EndColor.a = EditorGUILayout.Slider(tColor.EndColor.a, 0f, 1f);
            }
            else
            {
                tColor.EndColor = EditorGUILayout.ColorField(tColor.EndColor);
            }

            if (EditorTools.DrawButton(
                "S",
                ("Set current " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")),
                IsSetColorValid(tColor.EndColor, tColor.CurrentColor, tColor.IsOnlyAlphaTween),
                20f))
            {
                EditorTools.RegisterUndo(("Set end " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")), tColor);
                if (tColor.IsOnlyAlphaTween)
                {
                    tColor.EndColor.a = tColor.CurrentColor.a;
                }
                else
                {
                    tColor.EndColor = tColor.CurrentColor;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));
            if (EditorTools.DrawButton("R", "Reset target", IsResetTargetValid(tColor), 20f))
            {
                EditorTools.RegisterUndo("Reset target", tColor);
                tColor.Target = null;
            }

            tColor.Target = (GameObject)EditorGUILayout.ObjectField(tColor.Target, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
        }

        private bool IsResetColorValid(Color c)
        {
            return (c.a != 0f);
        }

        private bool IsSetColorValid(Color cc, Color nc, bool alpha)
        {
            return (!alpha && ((cc.r != nc.r) || (cc.g != nc.g) || (cc.b != nc.b))) || (cc.a != nc.a);
        }

        private bool IsResetTargetValid(TweenColor t)
        {
            return t.Target != t.gameObject;
        }
    }
}