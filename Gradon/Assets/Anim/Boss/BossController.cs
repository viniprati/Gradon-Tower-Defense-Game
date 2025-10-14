using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Configurações do Alvo")]
    [Tooltip("Arraste o objeto do Totem para este campo no Inspector.")]
    [SerializeField] private Transform totemAlvo; // A referência para o Transform do Totem

    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade com que o Boss se move em direção ao Totem.")]
    [SerializeField] private float velocidade = 2f;

    // Update é chamado a cada frame
    void Update()
    {
        // 1. VERIFICAÇÃO DE SEGURANÇA:
        // Primeiro, verificamos se o 'totemAlvo' foi definido.
        // Se o totem for destruído ou não for atribuído, o Boss para e evitamos erros.
        if (totemAlvo == null)
        {
            // Se não há alvo, não faz nada.
            return;
        }

        // 2. LÓGICA DE MOVIMENTO:
        // Usamos Vector2.MoveTowards para mover o Boss em linha reta na direção do totem.
        // Parâmetros:
        // - transform.position: A posição atual do Boss.
        // - totemAlvo.position: A posição do alvo (Totem).
        // - velocidade * Time.deltaTime: A distância máxima que o Boss pode se mover neste frame.
        //    (Time.deltaTime garante que o movimento seja suave e independente do FPS).
        transform.position = Vector2.MoveTowards(transform.position, totemAlvo.position, velocidade * Time.deltaTime);

        // 3. (OPCIONAL) FAZER O BOSS VIRAR PARA O LADO CERTO:
        // Este trecho de código faz o Boss virar o sprite para a esquerda ou direita.
        // Ele assume que o seu sprite do Boss, por padrão, está virado para a DIREITA.
        if (transform.position.x > totemAlvo.position.x)
        {
            // Se o Boss está à DIREITA do totem, vira para a ESQUERDA (escala X negativa)
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x < totemAlvo.position.x)
        {
            // Se o Boss está à ESQUERDA do totem, vira para a DIREITA (escala X positiva)
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Você pode adicionar uma função pública para definir o alvo via código, se precisar.
    public void DefinirAlvo(Transform novoAlvo)
    {
        totemAlvo = novoAlvo;
    }
}