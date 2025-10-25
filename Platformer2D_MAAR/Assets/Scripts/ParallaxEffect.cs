using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("Speed factor for the background parallax effect.")]
    [SerializeField] private float speedBackground;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    /// <summary>
    /// Updates the background position to create a parallax effect based on camera movement.
    /// </summary>
    private void LateUpdate()
    {
        Vector3 backgroundMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(backgroundMovement.x * speedBackground, backgroundMovement.y * speedBackground, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
