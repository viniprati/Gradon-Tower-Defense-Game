// NormalEnemy.cs
using UnityEngine;

public class NormalEnemy : Enemy
{
    [Header("Configura��es do Normal Enemy")]
    [SerializeField] private int attackDamage = 20; // Dano espec�fico do Normal Enemy por ataque

    protected override void Start()
    {
        base.Start(); // Chama o Start da classe base para inicializar Rigidbody, Target, etc.
        // Sobrescreve atributos da base se desejar (ou defina no Inspector)
        // health = 80; 
        // speed = 4f; 
        // attackRange = 1.0f;
        // decelerationStartDistance = 4f;
    }

    // O Update() da classe base j� chama MoveTowardsTarget(), ent�o n�o precisamos de um Update() aqui
    // a menos que haja l�gica espec�fica para este inimigo a cada frame que n�o seja de movimento.

    public override void Attack()
    {
        if (target != null && Totem.instance != null)
        {
            // Aplica dano direto ao Totem
            Totem.instance.TakeDamage(attackDamage);
            Debug.Log($"NormalEnemy atacou o Totem, causando {attackDamage} de dano!");
            // Pode adicionar um cooldown para o ataque se necess�rio, ou uma anima��o.
        }
    }
}