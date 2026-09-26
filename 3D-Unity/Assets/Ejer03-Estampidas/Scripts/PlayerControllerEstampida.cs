using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int puntuacionTotal = 0;
    private float airTimer = 0f;
    private bool hasScoredInCurrentJump = false;

    [Header("Referencias OBLIGATORIAS")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider playerCollider;

    [Header("Configuración Inicial")]
    [SerializeField] private Vector3 initialPosition = new Vector3(0f, 0.7f, 8.5f);
    [SerializeField] private Vector3 initialScale = new Vector3(0.7f, 0.7f, 0.7f);

    [Header("Configuración de Pistas")]
    [SerializeField] private float laneStep = 2.5f;
    [SerializeField] private string trackTag = "Track";

    [Header("Física del Salto")]
    [SerializeField] private float jumpHeight = 2.5f;

    [Header("Mecánica de Rodar (Roll)")]
    [SerializeField] private float rollDuration = 0.8f;
    [SerializeField] private float rollColliderHeight = 1f;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private float rollTimer = 0f;
    private bool isRolling = false;

    private float minXLimit;
    private float maxXLimit;
    private bool isValidSetup = true;
    private bool isGrounded = true;

    private void Start()
    {
        if (rb == null)
        {
            Debug.LogError($"[PlayerController] ERROR: No se ha asignado el 'Rigidbody' en el Inspector.");
            isValidSetup = false;
            enabled = false;
            return;
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (playerCollider == null) playerCollider = GetComponent<CapsuleCollider>();

        if (playerCollider != null)
        {
            originalColliderHeight = playerCollider.height;
            originalColliderCenter = playerCollider.center;
        }

        transform.position = initialPosition;
        transform.localScale = initialScale;

        CalculateTrackLimits();
    }

    private void CalculateTrackLimits()
    {
        GameObject[] tracks = GameObject.FindGameObjectsWithTag(trackTag);
        int trackCount = tracks.Length;

        if (trackCount == 0 || trackCount % 2 == 0)
        {
            Debug.LogError($"[PlayerController] ERROR: Se requiere un número IMPAR de pistas. Pistas actuales: {trackCount}");
            isValidSetup = false;
            enabled = false;
            return;
        }

        int lanesToEachSide = (trackCount - 1) / 2;
        float maxOffset = lanesToEachSide * laneStep;

        minXLimit = -maxOffset;
        maxXLimit = maxOffset;
    }

    private void Update()
    {
        if (!isValidSetup) return;

        // 1. Temporizador de aire
        if (airTimer > 0f)
        {
            airTimer -= Time.deltaTime;
        }

        // 2. Temporizador de rodado
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                StopRoll();
            }
        }

        // 3. DETECCIÓN DE CAÍDA (Cuando la velocidad Y empieza a ser negativa en el aire)
        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            if (animator != null)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        // 4. Lectura de controles solo en el suelo
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.W) && !isRolling)
            {
                JumpVertical();
            }
            else if (Input.GetKeyDown(KeyCode.S) && !isRolling)
            {
                StartRoll();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                TryMoveLane(-laneStep);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                TryMoveLane(laneStep);
            }
        }
    }

    private void JumpVertical()
    {
        TriggerJumpAnimation();
        ApplyParabolicImpulse(0f, jumpHeight);
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }

        if (playerCollider != null)
        {
            playerCollider.height = rollColliderHeight;
            playerCollider.center = new Vector3(originalColliderCenter.x, rollColliderHeight / 2f, originalColliderCenter.z);
        }
    }

    private void StopRoll()
    {
        isRolling = false;

        if (playerCollider != null)
        {
            playerCollider.height = originalColliderHeight;
            playerCollider.center = originalColliderCenter;
        }
    }

    private void TryMoveLane(float directionStep)
    {
        float targetX = transform.position.x + directionStep;

        if (targetX < minXLimit - 0.1f || targetX > maxXLimit + 0.1f) return;

        if (isRolling) StopRoll();

        TriggerJumpAnimation();
        ApplyParabolicImpulse(directionStep, jumpHeight);
    }

    private void TriggerJumpAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Jump");
            animator.SetBool("IsGrounded", false);
            animator.SetBool("IsFalling", false);
        }
    }

    private void ApplyParabolicImpulse(float distanceX, float height)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        float vy = Mathf.Sqrt(2f * gravity * height);
        float totalTime = 2f * vy / gravity;
        float vx = distanceX / totalTime;

        rb.linearVelocity = new Vector3(vx, vy, 0f);
        isGrounded = false;

        airTimer = totalTime;
        hasScoredInCurrentJump = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(trackTag) || collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;

            if (animator != null)
            {
                animator.SetBool("IsGrounded", true);
                animator.SetBool("IsFalling", false);
            }

            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log($"Objeto tocado: {other.gameObject.name}");
        }
        else if (other.CompareTag("ScoreZone"))
        {
            if (airTimer > 0f && !hasScoredInCurrentJump)
            {
                puntuacionTotal++;
                hasScoredInCurrentJump = true;
                Debug.Log($"¡Punto conseguido! Tienes: {puntuacionTotal} puntos");
            }
        }
    }
}