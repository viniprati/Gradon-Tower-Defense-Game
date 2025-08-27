// DragaoAtirador.cs

using UnityEngine;

public class DragaoAtirador : MonoBehaviour
{
    [Header("Atributos de Ataque")]
    [Tooltip("Dano que cada proj�til causa.")]
    [SerializeField] private int damage = 15;
    [Tooltip("Quantos ataques por segundo a torre realiza.")]
    [SerializeField] private float attackRate = 0.8f;
    [Tooltip("Dist�ncia m�xima que a torre consegue enxergar e atacar inimigos.")]
    [SerializeField] private float attackRange = 7.0f;

    [Header("Refer�ncias")]
    [Tooltip("Arraste o Prefab do proj�til aqui.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Ponto exato de onde o proj�til ser� disparado (ex: a boca do drag�o).")]
    [SerializeField] private Transform firePoint;
    [Tooltip("A tag usada para identificar os inimigos na cena.")]
    [SerializeField] private string enemyTag = "Inimigo";

    // --- Vari�veis de Controle Interno ---
    private Transform currentTarget;     // O inimigo que est� sendo focado atualmente
    private float attackCountdown = 0f;  // Timer para controlar a cad�ncia de tiro

    // Update � chamado a cada frame
    void Update()
    {
        // Se n�o tem um alvo, tenta encontrar um novo.
        if (currentTarget == null)
        {
            UpdateTarget();
        }
        else
        {
            // Se o alvo atual saiu do alcance, perde o foco nele.
            if (Vector2.Distance(transform.position, currentTarget.position) > attackRange)
            {
                currentTarget = null;
            }
        }

        // Diminui o contador do timer de ataque.
        attackCountdown -= Time.deltaTime;

        // Se o timer zerou e temos um alvo v�lido, ataca.
        if (attackCountdown <= 0f && currentTarget != null)
        {
            Attack();
            // Reseta o timer baseado na cad�ncia de tiro (attackRate).
            attackCountdown = 1f / attackRate;
        }
    }

    /// <summary>
    /// Procura por todos os inimigos na cena e define o mais pr�ximo como alvo.
    /// </summary>
    void UpdateTarget()
    {
        // Encontra todos os GameObjects com a tag de inimigo.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Itera por cada inimigo encontrado para achar o mais pr�ximo.
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Se um inimigo foi encontrado e est� dentro do alcance, define-o como o alvo.
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            currentTarget = nearestEnemy.transform;
        }
    }

    /// <summary>
    /// Cont�m a l�gica para disparar um proj�til.
    /// </summary>
    void Attack()
    {
        // Checagem de seguran�a para evitar erros.
        if (projectilePrefab == null || firePoint == null) return;

        // 1. Instancia (cria) uma c�pia do prefab do proj�til.
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // 2. Obt�m o componente de script do proj�til rec�m-criado.
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();

        // 3. Se o script foi encontrado, passa as "ordens" para ele.
        if (projectileScript != null)
        {
            projectileScript.Seek(currentTarget, this.damage);
        }
    }

    /// <summary>
    /// Desenha o raio de alcance da torre no Editor da Unity para facilitar o posicionamento.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}