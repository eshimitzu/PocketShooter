using System.Collections.Generic;

namespace Heyworks.PocketShooter.Character
{
    public class MovementModifiers
    {
        private HashSet<IMoveSpeedMultiplier> speedMultipliers = new HashSet<IMoveSpeedMultiplier>();

        public float SpeedMultiplier
        {
            get
            {
                float speedMultiplier = 1;
                foreach (IMoveSpeedMultiplier provider in speedMultipliers)
                {
                    speedMultiplier = speedMultiplier + provider.SpeedMultiplier - 1;
                }

                return speedMultiplier;
            }
        }

        public void AddMoveSpeedMultiplier(IMoveSpeedMultiplier multiplier)
        {
            speedMultipliers.Add(multiplier);
        }

        public void RemoveMoveSpeedMultiplier(IMoveSpeedMultiplier multiplier)
        {
            speedMultipliers.Remove(multiplier);
        }
    }
}