#pragma warning disable SA1649 // File name should match first type name
using System;
using Heyworks.PocketShooter.Realtime.Simulation;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public struct RemotePlayerJoinedEvent : IServerEvent
    {
        public RemotePlayerJoinedEvent(IRemotePlayer remotePlayer, TrooperClass trooperClass, WeaponName weaponName)
        {
            RemotePlayer = remotePlayer;
            TrooperClass = trooperClass;
            WeaponName = weaponName;
        }

        public IRemotePlayer RemotePlayer { get; }

        public TrooperClass TrooperClass { get; }

        public WeaponName WeaponName { get; }
    }

    public struct RemotePlayerLeavedEvent : IServerEvent
    {
        public RemotePlayerLeavedEvent(IRemotePlayer remotePlayer)
        {
            RemotePlayer = remotePlayer;
        }

        public IRemotePlayer RemotePlayer { get; }
    }

    public struct ResetWorldEvent : IServerEvent
    {
    }

    public struct RemotePlayerRespawnedEvent : IServerEvent
    {
        public RemotePlayerRespawnedEvent(IRemotePlayer remotePlayer, TrooperClass trooperClass, WeaponName weaponName)
        {
            RemotePlayer = remotePlayer;
            TrooperClass = trooperClass;
            WeaponName = weaponName;
        }

        public IRemotePlayer RemotePlayer { get; }

        public TrooperClass TrooperClass { get; }

        public WeaponName WeaponName { get; }


    }

    public struct LocalPlayerSpawnedEvent : IServerEvent
    {
        public LocalPlayerSpawnedEvent(IClientPlayer clientPlayer)
        {
            ClientPlayer = clientPlayer;
        }

        public IClientPlayer ClientPlayer { get; }

        public override string ToString() =>
            $"{nameof(LocalPlayerSpawnedEvent)}{(ClientPlayer.Id, ClientPlayer.TrooperClass, ClientPlayer.CurrentWeapon.Name, ClientPlayer.Transform.Position)}";
    }

    public struct LocalPlayerRespawnedEvent : IServerEvent
    {
        public LocalPlayerRespawnedEvent(IClientPlayer clientPlayer)
        {
            ClientPlayer = clientPlayer.NotNull();
        }

        public IClientPlayer ClientPlayer { get; }

        public override string ToString() =>
            $"{nameof(LocalPlayerRespawnedEvent)}{(ClientPlayer.Id, ClientPlayer.TrooperClass, ClientPlayer.CurrentWeapon.Name, ClientPlayer.Transform.Position)}";
    }
}
#pragma warning restore SA1649 // File name should match first type name
