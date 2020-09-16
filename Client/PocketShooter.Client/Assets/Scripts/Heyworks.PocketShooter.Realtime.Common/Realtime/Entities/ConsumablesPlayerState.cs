namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class ConsumablesPlayerState
    {
        public int TotalOffensives { get; private set; }

        public int UsedOffensives { get; private set; }

        public int TotalSupports { get; private set; }

        public int UsedSupports { get; private set; }

        public ConsumablesPlayerState(int totalOffensives, int totalSupports)
        {
            TotalOffensives = totalOffensives;
            TotalSupports = totalSupports;
        }

        public void UseOffensive()
        {
            TotalOffensives--;
            UsedOffensives++;
        }

        public void UseSupport()
        {
            TotalSupports--;
            UsedSupports++;
        }
    }
}