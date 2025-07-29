using UnityEngine;
using UnityEngine.SceneManagement; // trocar de cena

public class MenuController : MonoBehaviour
{
    // A função Update é chamada uma vez por frame
    void Update()
    {
        // Verifica se a tecla ESPAÇO foi pressionada NESTE frame
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame(); // Chama a nossa função para iniciar o jogo
        }
    }

    // Função que carrega a cena do jogo
    public void StartGame()
    {
        // Certifique-se de que o nome da cena está EXATAMENTE igual ao do seu arquivo.
        // Pela sua imagem, o nome é "Game".
        SceneManager.LoadScene("Game");
    }
}