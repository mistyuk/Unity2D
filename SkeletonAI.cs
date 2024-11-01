// Assets/Scripts/SkeletonAI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
[RequireComponent(typeof(DamageDealer))]
public class SkeletonAI : MonoBehaviour, DamageReceiver
{
    [Header("Enemy Data")]
    public EnemyData enemyData; // Assign via Inspector

    [Header("Damage Stats")]
    [SerializeField] private DamageStats damageStats;
    // Add hurtTimer declaration in the class variables section
    [SerializeField] private float hurtTimer = 0f; // Timer for hurt state duration

    // Components
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public DamageDealer damageDealer;

    // Animation Clips
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkAnimation;
    [SerializeField] private AnimationClip attackAnimation1;
    [SerializeField] private AnimationClip attackAnimation2;
    [SerializeField] private AnimationClip hurtAnimation;
    [SerializeField] private AnimationClip deathAnimation;

    // Movement Parameters
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f; // Speed when chasing the player
    private Vector2 movementDirection;

    // Player Reference
    private Transform player;

    // AI States
    private enum State { Idle, Patrol, Chase, Attack, Hurt, Retreat, Dead }
    private State currentState = State.Idle;

    // Patrol Parameters
    [Header("Patrol Parameters")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float pauseDuration = 2f; // Time to pause at each waypoint
    private int currentPatrolIndex = 0;
    private float pauseTimer = 0f;
    private bool isPausedAtWaypoint = false;

    // Attack Parameters
    [Header("Attack Parameters")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 2f;
    private float attackCooldownTimer = 0f;
    [SerializeField] private GameObject attackHitbox;
    // Removed 'attackDamage' as it's now handled via DamageStats

    // Timing Parameters for Hitbox Activation
    [Header("Hitbox Timing Parameters")]
    [SerializeField] private float hitboxActivationDelay = 0.3f; // Delay before activating hitbox
    [SerializeField] private float hitboxActiveDuration = 0.5f;  // Duration the hitbox remains active

    // Health Parameters
    [Header("Health Parameters")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private Image healthBarImage;

    // Detection Parameters
    [Header("Detection Parameters")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float followRange = 10f; // Maximum distance to follow the player

    // Hurt Parameters
    [Header("Hurt Parameters")]
    private bool isHurt = false;
    [SerializeField] private float hurtDuration = 0.5f;
    // Removed 'knockbackForce' as it's now handled via DamageInfo

    // Ground Check
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = false;

    // Retreat Parameters
    [Header("Retreat Parameters")]
    [SerializeField] private float retreatDuration = 1f;
    private float retreatTimer = 0f;

    // Attack State Flag
    private bool isAttacking = false;

    // Animator Parameter Hashes
    private int isWalkingHash;
    private int isAttackingHash;
    private int isHurtHash;
    private int isDeadHash;

    // Debug Options
    [Header("Debug Options")]
    [SerializeField] private bool enableDebug = true;

    // Invincibility Parameters
    [Header("Invincibility Parameters")]
    [SerializeField] private float invincibilityDuration = 1f; // Duration after being hit during which the skeleton cannot be hit again
    private bool canBeHit = true;

    // Events
    public event Action<int, int> OnHealthChanged; // currentHealth, maxHealth

    // Animation Cycle
    private AnimationClip currentAttackAnimation = null;
    private int attackComboStep = 0;



    void Awake()
    {
        // Initialize Components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageDealer = GetComponent<DamageDealer>();

        // Animator Controller Check
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator Controller is missing on the Skeleton. Please assign it in the Animator component.");
        }

        // Cache Animator parameter hashes
        isWalkingHash = Animator.StringToHash("isWalking");
        isAttackingHash = Animator.StringToHash("isAttacking");
        isHurtHash = Animator.StringToHash("isHurt");
        isDeadHash = Animator.StringToHash("isDead");

        // Initialize Health
        currentHealth = maxHealth;
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }

        // Find the player by script reference
        GameObject playerObj = FindObjectOfType<PlayerController2D>()?.gameObject;
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found. Ensure the player GameObject exists and has the PlayerController2D script.");

        // Apply zero friction physics material to prevent pushing
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            // Assign a physics material with zero friction in the Inspector for better performance
            if (col.sharedMaterial == null)
            {
                PhysicsMaterial2D zeroFrictionMaterial = new PhysicsMaterial2D("ZeroFriction");
                zeroFrictionMaterial.friction = 0f;
                zeroFrictionMaterial.bounciness = 0f;
                col.sharedMaterial = zeroFrictionMaterial;
            }
        }

        // Ensure attack hitbox is disabled initially
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
        else
            Debug.LogError("Attack Hitbox is not assigned in SkeletonAI.");
    }

    void Start()
    {
        // Subscribe to LevelUp event if applicable
        // Skeleton doesn't level up, but can access GameManager if needed
    }

    void Update()
    {
        if (currentState != State.Dead)
        {
            if (isHurt)
            {
                hurtTimer -= Time.deltaTime;
                if (hurtTimer <= 0)
                {
                    isHurt = false;
                    currentState = State.Chase; // Return to chasing the player
                }
            }
            else
            {
                StateMachine();
            }

            UpdateAnimations();

            if (attackCooldownTimer > 0)
                attackCooldownTimer -= Time.deltaTime;
        }

        CheckGrounded();
    }

    void FixedUpdate()
    {
        if (currentState == State.Patrol || currentState == State.Chase || currentState == State.Retreat)
        {
            if (!isPausedAtWaypoint)
            {
                Move();
            }
        }
    }

    // Ground Check
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // State Machine Logic
    void StateMachine()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = State.Chase;
                }
                else
                {
                    currentState = State.Patrol;
                }
                break;

            case State.Patrol:
                Patrol();
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                if (distanceToPlayer > followRange)
                {
                    currentState = State.Patrol;
                    break;
                }

                if (distanceToPlayer > attackRange)
                {
                    // Move towards the player
                    ChasePlayer();
                }
                else
                {
                    // Stop moving forward when in attack range
                    movementDirection = Vector2.zero;

                    if (attackCooldownTimer <= 0 && !isAttacking)
                    {
                        StartAttack();
                    }
                }
                break;

            case State.Attack:
                // Attack logic is handled in the attack coroutine
                break;

            case State.Retreat:
                Retreat();
                break;

            case State.Hurt:
                // Hurt behavior is handled in Update()
                break;

            case State.Dead:
                // Do nothing
                break;
        }
    }

    // Movement Methods
    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            // No patrol points, stay idle
            movementDirection = Vector2.zero;
            return;
        }

        if (isPausedAtWaypoint)
        {
            movementDirection = Vector2.zero;
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                isPausedAtWaypoint = false;
                // Move to the next patrol point
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        movementDirection = (targetPoint.position - transform.position).normalized;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Reached patrol point, pause
            isPausedAtWaypoint = true;
            pauseTimer = pauseDuration;
            movementDirection = Vector2.zero;
        }
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        // Calculate horizontal movement direction only
        float directionX = player.position.x - transform.position.x;
        movementDirection = new Vector2(directionX, 0).normalized;

        // Flip sprite to face the player
        if (directionX > 0)
            spriteRenderer.flipX = false;
        else if (directionX < 0)
            spriteRenderer.flipX = true;
    }

    void Retreat()
    {
        if (player == null)
            return;

        // Move backward from the player
        float directionX = transform.position.x - player.position.x; // Opposite direction
        movementDirection = new Vector2(directionX, 0).normalized;

        // Keep sprite facing the player
        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false; // Face right
        else if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true; // Face left

        retreatTimer -= Time.deltaTime;
        if (retreatTimer <= 0)
        {
            // Return to chase state
            currentState = State.Chase;
        }
    }

    void Move()
    {
        if (!isGrounded)
            return;

        float speed = (currentState == State.Chase) ? chaseSpeed : moveSpeed;
        Vector2 newPosition = rb.position + movementDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Handle sprite flipping based on movement direction, except when retreating
        if (currentState != State.Retreat)
        {
            if (movementDirection.x > 0)
                spriteRenderer.flipX = false;
            else if (movementDirection.x < 0)
                spriteRenderer.flipX = true;
        }
        // In retreat state, sprite flipping is handled in Retreat()
    }

    // Attack Methods
    void StartAttack()
    {
        Debug.Log("Starting Attack");
        currentState = State.Attack;
        isAttacking = true;
        attackCooldownTimer = attackCooldown;

        // Stop movement
        movementDirection = Vector2.zero;
        rb.velocity = Vector2.zero;

        // Cycle through attack animations
        if (attackComboStep == 0)
        {
            currentAttackAnimation = attackAnimation1;
            attackComboStep = 1;
        }
        else
        {
            currentAttackAnimation = attackAnimation2;
            attackComboStep = 0;
        }

        // Trigger attack in Animator
        animator.SetTrigger("Attack");
        Debug.Log("Attack Trigger Set");

        // Start the attack coroutine
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        Debug.Log("Attack Routine Started");

        // Optional delay before activating hitbox to sync with animation
        yield return new WaitForSeconds(hitboxActivationDelay);

        // Activate the attack hitbox
        ActivateAttackHitbox();

        // Wait for the duration you want the hitbox to be active
        yield return new WaitForSeconds(hitboxActiveDuration);

        // Deactivate the attack hitbox
        DeactivateAttackHitbox();

        // Wait for the remainder of the attack animation
        float remainingAttackTime = currentAttackAnimation.length - hitboxActivationDelay - hitboxActiveDuration;
        if (remainingAttackTime > 0)
            yield return new WaitForSeconds(remainingAttackTime);
        else
            yield return null; // Prevent negative wait time

        Debug.Log("Attack Routine Ended");

        // Attack animation finished
        isAttacking = false;

        // Start retreating after attack
        currentState = State.Retreat;
        retreatTimer = retreatDuration;
    }

    // Update Animations
    void UpdateAnimations()
    {
        if (currentState == State.Dead)
        {
            animator.SetBool(isDeadHash, true);
            return;
        }
        else
        {
            animator.SetBool(isDeadHash, false);
        }

        if (currentState == State.Hurt)
        {
            animator.SetBool(isHurtHash, true);
            return;
        }
        else
        {
            animator.SetBool(isHurtHash, false);
        }

        if (currentState == State.Attack || isAttacking)
        {
            animator.SetBool(isAttackingHash, true);
            return;
        }
        else
        {
            animator.SetBool(isAttackingHash, false);
        }

        // Set walking state
        bool isMoving = movementDirection.magnitude > 0.1f;
        if (animator.GetBool(isWalkingHash) != isMoving)
            animator.SetBool(isWalkingHash, isMoving);
    }

    // Health Management
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (currentState == State.Dead)
            return;

        if (!canBeHit)
            return; // Prevent taking damage during invincibility frames

        canBeHit = false; // Disable further damage

        currentHealth = Mathf.Clamp(currentHealth - damageInfo.damageAmount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Apply knockback
            rb.velocity = Vector2.zero; // Reset velocity
            rb.AddForce(damageInfo.knockbackDirection.normalized * damageInfo.knockbackForce, ForceMode2D.Impulse);

            // Play hurt animation
            isHurt = true;
            hurtTimer = hurtDuration;
            currentState = State.Hurt;

            // Trigger hurt in Animator
            animator.SetTrigger("Hurt");

            // Start invincibility coroutine
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        canBeHit = true; // Re-enable taking damage
    }

    void Die()
    {
        Debug.Log("Skeleton Died");
        currentState = State.Dead;
        animator.SetTrigger("Die");

        // Trigger the OnEnemyDeath event
        if (enemyData != null && EventManager.Instance != null)
        {
            EventManager.Instance.OnEnemyDeath.Invoke(enemyData);
            Debug.Log($"Enemy '{enemyData.enemyName}' died. Experience Reward: {enemyData.experienceReward}");
        }
        else
        {
            Debug.LogWarning("EnemyData or EventManager Instance is missing.");
        }

        // Disable movement and collider
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Disable attack hitbox
        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        // Destroy the enemy after some time
        Destroy(gameObject, 2f); // Adjust time as needed
    }

    // Methods to control the attack hitbox directly
    public void ActivateAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            Debug.Log("Attack Hitbox Activated");
        }
        else
            Debug.LogError("Attack Hitbox is not assigned in SkeletonAI.");
    }

    public void DeactivateAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            Debug.Log("Attack Hitbox Deactivated");
        }
        else
            Debug.LogError("Attack Hitbox is not assigned in SkeletonAI.");
    }

    // OnDrawGizmos for Debugging
    void OnDrawGizmos()
    {
        if (!enableDebug)
            return;

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw follow range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followRange);

        // Draw patrol points and lines
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.1f);

                    // Draw line to next patrol point
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }

                    // Close the loop if needed
                    if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }

#if UNITY_EDITOR
                    UnityEditor.Handles.Label(patrolPoints[i].position, patrolPoints[i].name);
#endif
                }
            }
        }

        // Draw ground check
#if UNITY_EDITOR
        if (groundCheck != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
#endif
    }
}
