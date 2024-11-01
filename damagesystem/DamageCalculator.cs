// Assets/Scripts/DamageCalculator.cs
using UnityEngine;

public static class DamageCalculator
{
    /// <summary>
    /// Calculates the damage based on DamageStats and determines if it's a critical hit.
    /// </summary>
    /// <param name="stats">DamageStats object containing damage parameters.</param>
    /// <returns>A tuple containing the final damage amount and a boolean indicating a critical hit.</returns>
    public static (int finalDamage, bool isCritical) CalculateDamage(DamageStats stats)
    {
        // Randomly determine damage within the specified range
        int baseDamage = Random.Range(stats.minDamage, stats.maxDamage + 1);

        // Determine if the attack is a critical hit
        bool isCritical = Random.value <= stats.criticalHitChance;

        // Apply critical hit multiplier if applicable
        int finalDamage = isCritical ? Mathf.RoundToInt(baseDamage * stats.criticalHitMultiplier) : baseDamage;

        return (finalDamage, isCritical);
    }

    /// <summary>
    /// Calculates the knockback force based on DamageStats.
    /// </summary>
    /// <param name="stats">DamageStats object containing knockback parameters.</param>
    /// <returns>A float representing the knockback force.</returns>
    public static float CalculateKnockbackForce(DamageStats stats)
    {
        return Random.Range(stats.minKnockbackForce, stats.maxKnockbackForce);
    }
}
