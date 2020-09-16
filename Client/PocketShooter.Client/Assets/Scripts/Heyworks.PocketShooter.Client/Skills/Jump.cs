using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    public class Jump
    {
        private readonly LocalCharacter character;
        private readonly float angel;
        private readonly float speed;

        public Jump(LocalCharacter character, float angel, float speed)
        {
            Assert.IsNotNull(character);

            this.character = character;
            this.angel = angel;
            this.speed = speed;

            character.CharacterController.OnLand += CharacterControllerOnOnLand;
        }

        public void Execute()
        {
            character.CharacterController.Jump(angel, speed);
            character.CharacterCommon.AnimationController.Jump();
        }

        private void CharacterControllerOnOnLand()
        {
            character.CharacterCommon.AnimationController.Land();
        }
    }
}
