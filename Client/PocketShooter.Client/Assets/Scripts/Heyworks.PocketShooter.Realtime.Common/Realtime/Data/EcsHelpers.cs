using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Heyworks.PocketShooter.Realtime.Data
{
    // some domain specific shortcuts or netcode ecs
    public static class EcsHelpers
    {
        public static bool NearEquals(float double1, float double2, float precision) => (Math.Abs(double1 - double2) <= precision);

        public static bool ThrowEquals()
        {               
          Throw.InvalidOperation("Do not cast to object. Slooow.");
            return false;       
        }
    }
}