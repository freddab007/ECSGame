using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TurretAuthoring : MonoBehaviour
{
    public float turretCooldown = 5;
    public float range = 5;
    public int damage = 10;
    public GameObject turretRotation;
    //test
    public class Baker : Baker<TurretAuthoring>
    {
        public override void Bake(TurretAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            Entity childEntitie = new Entity();
            childEntitie = GetEntity(authoring.turretRotation, TransformUsageFlags.Dynamic);

            AddComponent(entity, new Turret
            {
                cooldown = authoring.turretCooldown,
                damage = authoring.damage,
                range = authoring.range,
                turretRotation = childEntitie,
            });
            print("Turret authoring");
        }
    }
}

public struct Turret : IComponentData
{
    public float cooldown;
    public int damage;
    public float range;
    public Entity turretRotation;
}