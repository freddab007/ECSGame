using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float enemySpeed = 5;
    public int health = 10;
    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

            AddComponent(entity, new Enemy
            {
                speed = authoring.enemySpeed,
                health = authoring.health,
            });
            print("Enemy authoring");
        }
    }
}

public struct Enemy : IComponentData
{
    public float speed;
    public int health;
}
