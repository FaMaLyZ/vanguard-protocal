using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Camera cam;

    void Start()
    {
        // Get the Camera component attached to this GameObject
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // --- Panning Logic ---
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

        // Move the camera based on input
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // --- Zooming Logic ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Adjust the orthographic size and clamp it between min and max values
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}