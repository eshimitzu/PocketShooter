using System;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Static class, which provides the static method, which returns the device identifier.
    /// </summary>
    public static class DeviceIdentity
    {
        private const string PrefsDeviceIdentityKey = "DeviceIdentity";
        private static string deviceId = string.Empty;

        /// <summary>
        /// Gets the device id to perform  registration and login.
        /// </summary>
        /// <returns> Device ID with some postfix, specified in the DebugConfiguration. </returns>
        public static string GetDeviceId()
        {
            if (!string.IsNullOrEmpty(deviceId))
            {
                GameLog.Information("Device identity was already initialized. Returning device id {deviceId}", deviceId);

                return deviceId;
            }

            // Read first from player prefs.
            var prefsDeviceId = PlayerPrefs.GetString(PrefsDeviceIdentityKey, string.Empty);
            GameLog.Information("Reading device id from player prefs: {prefsDeviceId}", prefsDeviceId);

            if (string.IsNullOrEmpty(prefsDeviceId))
            {
                GameLog.Information("Player prefs and storage are empty. This is a new installation. Generate device identity and store it.");

                var guid = Guid.NewGuid();
                deviceId = guid.ToString();

                PlayerPrefs.SetString(PrefsDeviceIdentityKey, deviceId);
            }
            else
            {
                GameLog.Information("Player pref is not empty. Using prefs device id {prefsDeviceId}", prefsDeviceId);

                deviceId = prefsDeviceId;
            }

            GameLog.Information("Returning device id: {deviceId}", deviceId);

            return deviceId;
        }

        /// <summary>
        /// Gets the Application Store Name.
        /// </summary>
        public static ApplicationStoreName GetApplicationStoreName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return ApplicationStoreName.Apple;
                case RuntimePlatform.Android:
                    return ApplicationStoreName.Google;
                default:
                    return ApplicationStoreName.Google;
            }
        }
    }
}
