using UnityEngine;

public class GameViewFlyCam : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float lookSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        // Toggle camera rotation with Right Mouse Button
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * lookSensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * lookSensitivity;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0b0);

            // WASD Movement relative to camera facing direction
            float moveX = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
            float moveZ = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

            transform.Translate(new Vector3(moveX, 0, moveZ));
        }
    }
}