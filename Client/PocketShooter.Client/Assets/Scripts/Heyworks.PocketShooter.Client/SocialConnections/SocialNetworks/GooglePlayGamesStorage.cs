using UnityEngine;

namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    public static class GooglePlayGamesStorage
    {
        private const string PLAYER_PREFS_TOKEN_KEY_MASK = "GPGToken_{0}";
        private const string PLAYER_PREFS_CONNECTED_STATE_KEY = "GPGConnected";

        public static bool IsTokenExist(string userId)
        {
            return PlayerPrefs.HasKey(string.Format(PLAYER_PREFS_TOKEN_KEY_MASK, userId));
        }

        public static void SaveToken(string userId, string token)
        {
            PlayerPrefs.SetString(string.Format(PLAYER_PREFS_TOKEN_KEY_MASK, userId), token);
        }

        public static string LoadToken(string userId)
        {
            return PlayerPrefs.GetString(string.Format(PLAYER_PREFS_TOKEN_KEY_MASK, userId), string.Empty);
        }

        public static bool IsServiceConnected()
        {
            return PlayerPrefs.HasKey(PLAYER_PREFS_CONNECTED_STATE_KEY);
        }

        public static void SetServiceConnectedState()
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_CONNECTED_STATE_KEY, 1);
        }

        public static void ResetServiceConnectedState()
        {
            PlayerPrefs.DeleteKey(PLAYER_PREFS_CONNECTED_STATE_KEY);
        }
    }

}