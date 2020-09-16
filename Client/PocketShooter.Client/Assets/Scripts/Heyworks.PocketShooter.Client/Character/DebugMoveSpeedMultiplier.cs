using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    public class DebugMoveSpeedMultiplier : IMoveSpeedMultiplier
    {
        public float SpeedMultiplier => Input.GetKey(KeyCode.LeftShift) ? 5 : 1;
    }
}