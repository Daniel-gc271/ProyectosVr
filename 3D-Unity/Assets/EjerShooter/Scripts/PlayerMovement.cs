using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Velocidades")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Configuración de Salto e Inercia")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float airControlFactor = 0.2f; // 0 = sin control en el aire, 1 = control total

    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Rigidbody rb;

    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isGrounded = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerControls.ShooterPlayerMovement.Enable();
        playerControls.ShooterPlayerMovement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.ShooterPlayerMovement.Move.canceled += ctx => moveInput = Vector2.zero;

        playerControls.ShooterPlayerMovement.Sprint.started += ctx => ToggleSprint();
        playerControls.ShooterPlayerMovement.Crouch.started += ctx => ToggleCrouch();

        // MODIFICACIÓN 3A: Suscribir la acción de salto (Asegúrate de crear "Jump" en tu Asset de Inputs)
        playerControls.ShooterPlayerMovement.Jump.started += ctx => Saltar();

    }

    private void OnDisable()
    {
        playerControls.ShooterPlayerMovement.Disable();
    }

    private void FixedUpdate()
    {
        MoverPersonaje();
    }

    private void ToggleSprint()
    {
        if (isCrouching) return;
        isSprinting = !isSprinting;
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        if (isCrouching)
        {
            isSprinting = false;
        }
    }

    // MODIFICACIÓN 3B: Método para ejecutar el salto físico
    private void Saltar()
    {
        if (isGrounded)
        {
            // Aplicamos un impulso directo hacia arriba ignorando la masa para una respuesta inmediata
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Cancelamos estados de movimiento al saltar si lo deseas por diseño
            isCrouching = false;
        }

    }

    private void MoverPersonaje()
    {
        // 1. Evitar exceso de velocidad diagonal normalizando el input
        Vector2 normalizedInput = moveInput;
        if (normalizedInput.magnitude > 1f)
        {
            normalizedInput.Normalize();
        }

        // 2. Determinar la velocidad máxima deseada según el estado
        float currentSpeed = walkSpeed;
        if (isSprinting && normalizedInput.magnitude > 0)
        {
            currentSpeed = sprintSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        if (normalizedInput.magnitude == 0 && isSprinting)
        {
            isSprinting = false;
        }

        
        // Tomamos la dirección hacia donde mira la cámara principal del juego
        Transform camTransform = Camera.main.transform;

        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;

        // Proyectamos los vectores en el plano horizontal (eliminamos el eje Y para no volar/hundirnos)
        camForward.y = 0f;
        camRight.y = 0f;

        // Volvemos a normalizar los vectores para que la inclinación de la cámara no reste velocidad
        camForward.Normalize();
        camRight.Normalize();

        // Calculamos la dirección final en base a la vista de la cámara
        Vector3 moveDirection = (camForward * normalizedInput.y) + (camRight * normalizedInput.x);
        Vector3 targetVelocity = moveDirection * currentSpeed;
        // ==========================================

        // 3. Lógica diferenciada Suelo vs Aire
        if (isGrounded)
        {
            // En el suelo tenemos tracción y control total inmediato
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // En el aire calculamos la velocidad horizontal que el jugador INTENTA aplicar
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // Interpolamos linealmente (Lerp) de la velocidad actual hacia la deseada usando el factor de control
            Vector3 airMovement = Vector3.Lerp(horizontalVelocity, targetVelocity, airControlFactor * Time.fixedDeltaTime * 10f);

            // Aplicamos la velocidad resultante respetando la gravedad original (eje Y)
            airMovement.y = rb.linearVelocity.y;
            rb.linearVelocity = airMovement;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor")) { isGrounded = true; }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor")) { isGrounded = false; }
    }

}