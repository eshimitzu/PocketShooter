#if UNITY_IOS

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public class ChangeXcodePlistPostProcessor
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            //// Get root
            PlistElementDict rootDict = plist.root;

            //// Change value of ITSAppUsesNonExemptEncryption in Xcode plist
            var buildKey = "ITSAppUsesNonExemptEncryption";
            rootDict.SetString(buildKey, "false");

            //// Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}

#endif