using UnityEngine;

namespace Platformer._Project.Scripts.SpawnSystem
{
    public interface ISpawnPointStrategy
    {
        Transform NextSpawnPoint();
    }
}