using UnityEngine;

namespace Platformer._Project.Scripts.SpawnSystem
{
    public interface IEntityFactory<T> where T : Entity
    {
        T Create(Transform spawnPoint);
    }
}