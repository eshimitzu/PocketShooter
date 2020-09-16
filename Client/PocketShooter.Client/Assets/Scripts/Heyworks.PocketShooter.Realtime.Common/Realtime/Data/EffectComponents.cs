namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Effect components components.
    /// </summary>
    public struct EffectComponents
    {
        public StunComponents Stun;

        public RootComponents Root;

        public InvisibleComponents Invisible;

        public ImmortalityComponents Immortality;

        public RageComponents Rage;

        public JumpComponents Jump;

        public LuckyComponents Lucky;

        public DashComponents Dash;

        public MedKitComponents MedKit;

        public MedKitComponents Heal;
    }

    public static class EffectComponentsExtensions
    {
        public static bool IsImmortal(this in EffectComponents self) => self.Immortality.Base.IsImmortal;
    }    
}