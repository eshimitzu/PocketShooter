using System;
using Heyworks.PocketShooter.Weapons;
using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    [Serializable]
    public class WeaponViewConfig
    {
        [SerializeField]
        private RuntimeAnimatorController characterAnimatorController;
        [SerializeField]
        private WeaponName name;
        [SerializeField]
        private WeaponController weaponController;
        [SerializeField]
        private WeaponRaycaster weaponRaycaster;
        [SerializeField]
        private WeaponFireVisualiser weaponFireVisualiser;
        [SerializeField]
        private WeaponView weaponView;
        [SerializeField]
        private Sprite weaponCrosshairIcon;
        [SerializeField]
        private float crosshairRelativeSize;
        [SerializeField]
        private bool sniperScope;

        public WeaponName Name => name;

        public WeaponView View => weaponView;

        public WeaponController WeaponController => weaponController;

        public WeaponRaycaster WeaponRaycaster => weaponRaycaster;

        public WeaponFireVisualiser WeaponFireVisualiser => weaponFireVisualiser;

        public Sprite WeaponCrosshairIcon => weaponCrosshairIcon;

        public float CrosshairRelativeSize => crosshairRelativeSize;

        public bool SniperScope => sniperScope;

        public RuntimeAnimatorController CharacterAnimatorController => characterAnimatorController;
    }
}