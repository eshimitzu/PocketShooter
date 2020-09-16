using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Realtime.Entities;
using UniRx;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponFireVisualizerWarmingUpFirearm : WeaponFireVisualizerFirearm
    {
        private IWarmingUpWeaponView WarmingUpWeaponView => (IWarmingUpWeaponView)CharacterView.WeaponView;

        public override void Setup(ICharacterContainer characterContainer, WeaponRaycaster weaponRaycaster, CharacterView characterView, IPlayerEvents playerEvents, IRemotePlayer model = null, OrbitFollowTransform orbitFollowTransform = null)
        {
            base.Setup(characterContainer, weaponRaycaster, characterView, playerEvents, model, orbitFollowTransform);
            PlayerEvents.WarmingUpProgressChanged.Subscribe(WarmingUpWeaponView.UpdateWarmupProgress).AddTo(this);
        }
    }
}