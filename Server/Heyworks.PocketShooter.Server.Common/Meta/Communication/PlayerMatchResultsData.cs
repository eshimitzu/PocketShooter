using System;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class PlayerMatchResultsData
    {
        public Guid PlayerId { get; set; }

        public string Nickname { get; set; }

        public TeamNo TeamNo { get; set; }

        public TrooperClass TrooperClass { get; set; }

        public WeaponName CurrentWeapon { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public bool IsWinner { get; set; }

        public bool IsDraw { get; set; }

        public bool IsBot { get; set; }
    }
}
