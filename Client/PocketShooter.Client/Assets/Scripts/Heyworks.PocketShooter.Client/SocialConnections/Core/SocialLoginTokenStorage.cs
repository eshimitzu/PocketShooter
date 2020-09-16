using System;
using Heyworks.PocketShooter.LocalSettings;
using Heyworks.PocketShooter.Serialization;
using Heyworks.PocketShooter.Utils;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.SocialConnections.Core
{
    public static class SocialLoginTokenStorage
    {
        private const int DESYNC_TIME_IN_MINUTES = 1;
        private const string PLAYER_PREFS_TOKEN_KEY_MASK = "SLToken_{0}";
        private static readonly IDataSerializer Serializer = new JSONSerializer();

        public static void StoreToken(string socialUserId, SocialLoginToken token)
        {
            AssertUtils.NotNull(socialUserId, "Social ID");
            AssertUtils.NotNull(token, "Social Login Token");

            AuthLog.Log.LogInformation("Login Token Storage ### Store Token for socialID:{0} Token:{1} ExpDate:{2}", socialUserId, token.Token, token.ExpirationDate.ToString(@"dd\/MM\/yyyy HH:mm"));

            PlayerPrefsExtensions.SetPlayerPrefsObject(string.Format(PLAYER_PREFS_TOKEN_KEY_MASK, socialUserId), token, Serializer);
        }

        public static string GetToken(string socialUserId)
        {
            string tokenKey = string.Format(PLAYER_PREFS_TOKEN_KEY_MASK, socialUserId);

            AuthLog.Log.LogInformation("Login Token Storage ### Trying to get token for socialID:{0}", socialUserId);

            var tokenObject = PlayerPrefsExtensions.GetPlayerPrefsObject<SocialLoginToken>(tokenKey, Serializer);

            if (tokenObject != null)
            {
                if (!IsTokenValid(tokenObject.ExpirationDate))
                {
                    AuthLog.Log.LogInformation("Login Token Storage ### Token for socialID:{0} has expired! Token has removed from storage.", socialUserId);

                    DeleteToken(socialUserId);
                    return null;
                }

                AuthLog.Log.LogInformation("Login Token Storage ### Token for socialID:{0} found and it's valid.", socialUserId);
            }
            else
            {
                AuthLog.Log.LogInformation("Login Token Storage ### Token for socialID:{0} not found.", socialUserId);
            }

            return tokenObject?.Token;
        }

        private static void DeleteToken(string socialUserId)
        {
            string tokenKey = string.Format(PLAYER_PREFS_TOKEN_KEY_MASK, socialUserId);

            AuthLog.Log.LogInformation("Login Token Storage ### Delete Token for socialID:{0}.", socialUserId);

            PlayerPrefs.DeleteKey(tokenKey);
        }

        private static bool IsTokenValid(DateTime expirationDate)
        {
            TimeSpan remainingTime = expirationDate - DateTime.UtcNow - TimeSpan.FromMinutes(DESYNC_TIME_IN_MINUTES);
            return remainingTime > TimeSpan.Zero;
        }
    }
}
