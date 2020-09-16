using System;

namespace Heyworks.PocketShooter.SocialConnections.Core
{
    /// <summary>
    /// Social Login Token. Our internal token, used by server for login instead of social network credentials.
    /// </summary>
    [Serializable]
    public sealed class SocialLoginToken
    {
        public string Token { get; private set; }

        public DateTime ExpirationDate { get; private set; }

        public SocialLoginToken(string token, DateTime expirationDate)
        {
            Token = token;
            ExpirationDate = expirationDate;
        }
    }
}
