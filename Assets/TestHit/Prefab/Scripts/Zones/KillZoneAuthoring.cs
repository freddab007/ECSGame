using Unity.Entities;
using UnityEngine;

public class KillZoneAuthoring : MonoBehaviour
{
    private class Baker : Baker<KillZoneAuthoring>
    {
        public override void Bake(KillZoneAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);

            AddComponent(entity, new Zone());
            print("Zone authoring");
        }
    }
}

public struct Zone : IComponentData
{

}
