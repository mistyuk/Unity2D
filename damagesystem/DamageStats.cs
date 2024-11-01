// Assets/Scripts/DamageStats.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageStats", menuName = "Damage System/Damage Stats")]
public class DamageStats : ScriptableObject
{
    [Header("Damage Parameters")]
    [Tooltip("Minimum damage that can be dealt.")]
    public int minDamage = 10;

    [Tooltip("Maximum damage that can be dealt.")]
    public int maxDamage = 20;

    [Header("Critical Hit Parameters")]
    [Tooltip("Chance (0 to 1) to land a critical hit.")]
    [Range(0f, 1f)]
    public float criticalHitChance = 0.1f; // 10%

    [Tooltip("Multiplier applied on critical hits.")]
    public float criticalHitMultiplier = 1.5f;

    [Header("Knockback Parameters")]
    [Tooltip("Minimum knockback force.")]
    public float minKnockbackForce = 5f;

    [Tooltip("Maximum knockback force.")]
    public float maxKnockbackForce = 10f;
}
    