// Assets/Scripts/PlayerController2D.cs
using UnityEngine;
using UnityEngine.UI; // For UI elements
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
[RequireComponent(typeof(DamageDealer))]
public class PlayerController2D : MonoBehaviour, DamageReceiver
{
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private DamageDealer damageDealer;

    // Movement Parameters
    [Header("Movement Parameters")]
    public float maxSpeed = 8f;
    public float acceleration = 70f;
    public float deceleration = 70f;
    public float airAccelerationMultiplier = 0.5f;
    public float airDecelerationMultiplier = 0.5f;

    // Gravity Parameters
    [Header("Gravity Parameters")]
    public float gravityScale = 5f;
    public float fallGravityMultiplier = 2f;
    public float maxFallSpeed = -20f;
    public float jumpCutMultiplier = 0.5f;

    // Jump Parameters
    [Header("Jump Parameters")]
    public float jumpForce = 15f;
    public int extraJumps = 1;
    private int jumpCount;
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    // Dash Parameters
    [Header("Dash Parameters")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    private float dashTimeCounter;
    private bool isDashing;

    // Dash Cooldown Parameters
    [Header("Dash Cooldown")]
    public float dashCooldown = 1f; // Cooldown duration in seconds
    private float dashCooldownTimer;
    private bool canDash = true;

    // Jump Apex Parameters
    [Header("Jump Apex Parameters")]
    public float apexThreshold = 10f;
    public float apexBonus = 2f;

    // Input
    private float moveInput;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool dashPressed;
    private bool crouchPressed;

    // Ground Check
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public bool isGrounded;
    private bool wasGrounded;

    // Animation Clips
    [Header("Animation Clips")]
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip fallAnimation;
    public AnimationClip landingAnimation;
    public AnimationClip dashAnimation;
    public AnimationClip attackAnimation1;
    public AnimationClip attackAnimation2;
    public AnimationClip attackAnimation3;
    public AnimationClip hurtAnimation;
    public AnimationClip crouchStartAnimation;
    public AnimationClip crouchIdleAnimation;
    public AnimationClip crouchWalkAnimation;
    public AnimationClip deathAnimation;

    private AnimationClip currentAnimation;

    // Unity Events for audio feedback
    public UnityEvent OnJump = new UnityEvent();
    public UnityEvent OnFall = new UnityEvent();
    public UnityEvent OnLand = new UnityEvent();
    public UnityEvent OnCrouch = new UnityEvent();
    public UnityEvent OnTakeDamage = new UnityEvent();
    public UnityEvent OnDeath = new UnityEvent();
    public UnityEvent OnAttack = new UnityEvent();




    // Attack Parameters
    [Header("Attack Parameters")]
    private bool isAttacking = false;
    private int attackComboStep = 0;
    public float attackDuration = 0.5f; // Duration of each attack animation
    private float attackTimer = 0f;
    public float comboResetTime = 1f; // Time window to continue the combo
    private float comboResetTimer = 0f;
    public GameObject attackHitbox; // Attack hitbox
    public float attackRange = 1.5f; // Attack range
    public float attackCooldownDuration = 1f; // Cooldown duration after attack
    private bool canAttack = true;

    // Landing Parameters
    [Header("Landing Parameters")]
    public bool enableLandingAnimation = true; // Option to enable/disable landing animation
    private bool isLanding = false;
    private float landingTimer = 0f;

    // Crouch Parameters
    [Header("Crouch Parameters")]
    public bool isCrouching = false;

    // Health Parameters
    [Header("Health Parameters")]
    public Image healthBarImage; // Health bar image

    // Death Parameters
    private bool isDead = false;

    // Hurt Parameters
    private bool isHurt = false;
    public float hurtDuration = 0.5f; // Duration of hurt animation
    private float hurtTimer = 0f;
    public float knockbackForce = 10f;

    // Damage Stats
    [Header("Damage Stats")]
    public DamageStats damageStats; // Assign via Inspector

    // Level and Experience System
    [Header("Level and Experience")]
    public PlayerStats playerStats; // Assign via Inspector
    public LevelManager levelManager; // Assign via Inspector

    // HUD References
    [Header("HUD Elements")]
    public Text levelText; // Assign via Inspector
    public Image experienceBarImage; // Assign via Inspector
    public Text experienceText; // Assign via Inspector

    // Variables to detect falling
    private bool wasFalling = false;

    void Awake()
    {
        // Initialize Components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageDealer = GetComponent<DamageDealer>();

        // Set Gravity Scale
        rb.gravityScale = gravityScale;

        // Ensure that the Animator Controller is assigned
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator Controller is missing. Please assign it in the Animator component.");
        }

        // Initialize Health via PlayerStats
        if (playerStats != null)
        {
            playerStats.currentHealth = playerStats.baseHealth;
            if (healthBarImage != null)
            {
                healthBarImage.fillAmount = (float)playerStats.currentHealth / playerStats.baseHealth;
            }
        }
        else
        {
            Debug.LogError("PlayerStats is not assigned in PlayerController2D.");
        }

        // Assign DamageStats to DamageDealer
        if (damageDealer != null)
        {
            damageDealer.damageStats = damageStats;
        }
        else
        {
            Debug.LogError("DamageDealer component is missing on the Player.");
        }

        // Ensure attack hitbox starts disabled
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
        else
        {
            Debug.LogError("Attack Hitbox is not assigned in PlayerController2D.");
        }
    }

    void Start()
    {
        // Find LevelManager if not assigned
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogError("LevelManager not found in the scene.");
            }
        }

        // Subscribe to LevelUp event
        if (levelManager != null)
        {
            levelManager.OnLevelUp.AddListener(OnLevelUp);
            UpdateStats();
            UpdateHUD();
        }
        else
        {
            Debug.LogError("LevelManager is not assigned in PlayerController2D.");
        }
    }

    void OnDestroy()
    {
        if (levelManager != null)
        {
            levelManager.OnLevelUp.RemoveListener(OnLevelUp);
        }
    }

    private void OnLevelUp()
    {
        UpdateStats();
        UpdateHUD();
    }

    private void UpdateStats()
    {
        if (levelManager == null || playerStats == null)
            return;

        int currentLevel = levelManager.currentLevel;

        // Update player stats based on level
        playerStats.currentHealth = playerStats.baseHealth + playerStats.healthPerLevel * (currentLevel - 1);
        playerStats.currentDefense = playerStats.baseDefense + playerStats.defensePerLevel * (currentLevel - 1);
        playerStats.currentAttackPower = playerStats.baseAttackPower + playerStats.attackPowerPerLevel * (currentLevel - 1);
        playerStats.currentCriticalChance = playerStats.baseCriticalChance + playerStats.criticalChancePerLevel * (currentLevel - 1);

        // Update DamageStats based on PlayerStats
        if (damageStats != null)
        {
            damageStats.minDamage = playerStats.currentAttackPower;
            damageStats.maxDamage = playerStats.currentAttackPower + 5; // Adjust as needed
            damageStats.criticalHitChance = playerStats.currentCriticalChance;
        }

        // Update Health Bar
        if (healthBarImage != null && playerStats != null)
        {
            healthBarImage.fillAmount = (float)playerStats.currentHealth / playerStats.baseHealth;
        }

        // Heal the player to full health upon leveling up
        playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, playerStats.baseHealth);
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)playerStats.currentHealth / playerStats.baseHealth;
        }
    }

    private void UpdateHUD()
    {
        if (levelText != null && experienceBarImage != null && experienceText != null && levelManager != null)
        {
            levelText.text = $"Level: {levelManager.currentLevel}";
            experienceBarImage.fillAmount = levelManager.GetExperienceProgress();
            experienceText.text = $"{levelManager.currentExperience}/{levelManager.GetCurrentExperienceRequirement()}";
        }
    }

    void Update()
    {
        if (isDead)
            return;

        if (isHurt)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                isHurt = false;
            }
            return; // Skip other inputs while hurt
        }

        // Handle Landing Timer
        if (isLanding)
        {
            landingTimer -= Time.deltaTime;
            if (landingTimer <= 0)
            {
                isLanding = false;
            }
        }

        // Get Inputs
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        dashPressed = Input.GetButtonDown("Fire3"); // Default "Fire3" is Left Shift
        crouchPressed = Input.GetAxisRaw("Vertical") < -0.5f;

        // Handle Crouching
        if (crouchPressed && isGrounded && !isCrouching)
        {
            StartCrouch();
        }
        else if (!crouchPressed && isCrouching)
        {
            StopCrouch();
        }

        // Coyote Time
        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;

        // Jump Buffer
        jumpBufferCounter = jumpPressed ? jumpBufferTime : jumpBufferCounter - Time.deltaTime;

        // Jump
        if (jumpBufferCounter > 0 && (coyoteTimeCounter > 0 || jumpCount > 0) && !isAttacking && !isDashing && !isCrouching)
        {
            Jump();
            jumpBufferCounter = 0;
        }

        // Dash Cooldown Timer
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
            }
        }

        // Dash
        if (dashPressed && canDash && moveInput != 0 && !isDashing && !isAttacking && !isCrouching)
        {
            StartDash();
        }

        // Attack Input
        if (Input.GetButtonDown("Fire1") && canAttack && !isAttacking && !isDashing && !isCrouching)
        {
            StartAttack();
            OnAttack.Invoke(); // Invoke the attack event
        }

        // Handle attack timers
        HandleAttackTimers();

        // Jump Cut
        if (!jumpHeld && rb.velocity.y > 0 && !isAttacking && !isDashing && !isCrouching)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
        }

        // Flip the character sprite based on movement direction
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Ground Check
        CheckGrounded();

        // Detect Landing
        if (enableLandingAnimation && isGrounded && !wasGrounded && rb.velocity.y <= 0)
        {
            isLanding = true;
            landingTimer = landingAnimation.length; // Duration of the landing animation
            OnLand.Invoke();
        }

        // Detect Falling
        if (!isGrounded && rb.velocity.y < 0 && !wasFalling)
        {
            OnFall.Invoke();
            wasFalling = true;
        }

        // Update wasGrounded and wasFalling
        wasGrounded = isGrounded;
        if (isGrounded)
        {
            wasFalling = false;
        }

        // Update Animations
        UpdateAnimations();

        // Update HUD continuously for experience bar
        if (levelManager != null)
        {
            UpdateHUD();
        }
    }

    void FixedUpdate()
    {
        if (isDead || isHurt)
            return;

        // Apply Gravity Multiplier
        rb.gravityScale = rb.velocity.y < 0 ? gravityScale * fallGravityMultiplier : gravityScale;

        // Clamp Fall Speed
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }

        // Horizontal Movement
        if (!isDashing)
        {
            float targetSpeed = moveInput * maxSpeed;

            // Reduce speed during attack or crouch
            if (isAttacking || isCrouching)
            {
                targetSpeed *= 0.5f; // Reduce speed to 50%
            }

            float speedDiff = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            // Adjust Acceleration in Air
            if (!isGrounded)
            {
                accelRate *= (Mathf.Abs(targetSpeed) > 0.01f) ? airAccelerationMultiplier : airDecelerationMultiplier;
            }

            // Apply Jump Apex Bonus
            float apexBonusModifier = Mathf.InverseLerp(0, apexThreshold, Mathf.Abs(rb.velocity.y));
            accelRate += apexBonus * apexBonusModifier;

            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, 0.9f) * Mathf.Sign(speedDiff);
            rb.AddForce(movement * Vector2.right);
        }

        // Dash Movement
        if (isDashing)
        {
            rb.velocity = new Vector2(dashSpeed * Mathf.Sign(moveInput), 0);
            dashTimeCounter -= Time.fixedDeltaTime;
            if (dashTimeCounter <= 0)
            {
                isDashing = false;
            }
        }
    }

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteTimeCounter = 0;
        if (!isGrounded)
        {
            jumpCount--;
        }
        OnJump.Invoke();
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeCounter = dashTime;
        dashCooldownTimer = dashCooldown;
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        comboResetTimer = comboResetTime; // Reset the combo reset timer
        canAttack = false; // Start attack cooldown

        // Play attack animation based on combo step
        PlayAnimation(GetCurrentAttackAnimation());

        // Invoke attack after a delay to sync with animation
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        // Wait for a portion of the attack duration before dealing damage
        float damageDelay = attackDuration * 0.3f; // Adjust based on animation timing
        yield return new WaitForSeconds(damageDelay);

        // Determine the target based on facing direction
        Transform target = GetAttackTarget();

        if (target != null)
        {
            damageDealer.Attack(target);
        }

        // Wait for the remainder of the attack duration
        yield return new WaitForSeconds(attackDuration - damageDelay);

        isAttacking = false;

        // Start attack cooldown
        StartCoroutine(AttackCooldownRoutine());
    }

    IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldownDuration);
        canAttack = true; // Re-enable attacking after cooldown
    }

    AnimationClip GetCurrentAttackAnimation()
    {
        if (attackComboStep == 0)
            return attackAnimation1;
        else if (attackComboStep == 1)
            return attackAnimation2;
        else
            return attackAnimation3;
    }

    Transform GetAttackTarget()
    {
        // Find the closest enemy within attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        Collider2D closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = hit;
            }
        }

        return closest?.transform;
    }

    void HandleAttackTimers()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }

        if (!canAttack)
        {
            comboResetTimer -= Time.deltaTime;
            if (comboResetTimer <= 0)
            {
                attackComboStep = 0;
                canAttack = true;
            }
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = extraJumps;
        }
    }

    void StartCrouch()
    {
        isCrouching = true;
        // Play crouch start animation
        PlayAnimation(crouchStartAnimation);
        OnCrouch.Invoke();
    }

    void StopCrouch()
    {
        isCrouching = false;
        // Play idle animation after stopping crouch
        PlayAnimation(idleAnimation);
    }

    void UpdateAnimations()
    {
        if (isDead)
        {
            PlayAnimation(deathAnimation);
        }
        else if (isHurt)
        {
            PlayAnimation(hurtAnimation);
        }
        else if (isAttacking)
        {
            // Attack animations are handled in StartAttack()
        }
        else if (isLanding)
        {
            PlayAnimation(landingAnimation);
        }
        else if (isDashing)
        {
            PlayAnimation(dashAnimation);
        }
        else if (isCrouching)
        {
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                PlayAnimation(crouchWalkAnimation);
            }
            else
            {
                PlayAnimation(crouchIdleAnimation);
            }
        }
        else if (!isGrounded)
        {
            PlayAnimation(rb.velocity.y > 0 ? jumpAnimation : fallAnimation);
        }
        else if (Mathf.Abs(moveInput) > 0.1f)
        {
            PlayAnimation(walkAnimation);
        }
        else
        {
            PlayAnimation(idleAnimation);
        }
    }

    void PlayAnimation(AnimationClip animationClip)
    {
        if (currentAnimation == animationClip)
            return;

        currentAnimation = animationClip;
        animator.CrossFade(animationClip.name, 0.1f);
    }

    // Implement DamageReceiver interface
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (isDead || isHurt)
            return;

        // Calculate actual damage after defense
        int actualDamage = Mathf.Max(damageInfo.damageAmount - playerStats.currentDefense, 0);
        playerStats.currentHealth -= actualDamage;

        // Update Health Bar
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)playerStats.currentHealth / playerStats.baseHealth;
        }

        if (playerStats.currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Apply knockback
            rb.velocity = Vector2.zero; // Reset velocity
            rb.AddForce(damageInfo.knockbackDirection * damageInfo.knockbackForce, ForceMode2D.Impulse);

            // Play hurt animation
            isHurt = true;
            hurtTimer = hurtDuration;
            PlayAnimation(hurtAnimation);

            // Invoke the OnTakeDamage event
            OnTakeDamage.Invoke();
        }
    }

    void Die()
    {
        isDead = true;
        // Disable movement
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Invoke the OnDeath event
        OnDeath.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        // Draw Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // Draw Attack Range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // Optional: Flash Red Coroutine (Visual Feedback)
    IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}
