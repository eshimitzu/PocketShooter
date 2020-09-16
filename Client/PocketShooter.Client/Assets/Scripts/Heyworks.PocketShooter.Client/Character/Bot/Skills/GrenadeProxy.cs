using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Lean.Pool;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    /// <summary>
    /// Physics replaced with spherecast. Used only for bots to simulate behaviour, cause grenades visualized by RemotePlayer.
    /// Can be reduced after merging bots simulations.
    /// It may replace physics, cause nonconvex colliders + interpolation = inaccurate collision. Sometimes collision happens in half meter before.
    /// </summary>
    public class GrenadeProxy : MonoBehaviour
    {
        [SerializeField]
        private float radius;

        private BotCharacter ownerCharacter;
        private float explosionRadius;

        private LayerMask mask;
        private Vector3 velocity;
        private Vector3 prevPosition;

        private bool active = false;

        public void Setup(BotCharacter character, float explosionRadius, int layer)
        {
            this.ownerCharacter = character;
            this.radius = explosionRadius;
            gameObject.layer = layer;
            active = false;
        }

        public void Throw(Vector3 startVelocity)
        {
            mask = GetCollisionMask();
            prevPosition = transform.position;
            velocity = startVelocity;
            active = true;
        }

        private void Detonate(Collider other)
        {
            if (ownerCharacter)
            {
                var directVictims = new PooledList<EntityId>();
                var splashVictims = new PooledList<EntityId>();

                var hitCharacter = other.gameObject.GetComponentInParent<NetworkCharacter>();
                if (hitCharacter != null)
                {
                    directVictims.Add(hitCharacter.Id);
                }

                var colliders = Physics.OverlapSphere(transform.position, radius);

                HashSet<NetworkCharacter> nearCharacters = new HashSet<NetworkCharacter>();
                foreach (var col in colliders)
                {
                    var c = col.GetComponentInParent<NetworkCharacter>();
                    if (c != null && c != hitCharacter)
                    {
                        if (c.Model.Team != ownerCharacter.Model.Team)
                        {
                            nearCharacters.Add(c);
                        }
                    }
                }

                foreach (var c in nearCharacters)
                {
                    splashVictims.Add(c.Id);
                }

                var data = new GrenadeExplosionCommandData(ownerCharacter.Id, directVictims, splashVictims);
                ownerCharacter.AddCommand(data);
            }

            active = false;
            LeanPool.Despawn(this);
        }

        private void FixedUpdate()
        {
            if (!active)
            {
                return;
            }

            velocity += Physics.gravity * Time.fixedDeltaTime;
            Vector3 delta = velocity * Time.fixedDeltaTime;
            Vector3 nextPos = prevPosition + delta;
            prevPosition = nextPos;

            if (Physics.SphereCast(prevPosition, radius, velocity, out RaycastHit hit, delta.magnitude, mask))
            {
                transform.position = hit.point;
                Detonate(hit.collider);
            }
            else
            {
                transform.position = nextPos;
            }
        }

        private int GetCollisionMask()
        {
            int myLayer = gameObject.layer;
            int layerMask = 0;

            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(myLayer, i))
                {
                    layerMask = layerMask | 1 << i;
                }
            }

            return layerMask;
        }
    }
}