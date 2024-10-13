using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, DamageType damageTypeValue);

    void FloatingDamage(Vector3 position, int damage, DamageType damageTypeValue);
}
