#if !UNITY_EDITOR && UNITY_ANDROID
#define CHECK_OBB_PERMISSION
#endif

using System;
using System.Collections;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is used for loading app throug the empty scene. It makes possible to make apk file less then 100 Mb
/// When the Split Application Binary option is enabled, the app is split the following way:
/// APK- Consists of the executables(Java and native), plug-ins, scripts, and the data for the first Scene(with the index 0).
/// OBB - Contains everything else, including all of the remaining Scenes, resources, and streaming Assets.
/// </summary>
public class Loading : MonoBehaviour
{
    private void Start()
    {
#if CHECK_OBB_PERMISSION
        LoadObb();
#else
        LoadLevel();
#endif
    }

    private IGooglePlayObbDownloader m_obbDownloader;
    private string expPath;
    private bool downloadStarted;
    private const string STORAGE_PERMISSION = "android.permission.READ_EXTERNAL_STORAGE";

#if CHECK_OBB_PERMISSION

    private void ForcePermission(Action act)
    {
        if (!AndroidPermissionsManager.IsPermissionGranted(STORAGE_PERMISSION))
        {
            Debug.Log("permission check: failed");
            AndroidPermissionsManager.RequestPermission(
                new[] { STORAGE_PERMISSION },
                new AndroidPermissionCallback(
                    grantedPermission =>
                    {
                        Debug.Log("permission granted");
                        // The permission was successfully granted, restart the change avatar routine
                        act();
                    },
                    deniedPermission =>
                    {
                        Debug.LogError("permission grant failed");
                        Application.Quit();
                    },
                    null));
        }
    }

    private void LoadObb()
    {
        m_obbDownloader = GooglePlayObbDownloadManager.GetGooglePlayObbDownloader();
        if (m_obbDownloader != null)
        {
            expPath = m_obbDownloader.GetExpansionFilePath();
            if (expPath == null)
            {
                LoadLevel();
            }
            else
            {
                string mainPath = m_obbDownloader.GetMainOBBPath();
                if (mainPath == null)
                {
                    LoadLevel();
                }
                else
                {
                    try
                    {
                        using (var str = File.OpenRead(mainPath))
                        {
                            str.ReadByte();
                        }

                        Debug.Log("\nOBB Read check success not forcing permissions \n");
                    }
                    catch (Exception e)
                    {
                        Debug.Log("\nOBB Read check failed forcing permissions \n");
                        ForcePermission(LoadObb);
                        return;
                    }

                    StartCoroutine(LoadLevelDelayed());
                }
            }
        }
        else
        {
            LoadLevel();
        }
    }
#endif

    private IEnumerator LoadLevelDelayed()
    {
        //!!! IF REMOVED Applying permision wont fix android 6 bug !!!
        yield return new WaitForSeconds(0.5f);
        ///!!!
        LoadLevel();
    }

    private void LoadLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }
}