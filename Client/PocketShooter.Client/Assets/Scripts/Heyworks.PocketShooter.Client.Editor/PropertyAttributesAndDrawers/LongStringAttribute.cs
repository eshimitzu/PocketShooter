using UnityEngine;

namespace Heyworks.PocketShooter.PropertyAttributesAndDrawers
{
    public class LongStringAttribute : PropertyAttribute
    {
        public readonly float height;

        public LongStringAttribute()
        {
            height = 100f;
        }

        public LongStringAttribute(float height)
        {
            this.height = height;
        }
    }
}
