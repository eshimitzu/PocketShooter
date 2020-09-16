namespace Heyworks.PocketShooter.Realtime.Data
{
    public class TrooperInfo
    {
        private TrooperInfo()
        {
        }

        public TrooperInfo(TrooperMetaInfo metaInfo)
        {
            MetaInfo = metaInfo;
        }

        public TrooperClass Class => MetaInfo.Class;

        public TrooperMetaInfo MetaInfo { get; private set; }

        public int MaxHealth { get; set; }

        public int MaxArmor { get; set; }

        public float Speed { get; set; }

        public WeaponInfo Weapon { get; set; }

        public SkillInfo Skill1 { get; set; }

        public SkillInfo Skill2 { get; set; }

        public SkillInfo Skill3 { get; set; }

        public SkillInfo Skill4 { get; set; }

        public SkillInfo Skill5 { get; set; }
    }
}
