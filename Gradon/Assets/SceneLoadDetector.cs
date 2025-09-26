// SceneLoadDetector.cs - Nosso "Detetive"
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; // Essencial para pausar o editor
#endif

public class SceneLoadDetector : MonoBehaviour
{
    private string _initialSceneName;

    void Awake()
    {
        // Guarda o nome da cena inicial para compara��o
        _initialSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[Detetive] Inciando na cena: '{_initialSceneName}'");
    }

    void OnEnable()
    {
        // Se inscreve no evento que � chamado QUANDO uma nova cena � carregada
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se a nova cena carregada for a mesma que a inicial, encontramos o criminoso!
        if (scene.name == _initialSceneName)
        {
            Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.LogError($"[DETETIVE] CULPADO ENCONTRADO! Um script recarregou a cena '{_initialSceneName}' indevidamente.");
            Debug.LogError("O JOGO SER� PAUSADO AGORA. Olhe o 'Call Stack' para ver o script respons�vel.");
            Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            // Pausa o editor da Unity no exato momento do crime
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
        }
    }
}