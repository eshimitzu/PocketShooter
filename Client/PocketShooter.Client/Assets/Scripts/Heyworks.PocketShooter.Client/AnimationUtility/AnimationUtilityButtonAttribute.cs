using System;

namespace Heyworks.PocketShooter.AnimationUtility
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AnimationUtilityButtonAttribute : Attribute
    {
        public bool IsTrigger { get; private set; }

        public AnimationUtilityButtonAttribute(bool isTrigger)
        {
            IsTrigger = isTrigger;
        }
    }
}