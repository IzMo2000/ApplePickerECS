using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

public partial struct CollectAppleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerScore>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var appleCount = new NativeArray<byte>(1, Allocator.TempJob);
        var badAppleCount = new NativeArray<byte>(1, Allocator.TempJob);


        state.Dependency = new CollisionJob
        {
            AppleLookup = SystemAPI.GetComponentLookup<AppleTag>(true),
            BadAppleLookup = SystemAPI.GetComponentLookup<BadAppleTag>(true),
            BasketLookup = SystemAPI.GetComponentLookup<BasketTag>(true),
            ECB = ecb,
            AppleCount = appleCount,
            BadAppleCount = badAppleCount
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();

        if (appleCount[0] == 1)
        {
            var playerScore = SystemAPI.GetSingleton<PlayerScore>();
            playerScore.Value += 100;
            SystemAPI.SetSingleton(playerScore);
        }

        if (badAppleCount[0] == 1)
        {
            var playerScore = SystemAPI.GetSingleton<PlayerScore>();
            playerScore.Value -= 50;
            SystemAPI.SetSingleton(playerScore);
        }

        appleCount.Dispose();
        badAppleCount.Dispose();
    }

    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<BadAppleTag> BadAppleLookup;
        [ReadOnly] public ComponentLookup<AppleTag> AppleLookup;
        [ReadOnly] public ComponentLookup<BasketTag> BasketLookup;

        public EntityCommandBuffer ECB;
        public NativeArray<byte> AppleCount;
        public NativeArray<byte> BadAppleCount;


        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA; // basket
            var entityB = collisionEvent.EntityB; // apple

            if (AppleLookup.HasComponent(entityA) && BasketLookup.HasComponent(entityB))
            {
                ECB.DestroyEntity(entityA);
                AppleCount[0] = 1;
            }
            else if (AppleLookup.HasComponent(entityB) && BasketLookup.HasComponent(entityA))
            {
                ECB.DestroyEntity(entityB);
                AppleCount[0] = 1;
            }

            if (BadAppleLookup.HasComponent(entityA) && BasketLookup.HasComponent(entityB))
            {
                ECB.DestroyEntity(entityA);
                BadAppleCount[0] = 1;
            }
            else if (BadAppleLookup.HasComponent(entityB) && BasketLookup.HasComponent(entityA))
            {
                ECB.DestroyEntity(entityB);
                BadAppleCount[0] = 1;
            }
        }
    }
}
