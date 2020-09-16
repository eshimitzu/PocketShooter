namespace Heyworks.PocketShooter.Meta.Entities
{
    public interface IHasGradeAndLevel
    {
        Grade Grade { get; }

        int Level { get; }
    }
}
