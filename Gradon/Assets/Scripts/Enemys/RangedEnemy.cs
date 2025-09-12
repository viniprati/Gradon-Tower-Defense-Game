// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Configurações do Ranged Enemy")]
    [SerializeField] private GameObject projectilePrefab; // Prefab do projétil
    [SerializeField] private Transform firePoint; // Ponto de onde o projétil é instanciado
    [SerializeField] private float fireRate = 1.5f; // Tempo entre os disparos
    [SerializeField] private int projectileDamage = 30; // Dano que o projétil causa

    private float fireCooldown = 0f; // Contador para o cooldown de disparo

    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base
        // Sobrescreve atributos da base se desejar
        // health = 60; // Inimigo à distância pode ter menos vida
        // speed = 2.5f;
        attackRange = 6.0f; // Define um alcance de ataque maior para o inimigo à distância
        // decelerationStartDistance = 8f;
    }

    protected override void Update()
    {
        base.Update(); // Chama o Update da classe base para lidar com o movimento

        // Gerencia o cooldown de ataque
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Attack(); // Chama o ataque quando o cooldown termina
                fireCooldown = fireRate; // Reseta o cooldown
            }
        }
    }

    public override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || target == null)
        {
            Debug.LogWarning("RangedEnemy não pode atacar: Prefab do projétil, FirePoint ou Target ausente.", this);
            return;
        }

        // Instancia o projétil no FirePoint
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // Configura o projétil com o alvo e o dano
            projectileScript.Seek(target, projectileDamage);
            Debug.Log($"RangedEnemy atirou um projétil no Totem, com {projectileDamage} de dano!");
        }
        else
        {
            Debug.LogError("O prefab do projétil não possui um componente Projectile.", newProjectile);
        }
    }
}