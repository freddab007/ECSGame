using Unity.Entities;
using UnityEngine;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Physics;
using Unity.Collections;


namespace KillZoneCol
{
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct KillZoneActivation : ISystem
    {

        private void EnemyDetection(TriggerEvent _triggerEvent, EntityCommandBuffer _ecb, ref SystemState _state)
        {
            Entity zoneEntity;
            Entity enemyEntity;

            if (SystemAPI.HasComponent<Zone>(_triggerEvent.EntityA) && SystemAPI.HasComponent<Enemy>(_triggerEvent.EntityB))
            {
                zoneEntity = _triggerEvent.EntityA;
                enemyEntity = _triggerEvent.EntityB;
            }
            else if (SystemAPI.HasComponent<Enemy>(_triggerEvent.EntityA) && SystemAPI.HasComponent<Zone>(_triggerEvent.EntityB))
            {
                enemyEntity = _triggerEvent.EntityA;
                zoneEntity = _triggerEvent.EntityB;
            }
            else
            {
                return;
            }

            _ecb.DestroyEntity(enemyEntity);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState _state)
        {
                var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();
                sim.FinalJobHandle.Complete();

                var ecb = new EntityCommandBuffer(Allocator.Temp);
                foreach (var _triggerEvent in sim.TriggerEvents)
                {
                    EnemyDetection(_triggerEvent, ecb, ref _state);

                }
                ecb.Playback(_state.EntityManager);
        }
    }
}

