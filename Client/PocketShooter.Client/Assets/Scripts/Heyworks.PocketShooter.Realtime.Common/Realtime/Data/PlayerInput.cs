using System;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct PlayerInput
    {
        public enum ButtonState : byte
        {
            None = 0,
            Down = 1,
            Up = 2,
            Click = 3,
        }

        public FpsTransformComponent Transform;

        public bool WarmupWeapon;

        public ButtonState ButtonReload;
        public ButtonState ButtonHeal;

        public ButtonState ButtonSkill1;
        public ButtonState ButtonSkill2;
        public ButtonState ButtonSkill3;
    }
}