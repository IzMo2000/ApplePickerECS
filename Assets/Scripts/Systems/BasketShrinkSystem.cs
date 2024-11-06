using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct BasketSizeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BasketTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettingsComponent>();
        if (gameSettings.Level != 2)
            return;
        var deltaTime = SystemAPI.Time.DeltaTime;

        new ShrinkBasketJob
        {
            DeltaTime = deltaTime
        }.Schedule();
    }

    [BurstCompile]
    private partial struct ShrinkBasketJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        private void Execute(ref LocalTransform transform, ref BasketSizeData basketSizeData)
        {
            basketSizeData.ElapsedTime += DeltaTime;

            if (basketSizeData.IsShrinking)
            {
                if (basketSizeData.ElapsedTime >= basketSizeData.ShrinkDuration)
                {
                    basketSizeData.IsShrinking = false;
                    basketSizeData.ElapsedTime = 0f;
                    transform.Scale = basketSizeData.OriginalScale;
                }
            }
            else
            {
                if (basketSizeData.ElapsedTime >= basketSizeData.CooldownDuration)
                {
                    basketSizeData.IsShrinking = true;
                    basketSizeData.ElapsedTime = 0f;
                    transform.Scale = basketSizeData.ShrinkScale;
                }
            }
        }
    }
}


