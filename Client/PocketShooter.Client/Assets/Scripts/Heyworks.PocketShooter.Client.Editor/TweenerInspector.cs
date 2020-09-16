using System.Globalization;
using Heyworks.PocketShooter.UI.Animation;
using Heyworks.PocketShooter.Utils.Extensions;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter
{
    public abstract class TweenerInspector : Editor
    {
        protected Color defaultContentColor;
        protected Tweener tween;

        private float factor = 1f;

        public virtual void Awake()
        {
            defaultContentColor = GUI.contentColor;
            tween = (Tweener)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Method", true, GUILayout.Width(150f));
            tween.Method = (Method)EditorGUILayout.EnumPopup(tween.Method);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Steeper curves", true, GUILayout.Width(150f));
            tween.SteeperCurves = EditorGUILayout.Toggle(tween.SteeperCurves);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("NotUsedForScreenAnimatingCondition", true, GUILayout.Width(150f));
            tween.NotUsedForScreenAnimatingCondition = EditorGUILayout.Toggle(tween.NotUsedForScreenAnimatingCondition);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Style", true, GUILayout.Width(150f));
            tween.Style = (Style)EditorGUILayout.EnumPopup(tween.Style);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Curve", true, GUILayout.Width(150f));
            tween.UseCurve = EditorTools.DrawToggle(tween.UseCurve, string.Empty, "Use curve", true, 15f);
            tween.AnimationCurve = EditorGUILayout.CurveField(tween.AnimationCurve);
            tween.AnimationCurve.RoundEdges();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Ignore time scale", true, GUILayout.Width(150f));
            tween.IgnoreTimeScale = EditorGUILayout.Toggle(tween.IgnoreTimeScale);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Set Begin State At Start", true, GUILayout.Width(150f));
            tween.ShouldSetBeginStateAtStart = EditorGUILayout.Toggle(tween.ShouldSetBeginStateAtStart);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Duration", true, GUILayout.Width(150f));
            const float defaultDuration = 1f;
            if (EditorTools.DrawButton(defaultDuration.ToString(CultureInfo.CurrentCulture), ("Set duration to " + defaultDuration), true, 20f))
            {
                tween.Duration = defaultDuration;
            }

            tween.Duration = EditorGUILayout.Slider(tween.Duration, 0f, 10f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorTools.DrawLabel("Scale duration", true, GUILayout.Width(150f));
            const float defaultScaleDuration = 1f;
            if (EditorTools.DrawButton(
                defaultScaleDuration.ToString(CultureInfo.CurrentCulture),
                ("Set scale duration to " + defaultScaleDuration),
                true,
                20f))
            {
                tween.ScaleDuration = defaultScaleDuration;
            }

            tween.ScaleDuration = EditorGUILayout.Slider(tween.ScaleDuration, 0f, 5f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PrefixLabel("Call when finished");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Script");
            EditorGUILayout.LabelField("Call method");
            EditorGUILayout.EndHorizontal();
            EditorTools.DrawInvokeData(tween.InvokeWhenFinished);
            Color currentContentColor = GUI.contentColor;
            GUI.contentColor = Color.cyan;
            EditorGUILayout.LabelField(
                "============================================================================================");
            GUI.contentColor = currentContentColor;
            CustomInspectorGUI();
            GUI.contentColor = Color.cyan;
            EditorGUILayout.LabelField(
                "============================================================================================");
            GUI.contentColor = currentContentColor;
            if (Application.isPlaying)
            {
                DrawRunButtons(tween);
            }
            else
            {
                DrawStateSlider(tween);
            }

            EditorGUILayout.Space();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        protected void DrawRunButtons(Tweener t)
        {
            EditorGUILayout.BeginHorizontal();
            Color defaultColor = GUI.skin.button.normal.textColor;
            GUI.skin.button.normal.textColor = t.IsBeginStateSet ? Color.green : defaultColor;
            if (EditorTools.DrawButton("Begin " + t.GetType().ToString().Replace("Tween", string.Empty).ToLower()))
            {
                t.SetBeginState();
            }

            GUI.skin.button.normal.textColor = t.IsEndStateSet ? Color.green : defaultColor;
            if (EditorTools.DrawButton("End " + t.GetType().ToString().Replace("Tween", string.Empty).ToLower()))
            {
                t.SetEndState();
            }

            GUI.skin.button.normal.textColor = defaultColor;
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawStateSlider(Tweener t)
        {
            EditorGUILayout.BeginHorizontal();
            string state = (factor == 0f) ? "Begin" : ((factor == 1f) ? "End" : "Middle");
            GUILayout.Label(state, GUILayout.MaxWidth(40f));
            float newFactor = EditorGUILayout.Slider(factor, 0f, 1f);
            if (newFactor != factor)
            {
                factor = newFactor;
                t.Sample(factor, false);
            }

            EditorGUILayout.EndHorizontal();
        }

        protected abstract void CustomInspectorGUI();
    }
}