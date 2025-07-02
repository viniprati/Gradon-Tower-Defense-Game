// Tower.cs
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [Header("Atributos da Torre (Base)")]
    [Tooltip("O alcance de detecção e ataque da torre.")]
    [SerializeField] protected float attackRange = 5f;

    [Tooltip("O custo para construir esta torre.")]
    [SerializeField] public int cost = 50;

    protected Transform currentTarget;
    private string enemyTag = "Enemy";

    // A lógica agora é mais simples: encontrar um alvo e, se tiver um, atacar todo frame.
    protected virtual void Update()
    {
        FindTarget();

        if (currentTarget != null)
        {
            Attack();
        }
    }

    private void FindTarget()
    {
        // Se já temos um alvo, verifica se ele ainda está no alcance ou se foi destruído
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) > attackRange)
        {
            currentTarget = null;
        }

        // Se não temos alvo, procura um novo que esteja mais próximo
        if (currentTarget == null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
            float closestDistance = Mathf.Infinity;
            Transform newTarget = null;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    newTarget = collider.transform;
                }
            }
            currentTarget = newTarget;
        }
    }

    // Método abstrato para o ataque, que agora será chamado continuamente.
    protected abstract void Attack();

    // Desenha o alcance no editor da Unity para facilitar o debug.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}