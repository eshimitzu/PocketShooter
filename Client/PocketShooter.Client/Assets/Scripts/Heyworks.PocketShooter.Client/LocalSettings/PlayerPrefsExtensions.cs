using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Heyworks.PocketShooter.Serialization;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.LocalSettings
{
    /// <summary>
    /// Class-container for extension methods of Unity's <c>PlayerPrefs</c> class.
    /// </summary>
    public static class PlayerPrefsExtensions
    {
        private const string PREFS_DEVICE_IDENTITY_KEY = "DeviceIdentity";

        /// <summary>
        /// Set the player prefs value Boolean representation. Returns false if key is not found.
        /// </summary>
        /// <param name="key"> Key to fetch value by. </param>
        /// <param name="defaultValue"> Default, returned in case of no-value. </param>
        public static bool GetPlayerPrefsBool(string key, bool defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key) != 0;
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets the player prefs value, corresponding to Boolean representation.
        /// </summary>
        /// <param name="key"> Key to fetch value by. </param>
        /// <param name="value"> Boolean value to store. </param>
        public static void SetPlayerPrefsBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="key"> Key of the preference. </param>
        /// <param name="value"> Preference value to set. </param>
        public static void SetPlayerPrefsDateTime(string key, DateTime value)
        {
            var dateTicksString = value.Ticks.ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(key, dateTicksString);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// Returns <see cref="DateTime.UtcNow"/> if failed to retrieve the key.
        /// </summary>
        /// <param name="key"> Key of the preference. </param>
        public static DateTime GetPlayerPrefsDateTime(string key)
        {
            return GetPlayerPrefsDateTime(key, DateTime.MinValue);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        /// <param name="key"> Key of the preference. </param>
        /// <param name="defaultValue"> Default value to return if the preference value has not been retrieved. </param>
        public static DateTime GetPlayerPrefsDateTime(string key, DateTime defaultValue)
        {
            var dateTicksString = PlayerPrefs.GetString(key);
            long dateTicks;

            if (long.TryParse(dateTicksString, out dateTicks))
            {
                return new DateTime(dateTicks, DateTimeKind.Utc);
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <typeparam name="T"> Type of objects stored in the list. </typeparam>
        /// <param name="key"> Key of the preference. </param>
        /// <param name="value"> Preference value to set. </param>
        /// <param name="serializer"> Serializer used to serialize the specified list. </param>
        public static void SetPlayerPrefsList<T>(string key, IList<T> value, IDataSerializer serializer)
        {
            SetPlayerPrefsObject(key, value, serializer);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        /// <typeparam name="T"> Type of objects stored in the list. </typeparam>
        /// <param name="key"> Key of the preference. </param>
        /// <param name="serializer"> Serializer used to deserialize the specified list. </param>
        public static IList<T> GetPlayerPrefsList<T>(string key, IDataSerializer serializer)
        {
            var list = GetPlayerPrefsObject<IList<T>>(key, serializer);
            if (list == null)
            {
                list = new T[0];
            }

            return list;
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <typeparam name="T"> Type of objects stored in the list. </typeparam>
        /// <param name="key"> Key of the preference. </param>
        /// <param name="value"> Preference value to set. </param>
        /// <param name="serializer"> Serializer used to serialize the specified list. </param>
        public static void SetPlayerPrefsObject<T>(string key, T value, IDataSerializer serializer)
        {
            try
            {
                var serializedObject = serializer.Serialize(value);
                PlayerPrefs.SetString(key, serializedObject);
            }
            catch (SerializationException e)
            {
                GameLog.Log.Log(LogLevel.Error, $"Exception while serializing the list to the player prefs key {key}: {e.Message}");
            }
        }

        /// <summary>
        /// Returns the value corresponding to key in the preference file if it exists.
        /// </summary>
        /// <typeparam name="T"> Type of objects stored in the list. </typeparam>
        /// <param name="key"> Key of the preference. </param>
        /// <param name="serializer"> Serializer used to deserialize the specified list. </param>
        public static T GetPlayerPrefsObject<T>(string key, IDataSerializer serializer)
        {
            try
            {
                var objectSerialized = PlayerPrefs.GetString(key, string.Empty);
                if (!string.IsNullOrEmpty(objectSerialized))
                {
                    return serializer.Deserialize<T>(objectSerialized);
                }

                return default(T);
            }
            catch (SerializationException e)
            {
                GameLog.Log.Log(LogLevel.Error, $"Exception while serializing the list to the player prefs key {key}: {e.Message}");
            }

            return default(T);
        }

        /// <summary>
        /// Deletes all keys except player device id key
        /// </summary>
        public static void DeleteAllExceptDeviceId()
        {
            var prefsDeviceId = PlayerPrefs.GetString(PREFS_DEVICE_IDENTITY_KEY, string.Empty);
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString(PREFS_DEVICE_IDENTITY_KEY, prefsDeviceId);
            PlayerPrefs.Save();
        }
    }
}