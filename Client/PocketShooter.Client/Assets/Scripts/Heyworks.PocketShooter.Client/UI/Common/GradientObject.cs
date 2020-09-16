using System;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Common
{
    [Serializable]
    [CreateAssetMenu(fileName = "Gradient", menuName = "Heyworks/UI/Gradient")]
    public class GradientObject : ScriptableObject
    {
        public Gradient value;
    }
}
