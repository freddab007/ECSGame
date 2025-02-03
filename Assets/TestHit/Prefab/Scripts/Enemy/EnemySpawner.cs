using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using static UnityEngine.GraphicsBuffer;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct EnemySpawner : ISystem
{
    uint seedOffset;

    [BurstCompile]
    public void OnCreate(ref SystemState _state)
    {
        _state.RequireForUpdate<EnemySpawnerComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState _state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var newSpawnQuery = SystemAPI.QueryBuilder().WithAll<NewSpawn>().Build();
        _state.EntityManager.RemoveComponent<NewSpawn>(newSpawnQuery);

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        //foreach (var _spawner in SystemAPI.Query<RefRW<EnemySpawnerComp>>())
        //{
        //    if (_spawner.ValueRO.TimerSpawner <= 0)
        //    {


        //        _spawner.ValueRW.TimerSpawner = _spawner.ValueRO.BallSpawnInterval;
        //        _state.EntityManager.Instantiate(_spawner.ValueRO.BallPrefab, _spawner.ValueRO.NumBallsSpawn, Allocator.Temp);

        //        SeedOffset += (uint)_spawner.ValueRO.NumBallsSpawn;

        //        new RandomPositionJob
        //        {
        //            seedOffset = SeedOffset,
        //            spawnBoundsMin = _spawner.ValueRO.SpawnBoundsMin,
        //            spawnBoundsMax = _spawner.ValueRO.SpawnBoundsMax,
        //            target = _spawner.ValueRO.TargetSpawn,
        //            ballSpawnHeight = _spawner.ValueRO.BallSpawnHeight,

        //        }.ScheduleParallel();
        //    }
        //    _spawner.ValueRW.TimerSpawner -= dt;
        //}
    }

    [WithAll(typeof(EnemySpawnerComp))]
    [BurstCompile]
    public partial struct LaunchSpawning : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECBT;
        public float dt;
        public uint SeedOffset;

        public void Execute([EntityIndexInQuery] int index, ref EnemySpawnerComp _spawner)
        {
            if (_spawner.TimerSpawner <= 0)
            {
                _spawner.TimerSpawner = _spawner.BallSpawnInterval;
                JobHandle handle = new SpawnEnemy
                {
                    ECB = ECBT,
                    Prefab = _spawner.BallPrefab,
                }.Schedule(_spawner.NumBallsSpawn, _spawner.NumBallsSpawn / 5); // 100 instances, batch size = 32

                // Assurer la complétion du job avant d'appliquer les commandes
                handle.Complete(); new RandomPositionJob
                {
                    seedOffset = SeedOffset,
                    spawnBoundsMin = _spawner.SpawnBoundsMin,
                    spawnBoundsMax = _spawner.SpawnBoundsMax,
                    target = _spawner.TargetSpawn,
                    ballSpawnHeight = _spawner.BallSpawnHeight,

                }.ScheduleParallel();
            }

            _spawner.TimerSpawner -= dt;
        }
    }

    [BurstCompile]
    partial struct SpawnEnemy : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity Prefab;

        public void Execute([EntityIndexInQuery] int index)
        {
            ECB.Instantiate(index, Prefab);
        }
    }


    [WithAll(typeof(NewSpawn))]
    [BurstCompile]
    partial struct RandomPositionJob : IJobEntity
    {
        public uint seedOffset;
        public float3 spawnBoundsMin;
        public float3 spawnBoundsMax;
        public float3 target;
        public float ballSpawnHeight;

        public void Execute([EntityIndexInQuery] int index, ref Enemy _entity, ref LocalTransform transform)
        {
            var rand = Random.CreateFromIndex(seedOffset + (uint)index);

            var min = spawnBoundsMin;
            var max = spawnBoundsMax;
            min.y += ballSpawnHeight;
            max.y += ballSpawnHeight;

            _entity.target = target;


            var pos = rand.NextFloat3(min, max);
            transform.Position = pos;
        }
    }
}
