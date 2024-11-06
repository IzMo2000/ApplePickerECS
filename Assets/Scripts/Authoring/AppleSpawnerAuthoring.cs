using Unity.Entities;
using UnityEngine;

public struct AppleSpawner : IComponentData
{
    public Entity Prefab;
    public Entity BadApplePrefab;        // Prefab for the bad apple

    public float Interval;
    public int SpawnedCount;
}

[DisallowMultipleComponent] 
public class AppleSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private GameObject badApplePrefab;
    [SerializeField] private float appleSpawnInterval = 1f;

    private class AppleSpawnerAuthoringBaker : Baker<AppleSpawnerAuthoring>
    {
        public override void Bake(AppleSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new AppleSpawner
            {
                Prefab = GetEntity(authoring.applePrefab, TransformUsageFlags.Dynamic),
                BadApplePrefab = GetEntity(authoring.badApplePrefab, TransformUsageFlags.Dynamic),
                Interval = authoring.appleSpawnInterval,
                SpawnedCount = 0
            });
            AddComponent(entity, new Timer { Value = 2f });
        }
    }
}
