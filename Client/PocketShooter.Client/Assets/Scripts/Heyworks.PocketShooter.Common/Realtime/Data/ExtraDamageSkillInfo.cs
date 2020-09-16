namespace Heyworks.PocketShooter.Realtime.Data
{
    public class ExtraDamageSkillInfo : SkillInfo
    {
        public ExtraDamageSkillInfo()
        {
        }

        public ExtraDamageSkillInfo(SkillInfo copyFrom, float damagePercentOfMaxHealth)
            : base(copyFrom)
        {
            DamagePercentOfMaxHealth = damagePercentOfMaxHealth;
        }

        public float DamagePercentOfMaxHealth { get; set; }
    }
}