using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.UI.HUD.Buttons;
using UniRx;

namespace Heyworks.PocketShooter.UI.HUD
{
    internal class MedKitSkillButtonPresenter : ConsumableSkillButtonPresenter
    {
        private readonly MedKitSkillInfo medKitConfig;
        private const float HealthPercentageForAlertBlink = 0.25f;

        public MedKitSkillButtonPresenter(HUDButtonCooldown button, SkillController skillController, LocalCharacter character, Game game)
        : base(button, skillController, character, game)
        {
            medKitConfig = (MedKitSkillInfo)skillController.Model.Info;
            character.Model.Events.ExpendablesChanged.Subscribe(UpdateHealth).AddTo(Subscriptions);
            button.ButtonModule.SetIsInterectable(character.Model.CanUseFirstAidKit());
        }

        protected override void SkillButton_OnClick()
        {
            if (Character.Model.CanUseFirstAidKit())
            {
                base.SkillButton_OnClick();
            }
        }

        protected override void SkillStateChanged(SkillStateChangedEvent sse)
        {
            base.SkillStateChanged(sse);

            if (sse.Activated)
            {
                Button.BlinkModule.StopBlink();
            }
        }

        protected override bool CanUseSkill()
        {
            return Character.Model.CanUseFirstAidKit();
        }

        private void UpdateHealth(EntityId any)
        {
            var characterConfig = Character.Model.Info;

            Button.ButtonModule.SetIsInterectable(CanUseSkill());

            if (Character.Model.Health.Health < characterConfig.MaxHealth * HealthPercentageForAlertBlink &&
                !Button.BlinkModule.IsEffectActive &&
                Button.ButtonModule.IsInteractable)
            {
                Button.BlinkModule.StartBlink();
            }

            if (Character.Model.Health.Health > characterConfig.MaxHealth * HealthPercentageForAlertBlink &&
                 Button.BlinkModule.IsEffectActive)
            {
                Button.BlinkModule.StopBlink();
            }
        }
    }
}