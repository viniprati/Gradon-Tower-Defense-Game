// LevelManager.cs

using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelData currentLevelData { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(LevelData levelToLoad)
    {
        if (levelToLoad == null)
        {
            Debug.LogError("Tentativa de carregar uma fase, mas os dados da fase são nulos!");
            return;
        }

        currentLevelData = levelToLoad;

        SceneManager.LoadScene(currentLevelData.sceneToLoad);
    }
}