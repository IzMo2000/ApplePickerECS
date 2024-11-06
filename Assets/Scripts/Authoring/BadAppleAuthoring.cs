using Unity.Entities;
using UnityEngine;
public struct BadAppleTag : IComponentData
{
}

public struct BadAppleBottomY : IComponentData
{
    public float Value;
}

[DisallowMultipleComponent]
public class BadAppleAuthoring : MonoBehaviour
{
    [SerializeField] private float bottomY = -14f;  

    private class BadAppleAuthoringBaker : Baker<BadAppleAuthoring>
    {
        public override void Bake(BadAppleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BadAppleTag>(entity);
            AddComponent(entity, new BadAppleBottomY { Value = authoring.bottomY });
        }
    }
}

