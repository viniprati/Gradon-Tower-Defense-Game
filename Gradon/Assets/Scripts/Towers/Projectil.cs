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
    private float currentDamage;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Seek(Transform _target, float damageFromAttacker)
    {
        this.currentDamage = damageFromAttacker;

        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (_target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        transform.up = direction;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // --- LINHA DE DEBUG ADICIONADA AQUI ---
        // Esta linha vai nos dizer TUDO o que o projétil toca.
        Debug.Log($"PROJÉTIL TOCOU EM: {other.gameObject.name} | LAYER: {LayerMask.LayerToName(other.gameObject.layer)} | TAG: {other.tag}");

        if (hasHit) return;

        // VERIFICA SE ATINGIU UM INIMIGO
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead())
        {
            hasHit = true;
            enemy.TakeDamage(currentDamage);
            HandleImpact();
            return;
        }

        // SE NÃO FOR UM INIMIGO, VERIFICA SE ATINGIU O TOTEM
        Totem totem = other.GetComponent<Totem>();
        if (totem != null && !totem.IsDestroyed)
        {
            hasHit = true;
            totem.TakeDamage(currentDamage);
            HandleImpact();
        }
    }

    private void HandleImpact()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}