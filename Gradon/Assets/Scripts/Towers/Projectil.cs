// Projectile.cs (Versão Universal - Adaptada)
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    [Header("Efeitos")]
    [SerializeField] private GameObject hitEffectPrefab;

    // Variáveis Internas
    private Rigidbody2D rb;
    private float currentDamage; // ALTERAÇÃO 1: Dano agora é float para compatibilidade
    private bool hasHit = false; // Garante que o dano seja aplicado apenas uma vez

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Configura e DISPARA o projétil em direção a um alvo.
    /// Usado tanto por Torres quanto por Inimigos.
    /// </summary>
    public void Seek(Transform _target, float damageFromAttacker) // ALTERAÇÃO 2: Recebe float
    {
        this.currentDamage = damageFromAttacker;

        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (_target.position - transform.position).normalized;
        rb.velocity = direction * speed;
        transform.up = direction; // Rotaciona o projétil para apontar na direção
    }

    /// <summary>
    /// Chamado pela física da Unity quando o projétil toca em algo.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se o projétil já acertou algo, ignora colisões futuras
        if (hasHit) return;

        // --- ALTERAÇÃO 3: LÓGICA DE ALVO UNIVERSAL ---

        // VERIFICA SE ATINGIU UM INIMIGO
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead)
        {
            hasHit = true; // Marca que acertou
            enemy.TakeDamage(currentDamage);
            HandleImpact();
            return; // Sai da função após o acerto
        }

        // SE NÃO FOR UM INIMIGO, VERIFICA SE ATINGIU O TOTEM
        Totem totem = other.GetComponent<Totem>();
        if (totem != null && !totem.IsDestroyed)
        {
            hasHit = true; // Marca que acertou
            totem.TakeDamage(currentDamage);
            HandleImpact();
        }
    }

    /// <summary>
    /// Centraliza a lógica de pós-impacto (efeitos visuais e destruição).
    /// </summary>
    private void HandleImpact()
    {
        Debug.Log($"<color=green>Acerto VÁLIDO!</color> Projétil causou {currentDamage} de dano.");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destrói o projétil
        Destroy(gameObject);
    }
}