namespace Heyworks.PocketShooter
{
    /// <summary>
    /// Weapons can be one of this combinations {melee | range} X {immediate | delayed} X { local | splash} x {autonomous | controlled}.
    /// Some of combinations are non sense.
    /// </summary>
    public enum WeaponIdentifer : byte
    {
        // melee, immediate
        A = 1,
        B = 2,

        // range, immediate
        C = 30 + 1,
        D = 30 + 2,
        E = 30 + 3,
        F = 30 + 4,
        G = 30 + 5,
        H = 30 + 6,

        // range, splash, delayed
        I = 60 + 1,
    }
}