// EnemySeparation.cs (Versão 2.0 - Mais Robusta)
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySeparation : MonoBehaviour
{
    [Header("Configuração da Separação")]
    [Tooltip("O raio no qual este inimigo detecta outros para se afastar.")]
    [SerializeField] private float separationRadius = 1.5f;

    [Tooltip("A força máxima de repulsão quando dois inimigos estão quase se tocando.")]
    [SerializeField] private float maxSeparationForce = 10f;

    [Tooltip("A Layer em que os outros inimigos estão.")]
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        ApplySeparationForce();
    }

    private void ApplySeparationForce()
    {
        // Encontra todos os colliders de inimigos próximos dentro do raio de separação.
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyLayer);

        // Itera sobre cada vizinho encontrado.
        foreach (var neighbor in nearbyEnemies)
        {
            // Garante que não está se comparando consigo mesmo.
            if (neighbor.gameObject == this.gameObject)
            {
                continue;
            }

            // Calcula a distância e a direção para longe do vizinho.
            Vector2 awayFromNeighbor = transform.position - neighbor.transform.position;
            float distance = awayFromNeighbor.magnitude;

            // Se a distância for zero (caso raro), ignora para evitar divisão por zero.
            if (distance == 0) continue;

            // --- Lógica de Atenuação ---
            // A força de repulsão é inversamente proporcional à distância.
            // Quanto mais perto o vizinho, mais forte é o "empurrão".
            float repulsionStrength = 1.0f - (distance / separationRadius);

            // Calcula a força final a ser aplicada.
            Vector2 force = awayFromNeighbor.normalized * repulsionStrength * maxSeparationForce;

            // Aplica a força ao Rigidbody.
            rb.AddForce(force);
        }
    }

    // Desenha o raio de separação no editor para facilitar o debug.
    private void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}