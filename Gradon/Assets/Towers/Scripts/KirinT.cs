// KirinT.cs
using UnityEngine;
using System.Collections.Generic;

public class KirinT : MonoBehaviour
{
    [Header("Atributos de Buff do Kirin")]
    [SerializeField] private float buffRadius = 4f;
    [SerializeField] private float attackRateMultiplier = 1.5f;
    [SerializeField] private float checkInterval = 1.0f;

    // A lista agora guarda referências à sua classe base correta: TowerBase
    private List<TowerBase> buffedTowers = new List<TowerBase>();

    void Start()
    {
        InvokeRepeating("ApplyBuffToNearbyTowers", 0f, checkInterval);
    }

    private void ApplyBuffToNearbyTowers()
    {
        buffedTowers.RemoveAll(tower => tower == null);
        foreach (TowerBase tower in new List<TowerBase>(buffedTowers)) // Usa TowerBase
        {
            if (Vector2.Distance(transform.position, tower.transform.position) > buffRadius)
            {
                tower.RemoveBuff(); // Supondo que RemoveBuff() está em TowerBase
                buffedTowers.Remove(tower);
            }
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, buffRadius, LayerMask.GetMask("Tower"));

        foreach (var col in colliders)
        {
            // Tenta pegar o componente da sua classe base correta: TowerBase
            TowerBase tower = col.GetComponent<TowerBase>();
            if (tower != null && !buffedTowers.Contains(tower))
            {
                tower.ApplyBuff(attackRateMultiplier); // Supondo que ApplyBuff() está em TowerBase
                buffedTowers.Add(tower);
            }
        }
    }

    void OnDestroy()
    {
        foreach (var tower in buffedTowers)
        {
            if (tower != null)
            {
                tower.RemoveBuff();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }
}