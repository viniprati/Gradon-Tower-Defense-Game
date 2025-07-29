// MenuController.cs (Modificado para Toque/Clique)
using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para trocar de cena

public class MenuController : MonoBehaviour
{
    // A função Update é chamada uma vez por frame
    void Update()
    {
        // --- LÓGICA DE INPUT PARA TOQUE/CLIQUE ---

        if (Input.GetMouseButtonDown(0))
        {
            StartGame(); 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    // Função que carrega a cena do jogo
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}