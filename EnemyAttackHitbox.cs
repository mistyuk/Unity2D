// Assets/Scripts/EnemyAttackHitbox.cs
using UnityEngine;
using System.Collections; // Added to support IEnumerator

public class EnemyAttackHitbox : MonoBehaviour
{
    private SkeletonAI skeletonAI;
    private Collider2D hitboxCollider;
    private DamageDealer damageDealer;

    void Awake()
    {
        skeletonAI = GetComponentInParent<SkeletonAI>();
        if (skeletonAI == null)
            Debug.LogError("SkeletonAI component not found in parent.");

        hitboxCollider = GetComponent<Collider2D>();
        if (hitboxCollider == null)
            Debug.LogError("Collider2D component missing on EnemyAttackHitbox.");

        // Ensure hitbox is disabled initially
        hitboxCollider.enabled = false;
    }

    void OnEnable()
    {
        // Enable the collider when the hitbox is activated
        hitboxCollider.enabled = true;
        Debug.Log("Attack Hitbox Enabled.");
    }

    void OnDisable()
    {
        // Disable the collider when the hitbox is deactivated
        hitboxCollider.enabled = false;
        Debug.Log("Attack Hitbox Disabled.");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the DamageReceiver component from the player
            DamageReceiver receiver = collision.GetComponent<DamageReceiver>();
            if (receiver != null && skeletonAI != null && skeletonAI.damageDealer != null && skeletonAI.damageDealer.damageStats != null)
            {
                // Determine if the hit is a critical hit
                bool isCritical = UnityEngine.Random.value <= skeletonAI.damageDealer.damageStats.criticalHitChance;

                // Calculate final damage
                int damageAmount = isCritical
                    ? Mathf.RoundToInt(skeletonAI.damageDealer.damageStats.minDamage * skeletonAI.damageDealer.damageStats.criticalHitMultiplier)
                    : skeletonAI.damageDealer.damageStats.minDamage;

                // Calculate knockback direction (from enemy to player)
                Vector2 knockbackDirection = (collision.transform.position - skeletonAI.transform.position).normalized;

                // Calculate knockback force within the defined range
                float knockbackForce = Random.Range(skeletonAI.damageDealer.damageStats.minKnockbackForce, skeletonAI.damageDealer.damageStats.maxKnockbackForce);

                // Create DamageInfo struct with all necessary information
                DamageInfo damageInfo = new DamageInfo(damageAmount, isCritical, skeletonAI.damageDealer.damageStats.criticalHitMultiplier, knockbackForce, knockbackDirection);

                // Apply damage to the player
                receiver.TakeDamage(damageInfo);
            }
            else
            {
                Debug.LogWarning("Player does not have a DamageReceiver component or DamageDealer is not properly set.");
            }
        }
    }
}
