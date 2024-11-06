using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(TimerSystem))]
public partial struct AppleSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var gameSettings = SystemAPI.GetSingleton<GameSettingsComponent>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        new SpawnJob { ECB = ecb , Level = gameSettings.Level}.Schedule();


    }

    [BurstCompile]
    private partial struct SpawnJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public int Level;

        private void Execute(in LocalTransform transform, ref AppleSpawner spawner, ref Timer timer)
        {
            if (timer.Value > 0)
                return;

            timer.Value = spawner.Interval;
            Entity appleEntity;

            if (Level != 0)
            {
                spawner.SpawnedCount++;

                if (spawner.SpawnedCount == 5)
                {
                    appleEntity = ECB.Instantiate(spawner.BadApplePrefab);
                    spawner.SpawnedCount = 0;
                }
                else
                {
                    appleEntity = ECB.Instantiate(spawner.Prefab);
                }
            }

            else
            {
                appleEntity = ECB.Instantiate(spawner.Prefab);
            }

            ECB.SetComponent(appleEntity, LocalTransform.FromPosition(transform.Position));
        }
    }
}
