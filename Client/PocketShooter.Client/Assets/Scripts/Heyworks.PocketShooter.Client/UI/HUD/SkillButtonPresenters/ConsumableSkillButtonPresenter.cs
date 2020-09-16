using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.UI.HUD.Buttons;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD
{
    internal class ConsumableSkillButtonPresenter : SkillButtonPresenter
    {
        private ConsumableSkill consumableSkill;

        public ConsumableSkillButtonPresenter(HUDButtonCooldown button, SkillController skillController, LocalCharacter character, Game game) 
        : base(button, skillController, character, game)
        {
            consumableSkill = (ConsumableSkill)skillController.Model.Skill;

            button.ClickCounterModule.gameObject.SetActive(true);
            button.ClickCounterModule.SetRemainingClicks(consumableSkill.AvailableCount - consumableSkill.UseCount, consumableSkill.AvailableCount);
            button.ButtonModule.SetIsInterectable(character.Model.CanUseSkill(skillController.SkillName));
        }

        protected override void SkillButton_OnClick()
        {
            if (Character.Model.CanUseSkill(SkillController.SkillName))
            {
                base.SkillButton_OnClick();
            }
        }

        protected override void SkillStateChanged(SkillStateChangedEvent sse)
        {
            base.SkillStateChanged(sse);

            if (sse.Activated)
            {
                Button.ClickCounterModule.SetRemainingClicks(consumableSkill.AvailableCount - consumableSkill.UseCount, consumableSkill.AvailableCount);
            }
        }
    }
}