namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Consumable skill.
    /// </summary>
    public interface IConsumableSkill : ISkill
    {
        int UseCount { get; }

        int AvailableCount { get; set; }
    }
}