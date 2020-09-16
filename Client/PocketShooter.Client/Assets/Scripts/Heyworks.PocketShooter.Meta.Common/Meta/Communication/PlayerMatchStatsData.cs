namespace Heyworks.PocketShooter.Meta.Communication
{
    public class PlayerMatchStatsData
    {
        public string Nickname { get; set; }

        public TrooperClass TrooperClass { get; set; }

        public WeaponName CurrentWeapon { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public bool IsMVP { get; set; }
    }
}