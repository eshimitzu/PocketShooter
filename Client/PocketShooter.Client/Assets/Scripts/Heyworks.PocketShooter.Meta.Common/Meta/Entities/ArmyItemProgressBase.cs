using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Runnables;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ArmyItemProgressBase : Runnable
    {
        private readonly ArmyBase armyBase;
        private readonly ArmyItemProgressState state;
        private readonly IArmyConfigurationBase armyConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Runnable"/> class.
        /// </summary>
        /// <param name="armyBase">The army entity.</param>
        /// <param name="state">Item progress state.</param>
        /// <param name="timeProvider">Time provider.</param>
        public ArmyItemProgressBase(
            ArmyBase armyBase,
            ArmyItemProgressState state,
            IDateTimeProvider timeProvider,
            IArmyConfigurationBase armyConfiguration)
            : base(new SimpleTimer(state.Timer, timeProvider))
        {
            this.armyBase = armyBase;
            this.state = state;
            this.armyConfiguration = armyConfiguration;
        }

        public int ItemId => state.ItemId;

        public Price CompletePrice => armyConfiguration.GetItemProgressCompletePrice(RemainingTime);

        public virtual void Complete() => armyBase.CompleteItemProgress();

        protected override void Finish() => armyBase.CompleteItemProgress();
    }
}
