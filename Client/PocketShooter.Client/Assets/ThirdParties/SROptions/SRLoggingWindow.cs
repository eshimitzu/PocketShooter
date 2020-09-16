using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEngine;

public class SRLoggingWindow : MonoBehaviour
{
    private Type[] providers = { typeof(ConsoleLoggerProvider), typeof(FileLoggerProvider) };
    private IReadOnlyList<string> allCategories;
    private Dictionary<LogLevel, string> logLevelNaming = new Dictionary<LogLevel, string>();

    private void OnEnable()
    {
        allCategories = MLog.GetCategories();

        logLevelNaming.Clear();
        foreach (LogLevel item in Enum.GetValues(typeof(LogLevel)))
        {
            logLevelNaming.Add(item, item.ToString());
        }

        SRDebug.Instance.PanelVisibilityChanged += SRDebug_PanelVisibilityChanged;
    }

    private void OnDisable()
    {
        SRDebug.Instance.PanelVisibilityChanged -= SRDebug_PanelVisibilityChanged;
    }

    void SRDebug_PanelVisibilityChanged(bool isVisible)
    {
        if (!isVisible)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnGUI()
    {
        var loggerConfiguration = MLog.LoggerConfig;
        var filters = new List<LogFilterConfiguration>();

        bool modified = false;

        float scale = 2;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
        GUILayout.BeginArea(new Rect(Screen.width / scale - 600, 40, 560, 60 * (logLevelNaming.Count + 1)));
        GUILayout.BeginVertical(GUI.skin.box);
        {
            // Titles
            GUILayout.BeginHorizontal(GUILayout.Width(150 * (providers.Length + 1)));
            {
                GUILayout.Label("", GUI.skin.box, GUILayout.Width(100));

                foreach (Type provider in providers)
                {
                    GUILayout.Space(20);
                    GUILayout.Space(24);
                    GUILayout.Label(provider.Name, GUI.skin.box, GUILayout.Width(150));
                    GUILayout.Space(24);
                }
            }
            GUILayout.EndHorizontal();

            foreach (var category in allCategories)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(150 * (providers.Length + 1)));
                {
                    GUILayout.Label(category, GUI.skin.box, GUILayout.Width(100));

                    foreach (Type provider in providers)
                    {
                        GUILayout.Space(20);

                        LogFilterConfiguration filter = loggerConfiguration.Filters.Find(f => string.Equals(f.Category, category) && string.Equals(f.ProviderName, provider.FullName));
                        LogLevel logLevel = filter?.Level ?? LogLevel.Trace;
                        LogLevel logLevelBeforeChanges = logLevel;

                        if (GUILayout.Button("<"))
                        {
                            logLevel = (LogLevel)Mathf.Clamp((int)logLevel - 1, (int)LogLevel.Trace, (int)LogLevel.None);
                        }
                        GUILayout.Button(logLevelNaming[logLevel], GUILayout.Width(150));
                        if (GUILayout.Button(">"))
                        {
                            logLevel = (LogLevel)Mathf.Clamp((int)logLevel + 1, (int)LogLevel.Trace, (int)LogLevel.None);
                        }

                        if (logLevel != LogLevel.Trace)
                        {
                            filter = new LogFilterConfiguration(provider.FullName, category, logLevel);
                            filters.Add(filter);
                        }

                        modified |= logLevel != logLevelBeforeChanges;
                    }
                }

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (modified)
        {
            loggerConfiguration.Filters.Clear();
            loggerConfiguration.Filters.AddRange(filters);
            loggerConfiguration.OnChanged?.Invoke(loggerConfiguration);
        }
    }
}