// IDamageable.cs
public interface IDamageable
{
    // Qualquer script que implementar esta interface DEVE ter um método TakeDamage.
    void TakeDamage(float damage);
}