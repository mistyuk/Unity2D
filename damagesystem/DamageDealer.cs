using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    public DamageStats damageStats;
    public GameObject damageTextPrefab; // Reference to the DamageText prefab
    public Canvas damageTextCanvas; // Reference to the DamageTextCanvas in the scene

    private bool lastDirectionLeft = false; // To keep track of last direction

    public void Attack(Transform target)
    {
        if (damageStats == null)
        {
            Debug.LogError("DamageStats not assigned in DamageDealer.");
            return;
        }

        // Calculate damage
        int damage = Random.Range(damageStats.minDamage, damageStats.maxDamage + 1);
        bool isCritical = Random.value <= damageStats.criticalHitChance;
        if (isCritical)
        {
            damage = Mathf.RoundToInt(damage * damageStats.criticalHitMultiplier);
        }

        // Determine knockback
        float knockback = Random.Range(damageStats.minKnockbackForce, damageStats.maxKnockbackForce);
        Vector2 knockbackDirection = (target.position - transform.position).normalized;

        // Create DamageInfo
        DamageInfo damageInfo = new DamageInfo(damage, isCritical, damageStats.criticalHitMultiplier, knockback, knockbackDirection);

        // Apply damage
        DamageReceiver receiver = target.GetComponent<DamageReceiver>();
        if (receiver != null)
        {
            receiver.TakeDamage(damageInfo);

            // Instantiate Damage Text
            if (damageTextPrefab != null && damageTextCanvas != null)
            {
                // Determine spawn position slightly above the target to avoid overlapping with the enemy sprite
                Vector3 spawnPosition = target.position + Vector3.up * 1f; // Adjust the offset as needed

                GameObject damageTextGO = Instantiate(damageTextPrefab, damageTextCanvas.transform);
                DamageText damageText = damageTextGO.GetComponent<DamageText>();
                if (damageText != null)
                {
                    if (!isCritical)
                    {
                        // Alternate direction: right then left in a cycle
                        Vector3 direction = lastDirectionLeft ? new Vector3(-0.5f, 0.4f, 0) : new Vector3(0.5f, 0.4f, 0);
                        lastDirectionLeft = !lastDirectionLeft; // Toggle direction for next hit
                        damageText.Initialize(damage, isCritical, direction);
                    }
                    else
                    {
                        // Critical hits appear in the center without floating
                        damageText.Initialize(damage, isCritical, Vector3.zero);
                    }

                    damageTextGO.transform.position = spawnPosition;
                }
            }
            else
            {
                Debug.LogWarning("DamageTextPrefab or DamageTextCanvas not assigned in DamageDealer.");
            }
        }
        else
        {
            Debug.LogWarning("Target does not implement DamageReceiver.");
        }
    }
}
