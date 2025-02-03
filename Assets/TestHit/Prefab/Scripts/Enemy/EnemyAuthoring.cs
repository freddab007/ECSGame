using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float enemySpeed = 5;
    public int health = 10;
    public Transform target;
    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent<NewSpawn>(entity);
            AddComponent(entity, new Enemy
            {
                speed = authoring.enemySpeed,
                health = authoring.health,
                isDead = false,
            });
            print("Enemy authoring");
        }
    }
}

public struct Enemy : IComponentData
{
    public float speed;
    public int health;
    public float3 target;
    public bool isDead;
}

public struct NewSpawn : IComponentData
{

}
