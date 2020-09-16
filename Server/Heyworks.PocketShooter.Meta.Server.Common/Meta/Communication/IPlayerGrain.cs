using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task Create(Immutable<CreatePlayerData> data);

        [AlwaysInterleave]
        Task<ServerPlayerState> GetState();

        Task<string> GetNickname();

        Task<string> GetGroup();

        Task UpdateClientData(string bundleId, ApplicationStoreName applicationStore, string clientVersion);

        Task UpdateGroup(string group);

        Task<MatchMakingData> GetMatchMakingData();

        Task StartMatch();

        Task<PlayerReward> ApplyMatchResults(int teamPosition, int kills, bool isWin);

        Task<Immutable<(int accountLevel, PlayerInfo info)>> GetPlayerInfo();

        Task<bool> ChangeNickname(string newNickname);

        Task<bool> CollectRepeatingReward();
    }
}