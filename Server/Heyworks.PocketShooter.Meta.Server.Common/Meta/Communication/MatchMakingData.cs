namespace Heyworks.PocketShooter.Meta.Communication
{
    // data from user profile requited to do match making
    public class MatchMakingData
    {
        private MatchMakingData() { }

        public MatchMakingData(int learningMeter, int level)
        {
            this.LearningMeter = learningMeter;
            this.Level = level;

        }

        public int LearningMeter { get; }

        public int Level { get; }
    }
}