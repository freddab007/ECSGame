using ActivationPlates;
using NUnit.Framework.Constraints;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct EnemyMovment : ISystem
{
    //bool test;
    private float spawnTimer;

    [BurstCompile]
    public void OnCreate(ref SystemState _state)
    {
        spawnTimer = 0;
        _state.RequireForUpdate<ActivationPlates.Config>();
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState _state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        float dt = SystemAPI.Time.DeltaTime;
        if (spawnTimer <= 0)
        {
            //test = true;
            spawnTimer = config.BallSpawnInterval;
            var newEnemies = _state.EntityManager.Instantiate(config.BallPrefab, config.NumBallsSpawn, Allocator.Temp);

            
            var rand = new Random((uint)math.max(1, math.ceil(SystemAPI.Time.ElapsedTime)));

            var min = config.SpawnBoundsMin;
            var max = config.SpawnBoundsMax;
            min.y += config.BallSpawnHeight;
            max.y += config.BallSpawnHeight;
            

            foreach (var _enemy in newEnemies)
            {
                var trans = SystemAPI.GetComponentRW<LocalTransform>(_enemy);
                var pos = rand.NextFloat3(min, max);
                trans.ValueRW.Position = pos;
            }

        }

        spawnTimer -= dt;



        foreach (var (_enemy, trans, localTrans, velocity) in
                     SystemAPI.Query<RefRW<Enemy>, RefRO<LocalToWorld>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>())
        {
            //if (velocity.ValueRW.Linear.y == 0)
            {
                var dir = (config.enemyTarget - (Vector3)trans.ValueRO.Position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                float3 velo = trans.ValueRO.Forward * _enemy.ValueRO.speed;
                velo.y = velocity.ValueRO.Linear.y;
                localTrans.ValueRW.Rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

                velocity.ValueRW.Linear = velo;
            }
        }
        
    }
}
