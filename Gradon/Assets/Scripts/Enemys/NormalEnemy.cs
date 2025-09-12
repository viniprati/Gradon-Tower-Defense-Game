// NormalEnemy.cs
using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configurações do Normal Enemy")]
    [SerializeField] private int attackDamage = 20; // Dano específico do Normal Enemy por ataque

    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base para inicializar Rigidbody, Target, etc.
        // Sobrescreve atributos da base se desejar (ou defina no Inspector)
        // health = 80; 
        // speed = 4f; 
        // attackRange = 1.0f;
        // decelerationStartDistance = 4f;
    }

    // O Update() da classe base já chama MoveTowardsTarget(), então não precisamos de um Update() aqui
    // a menos que haja lógica específica para este inimigo a cada frame que não seja de movimento.

    public override void Attack()
    {
        if (target != null && Totem.instance != null)
        {
            // Aplica dano direto ao Totem
            Totem.instance.TakeDamage(attackDamage);
            Debug.Log($"NormalEnemy atacou o Totem, causando {attackDamage} de dano!");
            // Pode adicionar um cooldown para o ataque se necessário, ou uma animação.
        }
    }
}