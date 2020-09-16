using Heyworks.PocketShooter.Meta.Communication;
using UniRx.Async;

namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    /// <summary>
    /// Represents an object which has direct access to some social network.
    /// </summary>
    public interface ISocialNetwork
    {
        /// <summary>
        /// Gets a social network name
        /// </summary>
        SocialNetworkName Name { get; }

        /// <summary>
        /// Gets the social information about logged in player.
        /// </summary>
        SocialNetworkUser Player { get; }

        /// <summary>
        /// Gets a value indicating whether the player is currently logged in and connected to the social network.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Starts the social network initialization.
        /// </summary>
        UniTask Initialize();

        /// <summary>
        /// Log into the social network.
        /// </summary>
        UniTask<ResponseOption<SocialNetworkAccessData>> LogIn();

        /// <summary>
        /// Log out of the social network.
        /// </summary>
        void LogOut();
    }
}