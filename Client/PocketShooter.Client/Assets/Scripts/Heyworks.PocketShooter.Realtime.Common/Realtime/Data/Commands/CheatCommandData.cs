namespace Heyworks.PocketShooter.Realtime.Data
{
    public class CheatCommandData : IGameCommandData
    {
        public CheatCommandData(EntityId actorId, CheatType cheatType)
        {
            ActorId = actorId;
            CheatType = cheatType;
        }

        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.Cheat;

        public CheatType CheatType { get; }
    }

    public enum CheatType
    {
        StunSelf = 1,
        RootSelf = 2,
        KillSelf = 3,
        FullArmor = 4,
        NoArmor = 5,
        ResetSkills = 6,
        OneHp = 7,
        Damage500 = 8,
        EndGame = 9,
    }
}