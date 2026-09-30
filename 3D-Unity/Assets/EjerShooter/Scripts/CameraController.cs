using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Sensibilidad")]
    [SerializeField] private float mouseSensitivity = 15f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Referencias a Objetos")]
    [SerializeField] private Transform playerBody; // Arrastra aquí el GameObject principal del jugador

    private PlayerControls playerControls;
    private Vector2 lookInput;
    private float xRotation = 0f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        // Ocultar y bloquear el cursor en el centro de la pantalla para comodidad del shooter
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void OnEnable()
    {
        playerControls.ShooterPlayerMovement.Enable();
        // Leer el movimiento continuo del ratón
        playerControls.ShooterPlayerMovement.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.ShooterPlayerMovement.Look.canceled += ctx => lookInput = Vector2.zero;

    }

    private void OnDisable()
    {
        playerControls.ShooterPlayerMovement.Disable();
    }

    private void Update()
    {
        RotarCamara();
    }

    private void RotarCamara()
    {
        // Multiplicamos por la sensibilidad y deltaTime para suavizar el frame rate
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime * 10f;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime * 10f;
        // Calcular y limitar (Clamp) la rotación vertical (Arriba / Abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Aplicar la rotación vertical solo a la cámara (este GameObject)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Aplicar la rotación horizontal (Izquierda / Derecha) al cuerpo completo del jugador
        playerBody.Rotate(Vector3.up * mouseX);

    }

}