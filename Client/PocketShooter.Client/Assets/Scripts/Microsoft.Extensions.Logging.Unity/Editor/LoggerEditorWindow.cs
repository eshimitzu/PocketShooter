using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Logger Editor Window.
    /// </summary>
    public class LoggerEditorWindow : EditorWindow
    {
        [NonSerialized]
        private LoggerConfiguration loggerConfiguration = null;
        private Type[] providers = { typeof(ConsoleLoggerProvider), typeof(FileLoggerProvider) };
        private IReadOnlyList<string> allCategories;

        /// <summary>
        /// Opens the window.
        /// </summary>
        [MenuItem("Tools/Microsoft Logger Configuration")]
        public static void OpenWindow()
        {
            var window = GetWindow<LoggerEditorWindow>("LoggerConfiguration");
            window.Show();
        }

        /// <summary>
        /// GUI to setup logging filters and options.
        /// </summary>
        public void OnGUI()
        {
            name = nameof(LoggerConfiguration);

            var filters = new List<LogFilterConfiguration>();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                // Titles
                EditorGUILayout.BeginHorizontal(GUILayout.Width(150 * (providers.Length + 1)));
                {
                    EditorGUILayout.LabelField(string.Empty, GUILayout.Width(140));

                    foreach (Type provider in providers)
                    {
                        EditorGUILayout.LabelField(provider.Name, GUI.skin.box, GUILayout.Width(150));
                    }
                }

                EditorGUILayout.EndHorizontal();

                foreach (var category in allCategories)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(150 * (providers.Length + 1)));
                    {
                        EditorGUILayout.LabelField(category, GUI.skin.box, GUILayout.Width(100));

                        foreach (Type provider in providers)
                        {
                            LogFilterConfiguration filter = loggerConfiguration.Filters.Find(f => string.Equals(f.Category, category) && string.Equals(f.ProviderName, provider.FullName));
                            LogLevel logLevel = filter?.Level ?? LogLevel.Trace;
                            logLevel = (LogLevel)EditorGUILayout.EnumPopup(logLevel, GUILayout.Width(150));
                            if (logLevel != LogLevel.Trace)
                            {
                                filter = new LogFilterConfiguration(provider.FullName, category, logLevel);
                                filters.Add(filter);
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            bool modified = EditorGUI.EndChangeCheck();

            if (modified)
            {
                loggerConfiguration.Filters.Clear();
                loggerConfiguration.Filters.AddRange(filters);
                loggerConfiguration.Save();
                MLog.LoggerConfig?.Reload();
            }
        }

        private void OnEnable()
        {
            loggerConfiguration = loggerConfiguration ?? new LoggerConfiguration();
            allCategories = MLog.GetCategories();
        }
    }
}
#endif