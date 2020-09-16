using Heyworks.PocketShooter.Weapons;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class AnimationUtilityZenjectInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject trooperCreatorPrefab = null;

        public override void InstallBindings()
        {
            Container.Bind<OfflineTrooperCreator>()
                .FromComponentInNewPrefab(trooperCreatorPrefab)
                .UnderTransformGroup("Managers")
                .AsSingle()
                .NonLazy();

            Container.BindFactory<Object, WeaponView, WeaponView.Factory>()
                .FromFactory<PrefabFactory<WeaponView>>();
            Container.BindFactory<Object, OfflineLocalCharacter, OfflineLocalCharacter.Factory>()
                .FromFactory<PrefabFactory<OfflineLocalCharacter>>();
        }
    }
}