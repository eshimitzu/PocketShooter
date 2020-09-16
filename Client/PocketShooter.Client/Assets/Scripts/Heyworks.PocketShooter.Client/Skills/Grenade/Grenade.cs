using System.Collections;
using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    public class Grenade : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem explosion;

        private Renderer[] renderers;
        private bool exploded;
        private NetworkCharacter ownerCharacter;

        public Rigidbody Rigidbody { get; private set; }

        public Collider Collider { get; private set; }

        public TrailRenderer TrailRenderer { get; private set; }

        private void Awake()
        {
            renderers = GetComponents<Renderer>();

            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            TrailRenderer = GetComponent<TrailRenderer>();
        }

        private void OnEnable()
        {
            exploded = false;
            TrailRenderer.Clear();
            Rigidbody.ResetInertiaTensor();
        }

        public void Setup(NetworkCharacter character)
        {
            Assert.IsNotNull(character);

            this.ownerCharacter = character;
        }

        private void Detonate(Collider other)
        {
            if (ownerCharacter && ownerCharacter is LocalCharacter localCharacter)
            {
                // TODO: a.dezhurko init explosion radius in setup method. Grenade can be used not as skill.
                var config = localCharacter.Model.GetFirstSkillInfo<GrenadeSkillInfo>(SkillName.Grenade);

                var directVictims = new PooledList<EntityId>();
                var splashVictims = new PooledList<EntityId>();

                var hitCharacter = other.gameObject.GetComponentInParent<RemoteCharacter>();
                if (hitCharacter != null)
                {
                    directVictims.Add(hitCharacter.Id);
                }

                var colliders = Physics.OverlapSphere(transform.position, config.ExplosionRadius);

                HashSet<RemoteCharacter> nearCharacters = new HashSet<RemoteCharacter>();
                foreach (var col in colliders)
                {
                    var c = col.GetComponentInParent<RemoteCharacter>();
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

                var data = new GrenadeExplosionCommandData(localCharacter.Id, directVictims, splashVictims);
                localCharacter.AddCommand(data);
            }

            exploded = true;
            AudioController.Instance.PostEvent(AudioKeys.Event.PlayGrenadeExplosion, gameObject);
            StartCoroutine(DetonateCoroutine());
        }

        protected virtual IEnumerator DetonateCoroutine()
        {
            explosion.transform.parent = null;
            explosion.transform.rotation = Quaternion.identity;
            explosion.Play();
            foreach (var r in renderers)
            {
                r.enabled = false;
            }

            yield return new WaitForSeconds(explosion.main.duration);

            foreach (var render in renderers)
            {
                render.enabled = true;
            }

            explosion.transform.parent = transform;
            explosion.transform.localPosition = Vector3.zero;
            LeanPool.Despawn(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!exploded)
            {
                Detonate(other);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!exploded)
            {
                Detonate(collision.collider);
            }
        }
    }
}