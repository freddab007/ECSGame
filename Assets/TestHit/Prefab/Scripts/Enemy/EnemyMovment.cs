using ActivationPlates;
using Unity.Burst;
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

    [BurstCompile]
    public void OnCreate(ref SystemState _state)
    {
        _state.RequireForUpdate<ActivationPlates.Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState _state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        float dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new EnemyDeath
        {
            ECB = ecbSingleton.CreateCommandBuffer(_state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();

        new EnemyMovement
        {
            ECB = ecbSingleton.CreateCommandBuffer(_state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();


        //foreach (var (_enemy, trans, localTrans, velocity) in
        //             SystemAPI.Query<RefRW<Enemy>, RefRO<LocalToWorld>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>())
        //{


        //    //if (velocity.ValueRW.Linear.y == 0)
        //    {
        //        var dir = (config.enemyTarget - (Vector3)trans.ValueRO.Position).normalized;
        //        Vector3 forward = Vector3.Normalize(trans.ValueRO.Forward);
        //        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        //        float3 velo = forward * _enemy.ValueRO.speed;
        //        velo.y = velocity.ValueRO.Linear.y;
        //        localTrans.ValueRW.Rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

        //        velocity.ValueRW.Linear = velo;
        //    }
        //}


    }
}


[WithAll(typeof(Enemy))]
[BurstCompile]
public partial struct EnemyDeath : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, Enemy enemy, ref LocalTransform cubeTransform)
    {
        if (enemy.isDead)
        {
            ECB.DestroyEntity(chunkIndex, entity);
        }
    }
}


[WithAll(typeof(Enemy))]
[BurstCompile]
public partial struct EnemyMovement : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex, Enemy _enemy, ref LocalTransform _enemyTransform, ref LocalToWorld _worldTransform, ref PhysicsVelocity _velocity)
    {
        var dir = ((Vector3)_enemy.target - (Vector3)_enemyTransform.Position).normalized;
        Vector3 forward = Vector3.Normalize(_worldTransform.Forward);
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        float3 velo = forward * _enemy.speed;
        velo.y = _velocity.Linear.y;
        _enemyTransform.Rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

        _velocity.Linear = velo;
    }
}
