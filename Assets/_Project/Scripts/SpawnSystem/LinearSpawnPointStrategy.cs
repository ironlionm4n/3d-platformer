using UnityEngine;

namespace Platformer._Project.Scripts.SpawnSystem
{
    public class LinearSpawnPointStrategy : ISpawnPointStrategy
    {
        private int index = 0;
        private Transform[] spawnPoints;

        public LinearSpawnPointStrategy(Transform[] spawnPoints)
        {
            this.spawnPoints = spawnPoints;
        }
        
        public Transform NextSpawnPoint()
        {
            var spawnPoint = spawnPoints[index];
            index = (index + 1) % spawnPoints.Length;
            return spawnPoint;
        }
    }
}