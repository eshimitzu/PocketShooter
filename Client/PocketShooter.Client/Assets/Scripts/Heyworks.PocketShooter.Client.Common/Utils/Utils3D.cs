using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public class Utils3D
    {
        /// <summary>
        /// Returns Vector3 as (x, 0, z)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 HorizontalVector(Vector3 value)
        {
            value.y = 0.0f;
            return value;
        }

        /// <summary>
        /// returns the angle between a look vector and a target position.
        /// can be used for various aiming logic
        /// </summary>
        public static float LookAtAngle(Vector3 fromPosition, Vector3 fromForward, Vector3 toPosition)
        {

            return (Vector3.Cross(fromForward, (toPosition - fromPosition).normalized).y < 0.0f) ?
                -Vector3.Angle(fromForward, (toPosition - fromPosition).normalized) :
                Vector3.Angle(fromForward, (toPosition - fromPosition).normalized);

        }

        /// <summary>
        /// returns the angle between a look vector and a target position as
        /// seen top-down in the cardinal directions. useful for gui pointers
        /// </summary>
        public static float LookAtAngleHorizontal(Vector3 fromPosition, Vector3 fromForward, Vector3 toPosition)
        {

            return LookAtAngle(
                HorizontalVector(fromPosition),
                HorizontalVector(fromForward),
                HorizontalVector(toPosition));

        }

    }
}