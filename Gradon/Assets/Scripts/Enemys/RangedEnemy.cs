// RangedEnemy.cs
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Configura��es do Ranged Enemy")]
    [SerializeField] private GameObject projectilePrefab; // Prefab do proj�til
    [SerializeField] private Transform firePoint; // Ponto de onde o proj�til � instanciado
    [SerializeField] private float fireRate = 1.5f; // Tempo entre os disparos
    [SerializeField] private int projectileDamage = 30; // Dano que o proj�til causa

    private float fireCooldown = 0f; // Contador para o cooldown de disparo

    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base
        // Sobrescreve atributos da base se desejar
        // health = 60; // Inimigo � dist�ncia pode ter menos vida
        // speed = 2.5f;
        attackRange = 6.0f; // Define um alcance de ataque maior para o inimigo � dist�ncia
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
            Debug.LogWarning("RangedEnemy n�o pode atacar: Prefab do proj�til, FirePoint ou Target ausente.", this);
            return;
        }

        // Instancia o proj�til no FirePoint
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // Configura o proj�til com o alvo e o dano
            projectileScript.Seek(target, projectileDamage);
            Debug.Log($"RangedEnemy atirou um proj�til no Totem, com {projectileDamage} de dano!");
        }
        else
        {
            Debug.LogError("O prefab do proj�til n�o possui um componente Projectile.", newProjectile);
        }
    }
}