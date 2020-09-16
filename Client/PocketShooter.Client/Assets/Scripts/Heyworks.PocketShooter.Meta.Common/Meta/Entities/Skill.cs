namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Skill
    {
        public Skill(SkillName name, Grade grade)
        {
            Name = name;
            Grade = grade;
        }

        public SkillName Name { get; }

        public Grade Grade { get; }
    }
}
