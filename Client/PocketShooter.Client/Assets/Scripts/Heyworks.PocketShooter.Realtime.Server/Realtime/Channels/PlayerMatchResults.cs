using System;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    public class PlayerMatchResults
    {
        private readonly TeamNo winnerTeamNo;

        public PlayerMatchResults(
            Guid playerId,
            string nickname,
            TeamNo teamNo,
            TeamNo winnerTeamNo,
            TrooperClass trooperClass,
            PlayerMatchStats matchStats,
            bool isBot,
            WeaponName currentWeapon)
        {
            this.winnerTeamNo = winnerTeamNo;

            PlayerId = playerId;
            Nickname = nickname;
            TeamNo = teamNo;
            TrooperClass = trooperClass;
            MatchStats = matchStats;
            IsBot = isBot;
            CurrentWeapon = currentWeapon;
        }

        public Guid PlayerId { get; }

        public string Nickname { get; }

        public TeamNo TeamNo { get; set; }

        public TrooperClass TrooperClass { get; }

        public WeaponName CurrentWeapon { get; }

        public PlayerMatchStats MatchStats { get; }

        public bool IsWinner => TeamNo == winnerTeamNo;

        public bool IsDraw => winnerTeamNo == TeamNo.None;

        public bool IsBot { get; }
    }
}
