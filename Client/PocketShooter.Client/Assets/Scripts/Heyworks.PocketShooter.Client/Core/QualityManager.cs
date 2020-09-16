using System;
using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.Rendering;

namespace Heyworks.PocketShooter.Core
{
    /// <summary>
    /// Represents quality manager for setting up quality settings at the startup.
    /// </summary>
    public class QualityManager : MonoBehaviour
    {
        [SerializeField]
        private int defaultQualityLevel = 3;

        [SerializeField]
        private int lowFPS = 27;

        [SerializeField]
        private int normalFPS = 55;

        [SerializeField]
        private int targetFrameRate = 60;

        [SerializeField]
        private int qualitySwitchInterval = 15;

        [SerializeField]
        private int FPSSampleSize = 20;

        private float[] fpsSamples;
        private int sampleIndex;
        private int _fps;

        private int QualityLevel => QualitySettings.GetQualityLevel();

        private bool allowedMSAA = true;

        private void Start()
        {
            Application.targetFrameRate = targetFrameRate;

            fpsSamples = new float[FPSSampleSize];
            for (int i = 0; i < fpsSamples.Length; i++)
            {
                fpsSamples[i] = 0.001f;
            }

            allowedMSAA = QualitySettings.antiAliasing > 0;
            if (allowedMSAA && SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
            {
                var device = SystemInfo.deviceModel;
                switch (device)
                {
                    case "iPad4,1": // ipad air
                    case "iPad4,2": // ipad air
                    case "iPhone6,1": // 5s
                    case "iPhone6,2": // 5s
                    case "iPhone7,1": // 6
                    case "iPhone7,2": // 6s
                        allowedMSAA = false;
                        break;
                }
            }

            if (!allowedMSAA)
            {
                QualitySettings.antiAliasing = 0;
            }

            StartCoroutine(CheckQuality());

#if !UNITY_EDITOR
            if (Debug.isDebugBuild)
            {
                StackTraceLogType stacktrace = Application.GetStackTraceLogType(LogType.Log);
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

                Debug.Log($"===================== SystemInfo =====================");
                Type systemInfoType = typeof(SystemInfo);
                var props = systemInfoType.GetProperties();
                foreach (PropertyInfo propertyInfo in props)
                {
                    Debug.Log($"{propertyInfo.Name} = {propertyInfo.GetValue(null)}");
                }

                Application.SetStackTraceLogType(LogType.Log, stacktrace);
            }
#endif
        }

        private IEnumerator CheckQuality()
        {
            yield return new WaitForSeconds(qualitySwitchInterval);

            while (true)
            {
                _fps = CalculateFps();

                if (_fps < lowFPS && QualityLevel > 0)
                {
                    QualitySettings.DecreaseLevel(true);
                    if (!allowedMSAA)
                    {
                        QualitySettings.antiAliasing = 0;
                    }

                    GraphicsLog.Debug("Low FPS({_fps}) : Decrease Quality to {QualityLevel}", _fps, QualityLevel);
                    Debug.Log($"Low FPS({_fps}) : Decrease Quality to {QualityLevel}");
                }

                if (_fps > normalFPS && QualityLevel < defaultQualityLevel)
                {
                    QualitySettings.IncreaseLevel(true);
                    if (!allowedMSAA)
                    {
                        QualitySettings.antiAliasing = 0;
                    }

                    GraphicsLog.Debug("Normal FPS({_fps}) : Increase Quality to {QualityLevel}", _fps, QualityLevel);
                    Debug.Log($"Normal FPS({_fps}) : Increase Quality to {QualityLevel}");
                }

                yield return new WaitForSeconds(qualitySwitchInterval);
            }
        }

        private void Update()
        {
            fpsSamples[sampleIndex] = 1 / Time.smoothDeltaTime;
            sampleIndex++;
            sampleIndex = sampleIndex % FPSSampleSize;
        }

        private int CalculateFps()
        {
            float fps = 0;
            bool loop = true;
            int i = 0;
            while (loop)
            {
                if (i == FPSSampleSize - 1)
                {
                    loop = false;
                }

                fps += fpsSamples[i];
                i++;
            }

            fps /= fpsSamples.Length;

            return (int)fps;
        }
    }
}