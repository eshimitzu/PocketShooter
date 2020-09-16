using Collections.Pooled;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public abstract class WeaponFireVisualiser : MonoBehaviour
    {
        protected ICharacterContainer CharacterContainer { get; private set; }

        protected IRemotePlayer Model { get; private set; }

        protected IPlayerEvents PlayerEvents { get; private set; }

        protected CharacterView CharacterView { get; private set; }

        protected OrbitFollowTransform OrbitFollowTransform { get; private set; }

        protected WeaponRaycaster WeaponRaycaster { get; private set; }

        protected PooledList<ClientShotInfo> LastShotInfo { get; private set; }

        // TODO: v.filippov common for player model
        public virtual void Setup(
            ICharacterContainer characterContainer,
            WeaponRaycaster weaponRaycaster,
            CharacterView characterView,
            IPlayerEvents playerEvents,
            IRemotePlayer model = null,
            OrbitFollowTransform orbitFollowTransform = null)
        {
            CharacterContainer = characterContainer;
            Model = model;
            WeaponRaycaster = weaponRaycaster;
            CharacterView = characterView;
            OrbitFollowTransform = orbitFollowTransform;
            PlayerEvents = playerEvents;

            if (Model != null)
            {
                LastShotInfo = new PooledList<ClientShotInfo>();
                Model.Events.Attack.Subscribe(ProcessShoot).AddTo(this);
            }
        }

        public void VizualizeAttack(Vector3[] hitPoints) => 
            CharacterView.VizualizeAttack(hitPoints);

        protected virtual void ProcessShoot(AttackServerEvent ase)
        {
        }
    }
}