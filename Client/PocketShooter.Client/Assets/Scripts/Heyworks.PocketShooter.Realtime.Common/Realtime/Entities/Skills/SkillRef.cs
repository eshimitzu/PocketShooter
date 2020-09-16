using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

#pragma warning disable SA1649
namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// SkillRefBase.
    /// </summary>
    public abstract class SkillRefBase
    {
        /// <summary>
        /// playerStateRef.
        /// </summary>
        protected IRef<PlayerState> playerStateRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRefBase"/> class.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        protected SkillRefBase(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
        }

        /// <summary>
        /// ApplyState.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        public void ApplyState(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
        }
    }

    /// <summary>
    /// Skill reference.
    /// </summary>
    public class SkillRef1 : SkillRefBase, ISkillRef
    {
        /// <summary>
        /// Gets Id.
        /// </summary>
        public (EntityId PlayerId, SkillName SkillName) Id => (playerStateRef.Value.Id, Value.Base.Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRef1"/> class.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        public SkillRef1(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        /// <summary> Gets. </summary>
        public ref SkillComponents Value => ref playerStateRef.Value.Skill1;
    }

    /// <summary>
    /// Skill reference.
    /// </summary>
    public class SkillRef2 : SkillRefBase, ISkillRef
    {
        /// <summary>
        /// Gets Id.
        /// </summary>
        public (EntityId PlayerId, SkillName SkillName) Id => (playerStateRef.Value.Id, Value.Base.Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRef2"/> class.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        public SkillRef2(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        /// <summary> Gets. </summary>
        public ref SkillComponents Value => ref playerStateRef.Value.Skill2;
    }

    /// <summary>
    /// Skill reference.
    /// </summary>
    public class SkillRef3 : SkillRefBase, ISkillRef
    {
        /// <summary>
        /// Gets Id.
        /// </summary>
        public (EntityId PlayerId, SkillName SkillName) Id => (playerStateRef.Value.Id, Value.Base.Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRef3"/> class.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        public SkillRef3(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        /// <summary> Gets. </summary>
        public ref SkillComponents Value => ref playerStateRef.Value.Skill3;
    }

    /// <summary>
    /// Skill reference.
    /// </summary>
    public class SkillRef4 : SkillRefBase, ISkillRef
    {
        /// <summary>
        /// Gets Id.
        /// </summary>
        public (EntityId PlayerId, SkillName SkillName) Id => (playerStateRef.Value.Id, Value.Base.Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRef4"/> class.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        public SkillRef4(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        /// <summary> Gets. </summary>
        public ref SkillComponents Value => ref playerStateRef.Value.Skill4;
    }

    /// <summary>
    /// Skill reference.
    /// </summary>
    public class SkillRef5 : SkillRefBase, ISkillRef
    {
        /// <summary>
        /// Gets Id.
        /// </summary>
        public (EntityId PlayerId, SkillName SkillName) Id => (playerStateRef.Value.Id, Value.Base.Name);

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRef5"/> class.
        /// </summary>
        /// <param name="playerStateRef">Player.</param>
        public SkillRef5(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        /// <summary> Gets. </summary>
        public ref SkillComponents Value => ref playerStateRef.Value.Skill5;
    }
}