// Assets/Scripts/DamageInfo.cs
using UnityEngine;

[System.Serializable]
public struct DamageInfo
{
    public int damageAmount;
    public bool isCritical;
    public float criticalHitMultiplier;
    public float knockbackForce;
    public Vector2 knockbackDirection;

    public DamageInfo(int damageAmount, bool isCritical, float criticalHitMultiplier, float knockbackForce, Vector2 knockbackDirection)
    {
        this.damageAmount = damageAmount;
        this.isCritical = isCritical;
        this.criticalHitMultiplier = criticalHitMultiplier;
        this.knockbackForce = knockbackForce;
        this.knockbackDirection = knockbackDirection;
    }
}
