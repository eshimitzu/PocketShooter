using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.HUD.Buttons;
using UniRx;

namespace Heyworks.PocketShooter.UI.HUD
{
    internal class SkillButtonPresenter : IDisposablePresenter
    {
        protected HUDButtonCooldown Button { get; }

        protected SkillController SkillController { get; }

        protected List<IDisposable> Subscriptions { get; }

        protected LocalCharacter Character { get; }

        private readonly Game game;

        public SkillButtonPresenter(
            HUDButtonCooldown button,
            SkillController skillController,
            LocalCharacter character,
            Game game)
        {
            this.Button = button;
            this.SkillController = skillController;
            this.Character = character;
            this.game = game;

            Subscriptions = new List<IDisposable>();

            button.ClickCounterModule.gameObject.SetActive(false);

            button.ButtonModule.SkillIcon.sprite = skillController.Spec.Icon;
//            button.ButtonModule.SkillIcon.SetNativeSize();
            character.Model.Events.SkillStateChanged.Where(f => f.SkillName == skillController.SkillName)
                .Subscribe(SkillStateChanged)
                .AddTo(Subscriptions);
            character.Model.Events.StunChanged.Subscribe(OnStunChanged).AddTo(Subscriptions);

            // TODO: a.dezhurko move to the derived class
            if (skillController.SkillName == SkillName.Rage)
            {
                character.Model.Events.RageChanged.Subscribe(OnRageChanged).AddTo(Subscriptions);
            }

            button.ButtonModule.ButtonClick += SkillButton_OnClick;
            button.ButtonModule.ButtonDown += SkillButton_OnDown;
            button.ButtonModule.ButtonUp += SkillButton_OnUp;

            game.StateUpdated.Subscribe(OnStateUpdated).AddTo(Subscriptions);
        }

        public void Dispose()
        {
            foreach (var subscription in Subscriptions)
            {
                subscription.Dispose();
            }

            Subscriptions.Clear();
        }

        protected virtual void SkillButton_OnClick()
        {
            SkillController.SkillControlOnClick();
        }

        protected virtual void SkillStateChanged(SkillStateChangedEvent sse)
        {
            Button.ButtonModule.SetIsInterectable(CanUseSkill(), SkillController is PassiveSkillController);

            if (sse.Activated)
            {
                // TODO: a.dezhurko replace with data from components
                var duration = SkillController.Model.Info.CooldownTime;
                Button.ProgressModule.StartProgress(game.Ticker.Current, duration);
                Button.CooldownModule.StartCountDownTimer(game.Ticker.Current, duration);
            }
        }

        protected virtual bool CanUseSkill()
        {
            return Character.Model.CanUseSkill(SkillController.Model.Name);
        }

        private void SkillButton_OnDown()
        {
            SkillController.SkillControlOnDown();
        }

        private void SkillButton_OnUp()
        {
            SkillController.SkillControlOnUp();
        }

        private void OnStunChanged(bool isStunned)
        {
            Button.ButtonModule.SetIsInterectable(CanUseSkill(), SkillController is PassiveSkillController);
        }

        private void OnRageChanged(int progress)
        {
            if (progress > 0)
            {
                Button.ButtonModule.MakeSemiTransparent(true);
                Button.LabelModule.SetLabel(progress.ToString());
            }
            else
            {
                Button.ButtonModule.MakeSemiTransparent(false);
                Button.LabelModule.SetLabel(string.Empty);
            }
        }

        private void OnStateUpdated(int currentTime)
        {
            Button.CooldownModule.UpdateCountDownTimer(currentTime);
            Button.ProgressModule.UpdateProgress(currentTime);
        }


    }
}