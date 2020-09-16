using System;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Contains extension methods for <see cref="FpsTransformComponent"/> struct.
    /// </summary>
    public static class FpsTransformComponentExtensions
    {
        // TODO: create PositionExtensions
        
        /// <summary>
        /// Checks whether transform located in the inside the cylinder zone.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oY">Y coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        /// /// <param name="height">Height radius of the zone.</param>
        /// <returns>True if transform located in the inside the zone.</returns>
        public static bool IsInsideCylinder(in this FpsTransformComponent self, float oX, float oY, float oZ, float radiusSqr, float height)
        {
            double dx = oX - self.Position.X;
            double dz = oZ - self.Position.Z;
            double dy = Math.Abs(oY - self.Position.Y);
            return (dx * dx + dz * dz) < radiusSqr && dy < height;
        }

        /// <summary>
        /// Checks whether transform located in the inside the zone.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        /// <returns>True if transform located in the inside the zone.</returns>
        public static bool IsInside(in this FpsTransformComponent self, float oX, float oZ, float radiusSqr)
        {
            double dx = oX - self.Position.X;
            double dz = oZ - self.Position.Z;
            return (dx * dx + dz * dz) < radiusSqr;
        }

        /// <summary>
        /// Checks whether transform located in the inside the SPHERE.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oY">Y coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        /// <returns>True if transform located in the inside the zone.</returns>
        public static bool IsInsideSphere(in this FpsTransformComponent self, float oX, float oY, float oZ, float radiusSqr)
        {
            double dx = oX - self.Position.X;
            double dy = oY - self.Position.Y;
            double dz = oZ - self.Position.Z;
            return (dx * dx + dy * dy + dz * dz) < radiusSqr;
        }
    }
}