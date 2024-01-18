using UnityEngine;

namespace Platformer._Project.Scripts.SpawnSystem
{
    [CreateAssetMenu(menuName = "Create CollectibleData", fileName = "Platformer/CollectibleData", order = 0)]
    public class CollectibleData : EntityData
    {
        public int score;
        // additional properties specific to collectibles
    }
}