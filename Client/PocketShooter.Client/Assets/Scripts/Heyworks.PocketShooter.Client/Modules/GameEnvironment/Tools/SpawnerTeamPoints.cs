using Heyworks.PocketShooter.Realtime.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.Modules.GameEnvironment
{
    /// <summary>
    /// Contains spawner team points.
    /// </summary>
    public class SpawnerTeamPoints : MonoBehaviour
    {
        [SerializeField]
        private Transform[] spawnPoints;

        [SerializeField]
        private TeamNo team = 0;

        /// <summary>
        /// Gets get SpawnPoints.
        /// </summary>
        public Transform[] SpawnPoints => spawnPoints;

        /// <summary>
        /// Gets team.
        /// </summary>
        public TeamNo Team => team;
    }
}