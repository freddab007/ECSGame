using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ActivationPlates
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject BallPrefab;
        public Transform spawnBall;
        public Transform target;
        public float BallSpawnInterval = 4f;
        public int NumBallsSpawn = 10;
        public float ImpactStrength = 5f;
        public Bounds SpawnBounds;
        public float BallSpawnHeight; // how high the ball spawning bounds should be above the brick spawning bounds 

        public class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                authoring.SpawnBounds.center = authoring.spawnBall.position;
                AddComponent(entity, new Config
                {
                    BallPrefab = GetEntity(authoring.BallPrefab, TransformUsageFlags.Dynamic),
                    enemyTarget = authoring.target.position,
                    BallSpawnInterval = authoring.BallSpawnInterval,
                    NumBallsSpawn = authoring.NumBallsSpawn,
                    ImpactStrength = authoring.ImpactStrength,
                    SpawnBoundsMax = authoring.SpawnBounds.max,
                    SpawnBoundsMin = authoring.SpawnBounds.min,
                    BallSpawnHeight = authoring.BallSpawnHeight,
                });

                Debug.Log("Config Authoring");
            }
        }
    }

    public struct Config : IComponentData
    {
        public Entity BallPrefab;
        public Vector3 enemyTarget;
        public float BallSpawnInterval;
        public int NumBallsSpawn;
        public float ImpactStrength;
        public float3 SpawnBoundsMax;
        public float3 SpawnBoundsMin;
        public float BallSpawnHeight;
    }
}