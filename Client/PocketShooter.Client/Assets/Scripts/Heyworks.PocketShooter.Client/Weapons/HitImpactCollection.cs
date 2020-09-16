using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    [CreateAssetMenu(fileName = "HitImpactCollection", menuName = "HeyworksMain/Settings/Hit Impact Collection")]
    public class HitImpactCollection : ScriptableObject
    {
        // TODO: a.dezhurko Rewrite to array and custom editor
        [SerializeField]
        private HitImpact trooperImpact = null;
        [SerializeField]
        private HitImpact wallImpact = null;

        public HitImpact GetImpactPrefab(SurfaceType surface)
        {
            switch (surface)
            {
                case SurfaceType.Trooper:
                    return trooperImpact;
                case SurfaceType.Wall:
                    return wallImpact;
                default:
                    throw new ArgumentOutOfRangeException(nameof(surface), surface, null);
            }
        }
    }
}
