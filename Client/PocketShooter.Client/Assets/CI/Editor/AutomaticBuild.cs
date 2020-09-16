#region Copyright Notice & License Information
//
// AutomaticBuild.cs
//
// Author:
//       Josh Montoute
//       Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (c) 2011 by Thinksquirrel Software, LLC
// Copyright (c) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AI;
using UnityEditor.SceneManagement;
using Heyworks.PocketShooter;
using Heyworks.PocketShooter.VersionUtility.Versions;
using Heyworks.PocketShooter.VersionUtility;

public static class AutomaticBuild
{
    private const string BUILD_PATH_ARG = "BuildPath";
    private const string BUNDLE_IDENTIFIER_ARG = "BundleIdentifier";
    private const string CLIENT_VERSION_ARG = "ClientVersion";
    private const string VERSION_NAME_ARG = "VersionName";
    private const string METASERVER_IP_ARG = "MetaServerIp";
    
         

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }   

    static string[] GetScenePaths()
    {
        var EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes )
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);            
        }
        return EditorScenes.ToArray();
    }

    static string[] GetScenePathsWithTestScene()
    {
        var EditorScenes = new List<string>();
        EditorScenes.Add("Assets/Scenes/Tests.unity");
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes )
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);            
        }
        return EditorScenes.ToArray();
    }

    [MenuItem("File/Automatic Build/Windows (32 bit)")]
    public static void PerformWin32Build()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        var path = CommandLineReader.GetCustomArgument(BUILD_PATH_ARG);
        path = !string.IsNullOrEmpty(path) ? path: "Builds/Win-32/"+ GetProjectName() + ".exe";

        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("File/Automatic Build/Windows (64 bit)")]
    public static void PerformWin64Build()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        
        var path = CommandLineReader.GetCustomArgument(BUILD_PATH_ARG);
        path = !string.IsNullOrEmpty(path) ? path : "Builds/Win-64/" + GetProjectName() + ".exe";

        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }


    [MenuItem("File/Automatic Build/iOS")]
    public static void PerformiOSBuild()
    {
        var scenes = GetScenePaths();
        foreach (var scene in scenes)
        {
            EditorSceneManager.OpenScene(scene);
            NavMeshBuilder.BuildNavMesh();
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

        var versionName = (VersionName)Enum.Parse(typeof(VersionName), CommandLineReader.GetCustomArgument(VERSION_NAME_ARG));
        var clientVersion =  CommandLineReader.GetCustomArgument(CLIENT_VERSION_ARG);
        var bundleId =  CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);
        var metaServerIp = CommandLineReader.GetCustomArgument(METASERVER_IP_ARG);
        ApplyVersion(versionName, PlatformName.iOS, clientVersion, bundleId, metaServerIp);
        
        PlayerSettings.applicationIdentifier = CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);

        var path = CommandLineReader.GetCustomArgument(BUILD_PATH_ARG);
        path = !string.IsNullOrEmpty(path) ? path : "Builds/iOS/" + GetProjectName() + ".app";

        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.iOS, BuildOptions.None);
    }

    public static void PerformProdAndroidBuild()
    {
        var scenes = GetScenePaths();
        foreach (var scene in scenes)
        {
            EditorSceneManager.OpenScene(scene);
            NavMeshBuilder.BuildNavMesh();
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        var clientVersion =  CommandLineReader.GetCustomArgument(CLIENT_VERSION_ARG);                        
        //ApplyVersion(VersionName.Prod, PlatformName.Android, clientVersion);
        
        PlayerSettings.applicationIdentifier = CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);
                
        PlayerSettings.Android.keyaliasName = CommandLineReader.GetCustomArgument("KeyAliasName");
        PlayerSettings.Android.keystoreName = CommandLineReader.GetCustomArgument("KeyStoreName");
        PlayerSettings.Android.keystorePass = CommandLineReader.GetCustomArgument("KeyStorePass");
        PlayerSettings.Android.keyaliasPass = CommandLineReader.GetCustomArgument("KeyAliasPass");
        PlayerSettings.Android.bundleVersionCode = int.Parse(CommandLineReader.GetCustomArgument("VersionCode"));                
        PlayerSettings.Android.useAPKExpansionFiles = true;
        
        var path = CommandLineReader.GetCustomArgument(BUILD_PATH_ARG);
        path = !string.IsNullOrEmpty(path) ? path : "Builds/Android/" + GetProjectName() + ".apk";        
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.None);
    }
    
    public static void PerformAndroidBuild()
    {
        var scenes = GetScenePaths();
        foreach (var scene in scenes)
        {
            EditorSceneManager.OpenScene(scene);
            NavMeshBuilder.BuildNavMesh();
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        
        var versionName = (VersionName)Enum.Parse(typeof(VersionName), CommandLineReader.GetCustomArgument(VERSION_NAME_ARG));
        var clientVersion =  CommandLineReader.GetCustomArgument(CLIENT_VERSION_ARG);
        var bundleId =  CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);
        var metaServerIp = CommandLineReader.GetCustomArgument(METASERVER_IP_ARG);
        ApplyVersion(versionName, PlatformName.Android, clientVersion, bundleId, metaServerIp);        
        
        PlayerSettings.Android.keyaliasName = CommandLineReader.GetCustomArgument("KeyAliasName");
        PlayerSettings.Android.keystoreName = CommandLineReader.GetCustomArgument("KeyStoreName");
        PlayerSettings.Android.keystorePass = CommandLineReader.GetCustomArgument("KeyStorePass");
        PlayerSettings.Android.keyaliasPass = CommandLineReader.GetCustomArgument("KeyAliasPass");
        PlayerSettings.Android.bundleVersionCode = int.Parse(CommandLineReader.GetCustomArgument("VersionCode"));
        PlayerSettings.applicationIdentifier = CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);
        
        PlayerSettings.Android.useAPKExpansionFiles = CommandLineReader.GetCustomArgument("UseAPKExpansionFiles") == "True" ?  true : false;
        
        var path = CommandLineReader.GetCustomArgument(BUILD_PATH_ARG);        
        path = !string.IsNullOrEmpty(path) ? path : "Builds/Android/" + GetProjectName() + ".apk";

        var debugBuild = CommandLineReader.GetCustomArgument("DebugBuild");
        if (debugBuild == "True")
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.Development);
        }
        else
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.None);
        }
    }

    public static void PerformAndroidBuildForTest()
    {
        var scenes = GetScenePaths();
        foreach (var scene in scenes)
        {
            EditorSceneManager.OpenScene(scene);
            NavMeshBuilder.BuildNavMesh();
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        
        var versionName = (VersionName)Enum.Parse(typeof(VersionName), CommandLineReader.GetCustomArgument(VERSION_NAME_ARG));
        var clientVersion =  CommandLineReader.GetCustomArgument(CLIENT_VERSION_ARG);
        var bundleId =  CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);
        var metaServerIp = CommandLineReader.GetCustomArgument(METASERVER_IP_ARG);
        ApplyVersion(versionName, PlatformName.Android, clientVersion, bundleId, metaServerIp);        
        
        PlayerSettings.Android.keyaliasName = CommandLineReader.GetCustomArgument("KeyAliasName");
        PlayerSettings.Android.keystoreName = CommandLineReader.GetCustomArgument("KeyStoreName");
        PlayerSettings.Android.keystorePass = CommandLineReader.GetCustomArgument("KeyStorePass");
        PlayerSettings.Android.keyaliasPass = CommandLineReader.GetCustomArgument("KeyAliasPass");
        PlayerSettings.Android.bundleVersionCode = int.Parse(CommandLineReader.GetCustomArgument("VersionCode"));
        PlayerSettings.applicationIdentifier = CommandLineReader.GetCustomArgument(BUNDLE_IDENTIFIER_ARG);
        
        PlayerSettings.Android.useAPKExpansionFiles = CommandLineReader.GetCustomArgument("UseAPKExpansionFiles") == "True" ?  true : false;
        
        var path = CommandLineReader.GetCustomArgument(BUILD_PATH_ARG);        
        path = !string.IsNullOrEmpty(path) ? path : "Builds/Android/" + GetProjectName() + ".apk";

        var debugBuild = CommandLineReader.GetCustomArgument("DebugBuild");
        
        if (debugBuild == "True")
        {
            BuildPipeline.BuildPlayer(GetScenePathsWithTestScene(), path, BuildTarget.Android, BuildOptions.Development);
        }
        else
        {
            BuildPipeline.BuildPlayer(GetScenePathsWithTestScene(), path, BuildTarget.Android, BuildOptions.None);
        }
    }   

    public static void BuildAndroidToSelectedFolder()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Select build folder", "", "");

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, BuildOptions.None);
    }

    public static void BuildiOSToSelectedFolder()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Select build folder", "", "");

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.iOS, BuildOptions.None);
    }

    private static void ApplyVersion(VersionName versionName, PlatformName platforName, string clientVersion, string bundleId, string metaServerIp)
       {
           VersionManager versionManager = new VersionManager();
           versionManager.SetCurrentVersion(versionName, platforName);
           var version = versionManager.GetCurrentVersion();
           version.CommonSettings.AppConfiguration.Version = clientVersion;
           version.CommonSettings.AppConfiguration.BundleId = bundleId;
           version.CommonSettings.AppConfiguration.MetaServerIp = metaServerIp;
           versionManager.SaveAppConfiguration();
           ProjectSettingsManager.WriteSettingsToEditor(version);
       }
}