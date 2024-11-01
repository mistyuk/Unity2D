using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private Vector3 moveDirection;
    private float moveSpeed = 2.5f; // For normal hits
    private float fadeDuration = 1.5f;
    private bool isCritical = false;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Initializes the damage text with the specified damage amount and critical hit status.
    /// </summary>
    /// <param name="damage">Damage amount to display.</param>
    /// <param name="critical">Is this a critical hit?</param>
    /// <param name="customMoveDirection">Optional custom movement direction.</param>
    public void Initialize(int damage, bool critical, Vector3 customMoveDirection = default(Vector3))
    {
        isCritical = critical;
        textMesh.text = damage.ToString();

        if (isCritical)
        {
            // Configure for Critical Hits
            textMesh.color = new Color(0.5f, 0f, 0f, 1f); // Dark Red
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.fontSize = 36; // Slightly larger
            moveDirection = Vector3.zero; // No floating
        }
        else
        {
            // Configure for Normal Hits
            textMesh.color = Color.white; // White color
            textMesh.fontStyle = FontStyles.Normal;
            textMesh.fontSize = 24; // Normal size

            // Alternate floating direction: right then left in a cycle with randomness
            moveDirection = customMoveDirection != Vector3.zero 
                ? customMoveDirection.normalized 
                : (Random.value > 0.5f 
                    ? new Vector3(1f, 1f, 0) 
                    : new Vector3(-1f, 1f, 0));
            
            // Add randomness to movement direction
            moveDirection += new Vector3(Random.Range(-0.2f, 0.2f), 0f, 0f);
        }
    }

    void Update()
    {
        if (moveDirection != Vector3.zero)
        {
            // Move the text
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        // Fade out over time
        Color currentColor = textMesh.color;
        currentColor.a -= Time.deltaTime / fadeDuration;
        textMesh.color = currentColor;

        // Destroy after fade duration
        fadeDuration -= Time.deltaTime;
        if (fadeDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
