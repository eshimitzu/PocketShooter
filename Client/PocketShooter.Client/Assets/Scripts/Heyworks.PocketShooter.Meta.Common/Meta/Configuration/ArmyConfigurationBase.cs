using System;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class ArmyConfigurationBase : IArmyConfigurationBase
    {
        #region [Private Members]

        private readonly GameConfig gameConfig;

        #endregion [Private Members]

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmyConfigurationBase"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public ArmyConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #endregion [Constructors and initialization]

        #region [IArmyConfigurationBase Implementation]

        public Price GetItemProgressCompletePrice(TimeSpan remainingTime)
        {
            if (remainingTime <= gameConfig.BasicPricesConfig.FreeTimeLimit)
            {
                return Price.CreateGold(0);
            }

            return Price.CreateGold(
                (int)Math.Round(Math.Ceiling(remainingTime.TotalMinutes) * gameConfig.BasicPricesConfig.OneMinuteInGold));
        }

        #endregion [IArmyConfigurationBase Implementation]
    }
}
