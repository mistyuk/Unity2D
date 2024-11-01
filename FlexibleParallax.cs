using UnityEngine;

[System.Serializable]
public class ParallaxLayerGroup
{
    public Transform[] layers;          // Array of transforms for background or foreground layers
    public float parallaxMultiplierX = 0.5f; // Horizontal parallax multiplier
    public float parallaxMultiplierY = 0.0f; // Vertical parallax multiplier
    public float damping = 1.0f;        // Damping factor (lower values mean less damping)
    public bool controlZAxis = false;   // Option to control Z-axis movement
}

public class FlexibleParallax : MonoBehaviour
{
    public ParallaxLayerGroup backgroundGroup; // Group for background layers
    public ParallaxLayerGroup foregroundGroup; // Group for foreground layers

    public float depthStep = 1.0f;     // Step distance between layers on the Z-axis
    public float verticalShift = 0.0f; // Vertical shift applied uniformly to all layers

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        // Initialize the camera position tracking
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        // Distribute backgrounds and foregrounds along the Z-axis
        DistributeLayers(backgroundGroup, -depthStep); // Backgrounds go further back
        DistributeLayers(foregroundGroup, depthStep);  // Foregrounds go closer to the camera

        // Ensure damping is not zero or negative in both groups
        ValidateDamping(backgroundGroup);
        ValidateDamping(foregroundGroup);
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Apply parallax effect to each layer group
        ApplyParallax(backgroundGroup, deltaMovement);
        ApplyParallax(foregroundGroup, deltaMovement);

        // Update the last camera position for the next frame
        lastCameraPosition = cameraTransform.position;
    }

    void ApplyParallax(ParallaxLayerGroup group, Vector3 deltaMovement)
    {
        foreach (Transform layer in group.layers)
        {
            if (layer != null)
            {
                // Calculate the movement for this layer
                Vector3 movement = new Vector3(
                    deltaMovement.x * group.parallaxMultiplierX,
                    deltaMovement.y * group.parallaxMultiplierY,
                    0); // Z-axis is not affected by default

                Vector3 targetPosition = layer.position + movement;

                // Apply vertical shift uniformly
                targetPosition.y += verticalShift;

                // Control Z-axis if enabled for the group
                if (!group.controlZAxis)
                {
                    targetPosition.z = layer.position.z; // Keep current Z position if controlZAxis is false
                }

                // Smoothly move towards the target position using Lerp
                layer.position = Vector3.Lerp(
                    layer.position,
                    targetPosition,
                    Time.deltaTime * group.damping);
            }
        }
    }

    void DistributeLayers(ParallaxLayerGroup group, float zStep)
    {
        for (int i = 0; i < group.layers.Length; i++)
        {
            if (group.layers[i] != null)
            {
                Vector3 newPosition = group.layers[i].position;
                newPosition.z = i * zStep; // Distribute layers along the Z-axis
                group.layers[i].position = newPosition;
            }
        }
    }

    void ValidateDamping(ParallaxLayerGroup group)
    {
        if (group.damping <= 0f)
        {
            group.damping = 1.0f; // Ensure a positive damping value
        }
    }
}
