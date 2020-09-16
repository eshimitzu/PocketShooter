using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEngine;

public partial class SROptions
{
    private GameObject loggingWindow;

    [Category("Logging")]
    public void Show()
    {
        if(!loggingWindow)
        {
            loggingWindow = new GameObject("SRLoggingWindow");
            loggingWindow.AddComponent<SRLoggingWindow>();
            loggingWindow.hideFlags = HideFlags.NotEditable;
        }
    }

    [Category("Logging")]
    public void Hide()
    {
        if (loggingWindow)
        {
            UnityEngine.Object.Destroy(loggingWindow);
            loggingWindow = null;
        }
    }

    [Category("Logging")]
    public void DisableAll()
    {
        Type[] providers = { typeof(ConsoleLoggerProvider), typeof(FileLoggerProvider) };
        var allCategories = MLog.GetCategories();


        var filters = new List<LogFilterConfiguration>();
        foreach (var category in allCategories)
        {
            foreach (Type provider in providers)
            {
                LogFilterConfiguration filter = new LogFilterConfiguration(provider.FullName, category, LogLevel.None);
                filters.Add(filter);
            }
        }

        var config = MLog.LoggerConfig;
        config.Filters.Clear();
        config.Filters.AddRange(filters);
        config.OnChanged?.Invoke(config);
    }

    [Category("Logging")]
    public void Reload()
    {
        MLog.LoggerConfig.Reload();
    }
}