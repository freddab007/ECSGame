using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject BallPrefab;

    public float BallSpawnInterval = 4f;
    public int NumBallsSpawn = 10;
    public Bounds SpawnBounds;
    public float BallSpawnHeight; // how high the ball spawning bounds should be above the brick spawning bounds 

    public class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            authoring.SpawnBounds.center = authoring.gameObject.transform.position;
            AddComponent(entity, new EnemySpawnerComp
            {
                BallPrefab = GetEntity(authoring.BallPrefab, TransformUsageFlags.Dynamic),
                SpawnBoundsMax = authoring.SpawnBounds.max,
                SpawnBoundsMin = authoring.SpawnBounds.min,
                BallSpawnHeight = authoring.BallSpawnHeight,
                BallSpawnInterval = authoring.BallSpawnInterval,
                NumBallsSpawn = authoring.NumBallsSpawn,
                TimerSpawner = 0,
            });
            print("Enemy Spawner authoring");
        }
    }
}

public struct EnemySpawnerComp : IComponentData
{
    public Entity BallPrefab;
    public float3 SpawnBoundsMax;
    public float3 SpawnBoundsMin;
    public float BallSpawnHeight;
    public float BallSpawnInterval;
    public float TimerSpawner;
    public int NumBallsSpawn;
}