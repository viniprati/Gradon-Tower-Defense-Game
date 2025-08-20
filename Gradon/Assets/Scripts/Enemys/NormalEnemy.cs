// NormalEnemy.cs
using UnityEngine;

public class NormalEnemy : EnemyController // Garante que está herdando da classe base correta
{
    // A única coisa que ele precisa fazer é definir como se mover.
    protected override Vector2 HandleMovement()
    {
        // CORREÇÃO: Usa a variável 'currentTarget' herdada da classe base.
        if (currentTarget != null)
        {
            // A classe base já calcula a direção em 'moveDirection', então podemos simplesmente retorná-la.
            return moveDirection;
        }

        // Se, por algum motivo, não houver alvo, ele fica parado.
        return Vector2.zero;
    }

    // Você precisará adicionar uma lógica de ataque para o inimigo normal (Ghoul) aqui
    // Exemplo:
    /*
    private void Update()
    {
        base.Update(); // Roda o Update da classe base
        
        // Se estiver perto o suficiente do alvo, ataca
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            // Lógica de ataque com cooldown aqui...
        }
    }
    */
}