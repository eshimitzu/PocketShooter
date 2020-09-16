using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class MatchResultsGrain : Grain, IMatchResultsPublisherGrain, IMatchResultsProviderGrain
    {
        private readonly IConfigurationsProvider configurationsProvider;
        private readonly IDictionary<Guid, PlayerReward> matchRewards = new Dictionary<Guid, PlayerReward>();
        private readonly IDictionary<TeamNo, List<Guid>> teamLeaderboards = new Dictionary<TeamNo, List<Guid>>();

        private IList<PlayerMatchResultsData> matchResults = null;

        public MatchResultsGrain(IConfigurationsProvider configurationsProvider)
        {
            this.configurationsProvider = configurationsProvider;
        }

        public async Task ApplyMatchResults(Immutable<IList<PlayerMatchResultsData>> resultsData)
        {
            if (!IsResultsApplied)
            {
                matchResults = resultsData.Value;

                teamLeaderboards[TeamNo.First] = await GetTeamLeaderboards(TeamNo.First, matchResults);
                teamLeaderboards[TeamNo.Second] = await GetTeamLeaderboards(TeamNo.Second, matchResults);

                foreach (var resultData in matchResults.Where(item => !item.IsBot))
                {
                    var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(resultData.PlayerId);
                    var teamPosition = teamLeaderboards[resultData.TeamNo].IndexOf(resultData.PlayerId) + 1;
                    var matchReward = await playerGrain.ApplyMatchResults(teamPosition, resultData.Kills, resultData.IsWinner);
                    matchRewards.Add(resultData.PlayerId, matchReward);
                }
            }
        }

        public Task<MatchResultsData> GetMatchResults(Guid playerId)
        {
            if (IsResultsApplied && matchRewards.TryGetValue(playerId, out var playerReward))
            {
                var playerMatchResults = matchResults.Single(item => item.PlayerId == playerId);

                return Task.FromResult(new MatchResultsData
                {
                    IsWinner = playerMatchResults.IsWinner,
                    IsDraw = playerMatchResults.IsDraw,
                    Reward = playerReward,
                    Stats = CreatePlayerMatchStats(playerMatchResults),
                    OtherPlayerStats = matchResults
                                    .Where(item => item.TeamNo == playerMatchResults.TeamNo)
                                    .Except(new[] { playerMatchResults })
                                    .Select(CreatePlayerMatchStats)
                                    .ToList(),
                });
            }

            return Task.FromResult((MatchResultsData)null);
        }

        private bool IsResultsApplied => matchResults != null;

        private async Task<List<Guid>> GetTeamLeaderboards(TeamNo teamNo, IList<PlayerMatchResultsData> resultsData)
        {
            var modeConfiguration = await configurationsProvider.GetDominationModeConfiguration();

            return
                resultsData
                .Where(item => item.TeamNo == teamNo)
                .Select(item => (item.PlayerId, MVPRating: modeConfiguration.CalculateMVPRating(item.Kills, item.Deaths)))
                .OrderByDescending(item => item.MVPRating)
                .Select(item => item.PlayerId)
                .ToList();
        }

        private PlayerMatchStatsData CreatePlayerMatchStats(PlayerMatchResultsData resultsData)
        {
            return new PlayerMatchStatsData
            {
                Nickname = resultsData.Nickname,
                TrooperClass = resultsData.TrooperClass,
                CurrentWeapon = resultsData.CurrentWeapon,
                Kills = resultsData.Kills,
                Deaths = resultsData.Deaths,
                IsMVP = teamLeaderboards[resultsData.TeamNo].First() == resultsData.PlayerId,
            };
        }
    }
}
