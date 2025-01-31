using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;


[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct TurretLMovment : ISystem
{



    private void TurretRotation(TriggerEvent _triggerEvent, EntityCommandBuffer _ecb, ref SystemState _state, Vector3 _nearEntity)
    {
        Entity turretEntity;
        Entity enemyEntity;


        if (SystemAPI.HasComponent<Turret>(_triggerEvent.EntityA) && SystemAPI.HasComponent<Enemy>(_triggerEvent.EntityB))
        {
            turretEntity = _triggerEvent.EntityA;
            enemyEntity = _triggerEvent.EntityB;
        }
        else if (SystemAPI.HasComponent<Enemy>(_triggerEvent.EntityA) && SystemAPI.HasComponent<Turret>(_triggerEvent.EntityB))
        {
            enemyEntity = _triggerEvent.EntityA;
            turretEntity = _triggerEvent.EntityB;
        }
        else
        {
            return;
        }


        var turretComp = SystemAPI.GetComponentRW<Turret>(turretEntity);
        var turretPos = SystemAPI.GetComponentRO<LocalTransform>(turretEntity);
        var enemyPos = SystemAPI.GetComponentRO<LocalTransform>(enemyEntity);

        if (Vector3.Distance( turretPos.ValueRO.Position, _nearEntity) > Vector3.Distance(turretPos.ValueRO.Position, enemyPos.ValueRO.Position))
        {
            _nearEntity = (Vector3)enemyPos.ValueRO.Position;

            var dir = ((Vector3)enemyPos.ValueRO.Position - (Vector3)turretPos.ValueRO.Position).normalized;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            var tranRWRotat = SystemAPI.GetComponentRW<LocalTransform>(turretComp.ValueRO.turretRotation);

            tranRWRotat.ValueRW.Rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }


    }

    [BurstCompile]
    public void OnUpdate(ref SystemState _state)
    {
        var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();
        sim.FinalJobHandle.Complete();
        Vector3 test = new Vector3( 100000, 100000, 100000);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var _triggerEvent in sim.TriggerEvents)
        {
            TurretRotation(_triggerEvent, ecb, ref _state, test);

        }
        ecb.Playback(_state.EntityManager);
    }
}
