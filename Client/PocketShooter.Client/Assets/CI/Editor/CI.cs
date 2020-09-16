using System.Linq;
using Heyworks.PocketShooter.Diagnostics;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents class for building unity projects from CI servers.
/// </summary>
public class CI
{
    private static readonly string[] ScenesInBuild =
    {
            "Assets/Scenes/Loading.unity",
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/MexicoNight.unity",
            "Assets/Scenes/Peru.unity",
    };

    private static readonly string[] ScenesInBuildForTests =
    {
            "Assets/Scenes/Tests.unity",
            "Assets/Scenes/Loading.unity",
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/MexicoNight.unity",
            "Assets/Scenes/Peru.unity",
    };

    public static void BuildIOS()
    {
        SetupProjectFromCommandLineArguments();
        AutomaticBuild.PerformiOSBuild();
    }

    public static void BuildAndroid()
    {
        SetupProjectFromCommandLineArguments();
        AutomaticBuild.PerformAndroidBuild();
    }

    public static void BuildWin32()
    {
        SetupProjectFromCommandLineArguments();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        AutomaticBuild.PerformWin32Build();
    }

    public static void BuildWin64()
    {
        SetupProjectFromCommandLineArguments();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        AutomaticBuild.PerformWin64Build();
    }

    private static void SetupProjectFromCommandLineArguments()
    {
        var bundleId = CommandLineReader.GetCustomArgument("BundleId");

        if (!string.IsNullOrEmpty(bundleId))
        {
            PlayerSettings.applicationIdentifier = bundleId;
        }
    }

    public void SetupScenesForTest()
    {
        SetupScenes(ScenesInBuildForTests);
    }

    public void SetupScenes()
    {
        SetupScenes(ScenesInBuild);
    }

    private static void SetupScenes(string[] activeScenes)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            scene.enabled = activeScenes.Contains(scene.path);
        }
        EditorBuildSettings.scenes = scenes;
    }
}
