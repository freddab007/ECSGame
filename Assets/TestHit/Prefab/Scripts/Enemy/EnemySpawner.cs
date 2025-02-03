using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct EnemySpawner : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState _state)
    {
        //_state.RequireForUpdate<EnemySpawnerAuthoring>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState _state)
    {
        float dt = SystemAPI.Time.DeltaTime;


        foreach (var _spawner in SystemAPI.Query<RefRW<EnemySpawnerComp>>())
        {
            if (_spawner.ValueRO.TimerSpawner <= 0)
            {
                //test = true;
                _spawner.ValueRW.TimerSpawner = _spawner.ValueRO.BallSpawnInterval;
                var newEnemies = _state.EntityManager.Instantiate(_spawner.ValueRO.BallPrefab, _spawner.ValueRO.NumBallsSpawn, Allocator.Temp);


                var rand = new Random((uint)math.max(1, math.ceil(SystemAPI.Time.ElapsedTime)));

                var min = _spawner.ValueRO.SpawnBoundsMin;
                var max = _spawner.ValueRO.SpawnBoundsMax;
                min.y += _spawner.ValueRO.BallSpawnHeight;
                max.y += _spawner.ValueRO.BallSpawnHeight;


                foreach (var _enemy in newEnemies)
                {
                    var trans = SystemAPI.GetComponentRW<LocalTransform>(_enemy);
                    var pos = rand.NextFloat3(min, max);
                    trans.ValueRW.Position = pos;
                }

            }
            _spawner.ValueRW.TimerSpawner -= dt;
        }








    }


}
