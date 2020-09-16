using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ArmyItemProgress : ArmyItemProgressBase
    {
        private readonly Army army;
        private readonly IGameHubClient gameHubClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmyItemProgress"/> class.
        /// </summary>
        public ArmyItemProgress(
            Army army,
            ArmyItemProgressState state,
            IDateTimeProvider timeProvider,
            IArmyConfigurationBase armyConfiguration,
            IGameHubClient gameHubClient)
            : base(army, state, timeProvider, armyConfiguration)
        {
            this.army = army;
            this.gameHubClient = gameHubClient;
        }

        public override void Complete()
        {
            base.Complete();
            SendCompleteProgress();

            army.ItemProgressCompleted.OnNext(this);
        }

        protected override void Finish()
        {
            base.Finish();
            SendSyncProgress();

            army.ItemProgressCompleted.OnNext(this);
        }

        private async void SendSyncProgress() => await gameHubClient.SyncArmyItemProgress();

        private async void SendCompleteProgress() => await gameHubClient.CompleteArmyItemProgress();

        public IContentIdentity ToContentIdentity() => army.FindContent(ItemId);

        public bool IsFirstLevelOnGrade() => army.FindItem(ItemId).IsFirstLevelOnGrade;
    }
}
