// DragaoAtirador.cs

using UnityEngine;

public class DragonT : TowerCardUI
{
    [Header("Atributos de Ataque")]
    [Tooltip("Dano que cada projétil causa.")]
    [SerializeField] private int damage = 15;
    [Tooltip("Quantos ataques por segundo a torre realiza.")]
    [SerializeField] private float attackRate = 0.8f;
    [Tooltip("Distância máxima que a torre consegue enxergar e atacar inimigos.")]
    [SerializeField] private float attackRange = 7.0f;

    [Header("Referências")]
    [Tooltip("Arraste o Prefab do projétil aqui.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Ponto exato de onde o projétil será disparado (ex: a boca do dragão).")]
    [SerializeField] private Transform firePoint;
    [Tooltip("A tag usada para identificar os inimigos na cena.")]
    [SerializeField] private string enemyTag = "Enemy";

    // --- Variáveis de Controle Interno ---
    private Transform currentTarget;     // O inimigo que está sendo focado atualmente
    private float attackCountdown = 0f;  // Timer para controlar a cadência de tiro

    // Update é chamado a cada frame
    void Update()
    {
        // Se não tem um alvo, tenta encontrar um novo.
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

        // Se o timer zerou e temos um alvo válido, ataca.
        if (attackCountdown <= 0f && currentTarget != null)
        {
            Attack();
            // Reseta o timer baseado na cadência de tiro (attackRate).
            attackCountdown = 1f / attackRate;
        }
    }

    /// <summary>
    /// Procura por todos os inimigos na cena e define o mais próximo como alvo.
    /// </summary>
    void UpdateTarget()
    {
        // Encontra todos os GameObjects com a tag de inimigo.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Itera por cada inimigo encontrado para achar o mais próximo.
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Se um inimigo foi encontrado e está dentro do alcance, define-o como o alvo.
        if (nearestEnemy != null && shortestDistance <= attackRange)
        {
            currentTarget = nearestEnemy.transform;
        }
    }

    /// <summary>
    /// Contém a lógica para disparar um projétil.
    /// </summary>
    void Attack()
    {
        // Checagem de segurança para evitar erros.
        if (projectilePrefab == null || firePoint == null) return;

        // 1. Instancia (cria) uma cópia do prefab do projétil.
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // 2. Obtém o componente de script do projétil recém-criado.
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