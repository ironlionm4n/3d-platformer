﻿using System;
using UnityEngine;

namespace Platformer._Project.Scripts.SpawnSystem
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField] private CollectibleData[] collectibleData;
        [SerializeField] private float spawnInterval = 1f;

        private EntitySpawner<Collectible> spawner;

        private CountdownTimer spawnTimer;
        private int counter;
        
        protected override void Awake()
        {
            base.Awake();
            spawner = new EntitySpawner<Collectible>(new EntityFactory<Collectible>(collectibleData),
                spawnPointStrategy);
            
            spawnTimer = new CountdownTimer(spawnInterval);
            spawnTimer.OnTimerStop += () =>
            {
                if(counter >= spawnPoints.Length)
                {
                    spawnTimer.Stop();
                    return;
                }

                Spawn();
                counter++;
                spawnTimer.Start();
            };
        }

        private void Start() => spawnTimer.Start();

        private void Update() => spawnTimer.Tick(Time.deltaTime);

        public override void Spawn()
        {
            spawner.Spawn();
        }
    }
}