// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy // Garante que herda da classe base correta
{
    [Header("Atributos do Inimigo a Dist�ncia")]
    [Tooltip("O alcance no qual este inimigo para de se mover e come�a a atirar.")]
    [SerializeField] private float attackRange = 8f; // VARI�VEL REINTRODUZIDA AQUI

    [SerializeField] private float fireRate = 1f; // Tiros por segundo
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint; // Ponto de onde o proj�til sai
    [SerializeField] private int damage = 10; // Dano espec�fico deste inimigo

    private float fireCooldownTimer;

    // O Update agora controla a l�gica de ataque baseada em tempo
    protected override void Update()
    {
        // Chama a l�gica do Update da classe base (ex: virar o sprite para o alvo)
        base.Update();

        // Se o inimigo est� morto ou n�o tem alvo, n�o faz nada
        if (isDead || currentTarget == null) return;

        // Reduz o cooldown do tiro
        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
        }

        // Verifica se pode atacar (se est� no alcance E o cooldown acabou)
        if (Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            TryAttack();
        }
    }

    // A l�gica de movimento para o inimigo a dist�ncia � parar QUANDO estiver no alcance
    protected override Vector2 HandleMovement()
    {
        if (currentTarget == null || isDead) return Vector2.zero;

        // Calcula a dist�ncia at� o alvo
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Se estiver DENTRO do alcance de ataque, para de se mover.
        if (distanceToTarget <= attackRange)
        {
            return Vector2.zero;
        }
        else // Se estiver longe demais, continua se movendo em dire��o ao alvo.
        {
            return moveDirection; // 'moveDirection' � calculado na classe base
        }
    }

    private void TryAttack()
    {
        if (fireCooldownTimer <= 0f)
        {
            Fire();
            fireCooldownTimer = 1f / fireRate; // Reseta o cooldown
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null) return;

        Vector3 spawnPosition = firePoint.position;
        Vector2 direction = (Vector2)currentTarget.position - (Vector2)spawnPosition;
        GameObject projectileGO = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Assumindo que voc� tem um script 'Projectile.cs'
        // Projectile projectileScript = projectileGO.GetComponent<Projectile>();
        // if (projectileScript != null) { projectileScript.Launch(direction, this.damage); }

        Debug.Log(gameObject.name + " atirou em " + currentTarget.name);
    }

    // Gizmo para visualizar o alcance de ataque espec�fico deste inimigo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}