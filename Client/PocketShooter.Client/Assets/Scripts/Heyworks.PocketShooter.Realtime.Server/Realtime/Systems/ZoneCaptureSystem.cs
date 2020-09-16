using System;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    internal class ZoneCaptureSystem : IServerGameSystem
    {
        private readonly ITicker ticker;
        private readonly int checkInterval;
        private readonly int timeplayersToCapture;

        private int lastUpdate;

        public ZoneCaptureSystem(ITicker ticker, int checkInterval, int timeplayersToCapture)
        {
            this.ticker = ticker;
            this.checkInterval = checkInterval;
            this.timeplayersToCapture = timeplayersToCapture;
        }

        public bool Execute(IServerGame game)
        {
            int currentTime = ticker.Current;
            if (currentTime - lastUpdate < checkInterval)
            {
                return true;
            }

            lastUpdate = currentTime;

            CaptureZones(game);
            ScorePoints(game.Team1, game.Team2);

            return true;
        }

        private static void InvalidTeam(TeamNo teamNo)
        {
            throw new ArgumentException($"Team number must be specified. TeamNo = {teamNo.ToString()}");
        }

        public static float CaptureSpeed(int players)
        {
            float result = 0f;
            float sign = Math.Sign(players);
            for (int i = 1; i < sign * players + 1; i++)
            {
                result += sign / i;
            }

            return result;
        }

        private void CaptureZones(IServerGame game)
        {
            foreach (var zoneEntity in game.Zones.Values)
            {
                int currentFirstTeamCount = 0;
                int currentSecondTeamCount = 0;

                foreach (var player in game.Players.Values)
                {
                    if (zoneEntity.IsInside(player))
                    {
                        switch (player.Team)
                        {
                            case TeamNo.First:
                                currentFirstTeamCount++;
                                break;
                            case TeamNo.Second:
                                currentSecondTeamCount++;
                                break;
                            default:
                                InvalidTeam(player.Team);
                                break;
                        }
                    }
                }

                // if both teams inside
                if (currentFirstTeamCount * currentSecondTeamCount != 0)
                {
                    continue;
                }

                ref var zone = ref zoneEntity.State;
                float captureSpeed = zone.Captured ? 1 : -1;

                // if one team inside
                if (currentFirstTeamCount + currentSecondTeamCount != 0)
                {
                    (int allies, int enemies) sides = (0, 0);
                    switch (zone.OwnerTeam)
                    {
                        case TeamNo.First:
                            sides = (currentFirstTeamCount, currentSecondTeamCount);
                            break;
                        case TeamNo.Second:
                            sides = (currentSecondTeamCount, currentFirstTeamCount);
                            break;
                        case TeamNo.None:
                            if (currentFirstTeamCount > 0)
                            {
                                zone.OwnerTeam = TeamNo.First;
                                sides = (currentFirstTeamCount, currentSecondTeamCount);
                            }
                            else
                            {
                                zone.OwnerTeam = TeamNo.Second;
                                sides = (currentSecondTeamCount, currentFirstTeamCount);
                            }

                            break;
                        default:
                            InvalidTeam(zone.OwnerTeam);
                            break;
                    }

                    captureSpeed = CaptureSpeed(sides.allies - sides.enemies);
                }

                zone.Progress += captureSpeed;

                if (zone.Progress >= timeplayersToCapture)
                {
                    if (!zone.Captured)
                    {
                        zone.Captured = true;
                        game.GetTeam(zone.OwnerTeam).CaptureZone(zone.Id);
                    }

                    zone.Progress = timeplayersToCapture;
                }
                else if (zone.Progress <= 0)
                {
                    if (zone.Captured)
                    {
                        zone.Captured = false;
                        game.GetTeam(zone.OwnerTeam).ReleaseZone(zone.Id);
                    }

                    zone.Progress = 0;
                    zone.OwnerTeam = TeamNo.None;
                }
            }
        }

        private void ScorePoints(Team team1, Team team2)
        {
            var diff = team1.ZonesCaptured - team2.ZonesCaptured;

            int score1 = team1.ZonesCaptured * Math.Max(diff, 1);
            team1.State.Score += score1;

            int score2 = team2.ZonesCaptured * Math.Max(-diff, 1);
            team2.State.Score += score2;
        }
    }
}