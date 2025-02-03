using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ActivationPlates
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public Transform target;

        public class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    enemyTarget = authoring.target.position,
                });

                Debug.Log("Config Authoring");
            }
        }
    }

    public struct Config : IComponentData
    {
        public Vector3 enemyTarget;
    }
}