using System;

namespace Heyworks.PocketShooter
{
    /// <summary>
    /// Weapons can be one of this combinations {melee | range} X {immediate | delayed} X { local | splash} x {autonomous | controlled}.
    /// Some of combinations are non sense.
    /// </summary>
    public enum WeaponName : byte
    {
        Knife = 1,
        Katana = 2,

        M16A4 = 30 + 1,
        SVD = 30 + 2,
        SawedOff = 30 + 3,
        Minigun = 30 + 4,
        Remington = 30 + 5,
        Barret = 30 + 6,
        Bazooka = 60 + 1,
    }
}