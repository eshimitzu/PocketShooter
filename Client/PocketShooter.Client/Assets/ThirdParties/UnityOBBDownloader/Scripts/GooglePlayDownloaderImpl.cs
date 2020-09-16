using UnityEngine;
using System.IO;
using System;

internal class GooglePlayObbDownloader : IGooglePlayObbDownloader
{
    private static AndroidJavaClass EnvironmentClass = new AndroidJavaClass("android.os.Environment");
    private const string Environment_MediaMounted = "mounted";

    private string m_ExpansionFilePath;
    public string GetExpansionFilePath()
    {
        if (EnvironmentClass.CallStatic<string>("getExternalStorageState") != Environment_MediaMounted)
        {
            m_ExpansionFilePath = null;
            return m_ExpansionFilePath;
        }

        if (string.IsNullOrEmpty(m_ExpansionFilePath))
        {
            const string obbPath = "Android/obb";
            using (var externalStorageDirectory = EnvironmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
            {
                var externalRoot = externalStorageDirectory.Call<string>("getPath");
                m_ExpansionFilePath = string.Format("{0}/{1}/{2}", externalRoot, obbPath, ObbPackage);
            }
        }
        return m_ExpansionFilePath;
    }

    public string GetMainOBBPath()
    {
        return GetOBBPackagePath(GetExpansionFilePath(), "main");
    }

    public string GetPatchOBBPath()
    {
        return GetOBBPackagePath(GetExpansionFilePath(), "patch");
    }

    private static string GetOBBPackagePath(string expansionFilePath, string prefix)
    {
        if (string.IsNullOrEmpty(expansionFilePath))
            return null;

        var filePath = string.Format("{0}/{1}.{2}.{3}.obb", expansionFilePath, prefix, ObbVersion, ObbPackage);
        return File.Exists(filePath) ? filePath : null;
    }

    private static string m_ObbPackage;
    private static string ObbPackage
    {
        get
        {
            if (m_ObbPackage == null)
            {
                PopulateOBBProperties();
            }
            return m_ObbPackage;
        }
    }

    private static int m_ObbVersion;
    private static int ObbVersion
    {
        get
        {
            if (m_ObbVersion == 0)
            {
                PopulateOBBProperties();
            }
            return m_ObbVersion;
        }
    }

    // This code will reuse the package version from the .apk when looking for the .obb
    // Modify as appropriate
    private static void PopulateOBBProperties()
    {
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            m_ObbPackage = currentActivity.Call<string>("getPackageName");
            var packageInfo = currentActivity.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", m_ObbPackage, 0);
            m_ObbVersion = packageInfo.Get<int>("versionCode");
        }
    }
}
