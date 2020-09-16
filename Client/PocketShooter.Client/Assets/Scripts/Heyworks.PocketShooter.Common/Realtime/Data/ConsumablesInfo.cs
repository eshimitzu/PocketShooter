namespace Heyworks.PocketShooter.Realtime.Data
{
    public class ConsumablesInfo
    {
        public int TotalOffensives { get; }

        public int TotalSupports { get; }

        public ConsumablesInfo(int offensives, int supports)
        {
            TotalOffensives = offensives;
            TotalSupports = supports;
        }
    }
}