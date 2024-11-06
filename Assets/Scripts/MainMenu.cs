using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;

public struct GameSettingsComponent : IComponentData
{
    public int Level; // 0 = Easy, 1 = Medium, 2 = Hard
}

[DisallowMultipleComponent]
public class MainMenu : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity gameSettingsEntity;

    private void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create or find the singleton entity for GameSettings
        gameSettingsEntity = entityManager.CreateEntity(typeof(GameSettingsComponent));
    }
    public void LoadEasyLevel()
    {
        SetLevel(0);
        // SceneLoader.Instance.LoadScene(SceneType.Easy);
        SceneManager.LoadScene("Easy");
    }

    public void LoadMediumLevel()
    {
        SetLevel(1);

        // SceneLoader.Instance.LoadScene(SceneType.Easy);
        SceneManager.LoadScene("Medium");
    }

    public void LoadHardLevel()
    {
        SetLevel(2);
        SceneManager.LoadScene("Hard");
    }

    private void SetLevel(int level)
    {
        // Update the GameSettingsComponent with the selected level
        entityManager.SetComponentData(gameSettingsEntity, new GameSettingsComponent { Level = level });
    }
}
