// Assets/Scripts/PlayerAttackHitbox.cs
using UnityEngine;
using System.Collections; // Added to support IEnumerator

public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Attack Parameters")]
    public int baseDamage = 30;
    public float hitboxActivationDelay = 0.1f; // Delay before hitbox becomes active
    public float hitboxActiveDuration = 0.3f;  // Duration the hitbox remains active

    private Collider2D hitboxCollider;
    private Transform playerTransform;
    private DamageDealer damageDealer;

    void Awake()
    {
        // Get the Collider2D component
        hitboxCollider = GetComponent<Collider2D>();
        if (hitboxCollider == null)
        {
            Debug.LogError("Collider2D component missing on PlayerAttackHitbox.");
        }

        // Assume the player is the root of this object
        playerTransform = transform.root;

        // Get the DamageDealer component from the player
        damageDealer = GetComponentInParent<DamageDealer>();
        if (damageDealer == null)
        {
            Debug.LogError("DamageDealer component not found in parent.");
        }

        // Ensure the hitbox is initially disabled
        hitboxCollider.enabled = false;
    }

    void OnEnable()
    {
        // Start the activation coroutine when the hitbox is enabled
        StartCoroutine(ActivateHitboxRoutine());
    }

    void OnDisable()
    {
        // Ensure the collider is disabled when the hitbox is deactivated
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Coroutine to handle the timing of hitbox activation and deactivation.
    /// </summary>
    IEnumerator ActivateHitboxRoutine()
    {
        // Wait for the activation delay to sync with the attack animation
        yield return new WaitForSeconds(hitboxActivationDelay);

        // Enable the hitbox collider
        if (hitboxCollider != null)
            hitboxCollider.enabled = true;

        // Wait for the duration the hitbox should remain active
        yield return new WaitForSeconds(hitboxActiveDuration);

        // Disable the hitbox collider
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Get the DamageReceiver component from the enemy
            DamageReceiver receiver = collision.GetComponent<DamageReceiver>();
            if (receiver != null && damageDealer != null && damageDealer.damageStats != null)
            {
                // Determine if the hit is a critical hit
                bool isCritical = UnityEngine.Random.value <= damageDealer.damageStats.criticalHitChance;

                // Calculate final damage
                int damageAmount = isCritical
                    ? Mathf.RoundToInt(baseDamage * damageDealer.damageStats.criticalHitMultiplier)
                    : baseDamage;

                // Calculate knockback direction (from player to enemy)
                Vector2 knockbackDirection = (collision.transform.position - playerTransform.position).normalized;

                // Calculate knockback force within the defined range
                float knockbackForce = Random.Range(damageDealer.damageStats.minKnockbackForce, damageDealer.damageStats.maxKnockbackForce);

                // Create DamageInfo struct with all necessary information
                DamageInfo damageInfo = new DamageInfo(damageAmount, isCritical, damageDealer.damageStats.criticalHitMultiplier, knockbackForce, knockbackDirection);

                // Apply damage to the enemy
                receiver.TakeDamage(damageInfo);
            }
            else
            {
                Debug.LogWarning("Enemy does not have a DamageReceiver component or DamageDealer is not properly set.");
            }
        }
    }
}
