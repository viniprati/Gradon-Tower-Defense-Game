using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Configura��es do Alvo")]
    [Tooltip("Arraste o objeto do Totem para este campo no Inspector.")]
    [SerializeField] private Transform totemAlvo; // A refer�ncia para o Transform do Totem

    [Header("Configura��es de Movimento")]
    [Tooltip("A velocidade com que o Boss se move em dire��o ao Totem.")]
    [SerializeField] private float velocidade = 2f;

    // Update � chamado a cada frame
    void Update()
    {
        // 1. VERIFICA��O DE SEGURAN�A:
        // Primeiro, verificamos se o 'totemAlvo' foi definido.
        // Se o totem for destru�do ou n�o for atribu�do, o Boss para e evitamos erros.
        if (totemAlvo == null)
        {
            // Se n�o h� alvo, n�o faz nada.
            return;
        }

        // 2. L�GICA DE MOVIMENTO:
        // Usamos Vector2.MoveTowards para mover o Boss em linha reta na dire��o do totem.
        // Par�metros:
        // - transform.position: A posi��o atual do Boss.
        // - totemAlvo.position: A posi��o do alvo (Totem).
        // - velocidade * Time.deltaTime: A dist�ncia m�xima que o Boss pode se mover neste frame.
        //    (Time.deltaTime garante que o movimento seja suave e independente do FPS).
        transform.position = Vector2.MoveTowards(transform.position, totemAlvo.position, velocidade * Time.deltaTime);

        // 3. (OPCIONAL) FAZER O BOSS VIRAR PARA O LADO CERTO:
        // Este trecho de c�digo faz o Boss virar o sprite para a esquerda ou direita.
        // Ele assume que o seu sprite do Boss, por padr�o, est� virado para a DIREITA.
        if (transform.position.x > totemAlvo.position.x)
        {
            // Se o Boss est� � DIREITA do totem, vira para a ESQUERDA (escala X negativa)
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x < totemAlvo.position.x)
        {
            // Se o Boss est� � ESQUERDA do totem, vira para a DIREITA (escala X positiva)
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Voc� pode adicionar uma fun��o p�blica para definir o alvo via c�digo, se precisar.
    public void DefinirAlvo(Transform novoAlvo)
    {
        totemAlvo = novoAlvo;
    }
}