namespace Heyworks.PocketShooter.Realtime.Data
{
    public class DiveSkillInfo : AoESkillInfo
    {
        public DiveSkillInfo()
        {
        }

        public DiveSkillInfo(SkillInfo copyFrom, int castingTime, AoEInfo aoeInfo, float damage)
            : base(copyFrom, castingTime, aoeInfo)
        {
            Damage = damage;
        }

        public float Damage { get; set; }
    }
}