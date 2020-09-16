namespace Heyworks.PocketShooter.Audio
{
    public class LocalCharacterAudioController : CharacterAudioController
    {
        public override void HandleStun(bool isStunned)
        {
            AudioController.Instance.SetState(
                AudioKeys.StateGroup.EffectStun,
                isStunned ? AudioKeys.State.EffectStun : AudioKeys.State.None);

            if (isStunned)
            {
                PostEvent(AudioKeys.Event.PlayStunEffect);
            }
        }

        public override void HandleLifestealPlus()
        {
            PostEvent(AudioKeys.Event.PlayLifestealPlus);
        }

        public override void HandleLifestealMinus()
        {
            PostEvent(AudioKeys.Event.PlayLifestealMinus);
        }

        public override void HandleHeal()
        {
            PostEvent(AudioKeys.Event.PlayHealing);
        }

        public override void HandleRegeneration()
        {
            PostEvent(AudioKeys.Event.PlayRegeneration);
        }

        public override void HandleInstantReload()
        {
            PostEvent(AudioKeys.Event.PlayInstantReload);
        }

        public override void HandleRunSprint()
        {
            PostEvent(AudioKeys.Event.PlaySprint);
        }

        public override void HandleStopSprint()
        {
            PostEvent(AudioKeys.Event.StopSprint);
        }
    }
}
