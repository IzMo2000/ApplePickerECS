using Unity.Entities;
using UnityEngine;

public struct BasketTag : IComponentData
{
}

public struct BasketIndex : IComponentData
{
    public int Value;
}

public struct DestroyBasketTag : IComponentData
{
}

public struct BasketSizeData : IComponentData
{
    public float ElapsedTime; // Tracks elapsed time in seconds
    public float ShrinkDuration; // Duration of shrinking effect
    public float CooldownDuration; // Duration between shrinks
    public bool IsShrinking; // Flag to check if basket is currently shrinking
    public float OriginalScale; // Store original scale
    public float ShrinkScale; // Target scale when shrinking
}

public class BasketAuthoring : MonoBehaviour
{
    [SerializeField] private float shrinkDuration = 3f;
    [SerializeField] private float cooldownDuration = 5f;
    [SerializeField] private float shrinkScale = 0.5f; // Scale to shrink to
    private class BasketAuthoringBaker : Baker<BasketAuthoring>
    {
        public override void Bake(BasketAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<BasketTag>(entity);
            AddComponent<MoveWithMouse>(entity);
            AddComponent(entity, new BasketSizeData
            {
                ElapsedTime = 0f,
                ShrinkDuration = authoring.shrinkDuration,
                CooldownDuration = authoring.cooldownDuration,
                IsShrinking = false,
                OriginalScale = 1f, // Assuming initial scale is 1
                ShrinkScale = authoring.shrinkScale
            });
        }
    }
}
