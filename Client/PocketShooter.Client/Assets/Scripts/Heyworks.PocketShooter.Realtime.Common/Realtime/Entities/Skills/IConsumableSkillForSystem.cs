namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// IConsumableSkillForSystem.
    /// </summary>
    public interface IConsumableSkillForSystem : ISkillForSystem
    {
        int UseCount { get; set; }

        int AvailableCount { get; set; }
    }
}