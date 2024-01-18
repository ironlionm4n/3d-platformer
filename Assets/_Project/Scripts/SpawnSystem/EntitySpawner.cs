namespace Platformer._Project.Scripts.SpawnSystem
{
    public class EntitySpawner<T> where T : Entity
    {
        IEntityFactory<T> entityFactory;
        ISpawnPointStrategy spawnPointStrategy;
        
        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy)
        {
            this.entityFactory = entityFactory;
            this.spawnPointStrategy = spawnPointStrategy;
        }
        
        public T Spawn()
        {
            var spawnPoint = spawnPointStrategy.NextSpawnPoint();
            return entityFactory.Create(spawnPoint);
        }
    }
}