using Heyworks.PocketShooter.Networking.Actors;
using KinematicCharacterController;
using SRF;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotJump
    {
        private readonly BotCharacter character;
        private readonly KinematicCharacterMotor kinematicCharacter;
        private readonly PocketCharacterController pocketCharacterController;
        private readonly Rigidbody rigidbody;

        private readonly float angle;
        private readonly float speed;

        public bool InAir { get; private set; }

        public BotJump(BotCharacter character, float angle, float speed)
        {
            this.character = character;
            this.angle = angle;
            this.speed = speed;

            rigidbody = character.GetComponent<Rigidbody>();
            character.gameObject.SetLayerRecursive(LayerMask.NameToLayer("Player"));

            character.OnCollisionEnterAsObservable().Subscribe(OnCollisionEnter).AddTo(character);
            character.FixedUpdateAsObservable().Subscribe(FixedUpdate).AddTo(character);
        }

        public void Execute()
        {
            InAir = true;

            character.NavMesh.enabled = false;
            rigidbody.isKinematic = false;

            velocity = Vector3.up * 5 + Vector3.forward * 0.3f;

            UnityEngine.Debug.Log($"velocity {velocity}");
//            rigidbody.useGravity = true;
//            rigidbody.AddForce(Vector3.up * speed + Vector3.forward * 3, ForceMode.Impulse);
        }

        private Vector3 velocity;

        private void FixedUpdate(Unit unit)
        {
            if (InAir)
            {
                velocity -= 9.8f * Time.fixedDeltaTime * Vector3.up;

                Vector3 position = character.transform.position;
                position += velocity * Time.fixedDeltaTime;
//                character.transform.position = position;
                rigidbody.MovePosition(position);
                UnityEngine.Debug.Log($"position {position} | velocity {velocity}");
            }

//            
//            velocity
        }

        private void OnCollisionEnter(Collision c)
        {
            UnityEngine.Debug.Log($"OnCollisionEnter");
            if (rigidbody.velocity.y < 0)
            {
                InAir = false;

                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
                character.NavMesh.enabled = true;
            }
        }
    }
}