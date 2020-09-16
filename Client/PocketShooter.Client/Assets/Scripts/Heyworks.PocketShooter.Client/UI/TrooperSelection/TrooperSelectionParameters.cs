using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    /// <summary>
    /// Represents the trooper selection parameters.
    /// </summary>
    public class TrooperSelectionParameters
    {
        /// <summary>
        /// Gets the trooper class.
        /// </summary>
        public TrooperClass TrooperClass
        {
            get;
            private set;
        }

        public WeaponName WeaponId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trooper game object.
        /// </summary>
        public GameObject TrooperGameObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrooperSelectionParameters"/> class.
        /// </summary>
        /// <param name="trooperClass">The trooper class.</param>
        /// <param name="weaponIdentifer">The weapon identifier.</param>
        /// <param name="trooperGameObject">The trooper game object.</param>
        public TrooperSelectionParameters(TrooperClass trooperClass, WeaponName weaponIdentifer, GameObject trooperGameObject)
        {
            TrooperClass = trooperClass;
            WeaponId = weaponIdentifer;
            TrooperGameObject = trooperGameObject;
        }
    }
}