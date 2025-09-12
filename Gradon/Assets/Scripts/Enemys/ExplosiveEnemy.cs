// ExplosiveEnemy.cs
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Configura��es do Explosive Enemy")]
    [SerializeField] private float explosionRadius = 3f; // Raio da explos�o
    [SerializeField] private int explosionDamage = 75; // Dano da explos�o

    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base
        // Sobrescreve atributos da base se desejar
        // health = 120; // Explosive Enemy pode ter mais vida
        // speed = 2.0f; // Pode ser mais lento
        // attackRange = 0.5f; // Precisa estar bem perto para "atacar" (explodir)
        // decelerationStartDistance = 3f;
    }

    // Este inimigo pode usar o MoveTowardsTarget() da classe base para se aproximar e desacelerar.
    // Ou, se quiser um movimento sempre linear, pode sobrescrever o Update() e fazer:
    // protected override void Update()
    // {
    //     if (!IsDead && target != null)
    //     {
    //         Vector3 direction = (target.position - transform.position).normalized;
    //         rb.linearVelocity = direction * speed; // Sem desacelera��o aqui
    //         if (Vector3.Distance(transform.position, target.position) <= attackRange)
    //         {
    //             Attack(); // Explode ao alcan�ar
    //         }
    //     }
    //     else
    //     {
    //         rb.linearVelocity = Vector2.zero;
    //     }
    // }

    public override void Attack()
    {
        // Neste caso, o ataque � a explos�o.
        // Pode ser chamado ao atingir o alvo (se voc� quiser que ele exploda ao contato)
        // ou apenas ao morrer (como no Die()).
        PerformExplosion();
        // Se explodiu e causou dano, ele deve se autodestruir.
        Die();
    }

    protected override void Die()
    {
        // Garante que a explos�o ocorra ao morrer, mesmo que n�o tenha chegado ao alvo.
        if (!IsDead) // Para evitar explos�o dupla se Attack() j� chamou Die()
        {
            PerformExplosion();
        }
        base.Die(); // Chama a l�gica de morte da classe base
    }

    /// <summary>
    /// L�gica para realizar a explos�o, causando dano em �rea.
    /// </summary>
    private void PerformExplosion()
    {
        Debug.Log("ExplosiveEnemy explodiu! Causando dano em �rea.");
        // --- L�gica real de dano em �rea ---
        // Voc� pode usar Physics2D.OverlapCircleAll para encontrar colliders no raio
        // e aplicar dano a eles. Por exemplo:
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in colliders)
        {
            // Exemplo: Se o Totem estiver no raio, cause dano a ele.
            if (hitCollider.CompareTag("Totem")) // Assumindo que seu Totem tem a tag "Totem"
            {
                Totem totem = hitCollider.GetComponent<Totem>();
                if (totem != null)
                {
                    totem.TakeDamage(explosionDamage);
                    Debug.Log($"Explos�o causou {explosionDamage} de dano ao Totem.");
                }
            }
            // Voc� pode expandir para causar dano a outros inimigos pr�ximos ou ao jogador
            // if (hitCollider.CompareTag("Player")) { ... }
        }

        // Opcional: Adicionar um efeito visual/sonoro de explos�o
        // Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected(); // Desenha os Gizmos da classe base
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}