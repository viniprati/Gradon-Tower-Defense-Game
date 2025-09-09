// TowerBase.cs
using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    [Header("Informa��es da Torre")]
    public string towerName = "Torre Padr�o"; // <-- TORNADO P�BLICO

    [Header("Atributos Gerais da Torre")]
    // --- MUDAN�A PRINCIPAL: TORNADO 'public' PARA A UI LER ---
    public float attackRange = 5f;
    public float attackRate = 1f;
    public int cost = 50;
    public Sprite towerIcon;

    [Header("Refer�ncias (Setup no Prefab)")]
    [SerializeField] protected Transform partToRotate;
    [SerializeField] protected SpriteRenderer rangeIndicator;

    protected Transform currentTarget;
    protected float attackCooldown = 0f;
    protected float originalAttackRate;
    [SerializeField] protected string enemyTag = "Enemy";

    protected virtual void Start()
    {
        // ... seu c�digo de Start ...
        ShowRangeIndicator(false); // Esconde o indicador ao iniciar
    }

    // --- ADI��O NECESS�RIA PARA A UI ---
    /// <summary>
    /// Mostra ou esconde o c�rculo de alcance visual.
    /// </summary>
    public void ShowRangeIndicator(bool isVisible)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = isVisible;
        }
    }

    // ... todo o resto do seu TowerBase.cs (Update, FindTarget, Attack, etc.) ...
    protected virtual void Update() { /* ... */ }
    private void FindTarget() { /* ... */ }
    private void HandleRotation() { /* ... */ }
    protected abstract void Attack();
    public virtual void ApplyBuff(float rateMultiplier) { /* ... */ }
    public virtual void RemoveBuff() { /* ... */ }
    protected virtual void OnDrawGizmosSelected() { /* ... */ }
}