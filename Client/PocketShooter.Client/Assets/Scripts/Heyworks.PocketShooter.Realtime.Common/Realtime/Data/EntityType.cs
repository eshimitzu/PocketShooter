namespace Heyworks.PocketShooter.Realtime.Data
{
    public enum EntityType : byte
    {
        Player = 1,

        Team = 2,

        Zone = 3,

        Weapon = 4,

        Skill = 5,

        // TODO: v.shimkovich temp solution. maybe use damage/heal type.
        LifestealSkill = 6,
        RegenerationOnKillSkill = 7,
    }
}