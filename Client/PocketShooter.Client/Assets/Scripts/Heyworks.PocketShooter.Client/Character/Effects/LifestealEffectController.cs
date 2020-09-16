namespace Heyworks.PocketShooter.Client
{
    public class LifestealEffectController : EffectController
    {
        public void PlayLifesteal()
        {
            Character.CharacterCommon.AudioController.HandleLifestealPlus();
        }

        public void PlayLifestealed()
        {
            Character.CharacterCommon.AudioController.HandleLifestealMinus();
        }

        public override void Stop()
        {
        }
    }
}