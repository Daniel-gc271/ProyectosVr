using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*
     * Problema: puntuacion multiple durante parabola de salto horizontal
     * solucion: usar un temporizador que calcule el tiempo restante para aterrizar.
     * al saltar el jugador tocara el trigger que puntua, sumara uno a la puntuacion
     * posteriormente bloqueara la capacidad de obtener puntuacion hasta aterrizar
     *
     */
    private int puntuacionTotal = 0;
    private float airTimer = 0f; // Temporizador dinámico en el aire
    private bool hasScoredInCurrentJump = false; // Bloquea múltiples puntos en un mismo salto
    [Header("Referencias OBLIGATORIAS")]
    [SerializeField] private Rigidbody rb; // Asignar desde el Inspector

    [Header("Configuración Inicial")]
    [SerializeField] private Vector3 initialPosition = new Vector3(0f, 0.7f, 8.5f);
    [SerializeField] private Vector3 initialScale = new Vector3(0.7f, 0.7f, 0.7f);

    [Header("Configuración de Pistas")]
    [SerializeField] private float laneStep = 2.5f;
    [SerializeField] private string trackTag = "Track";

    [Header("Física del Salto")]
    [SerializeField] private float jumpHeight = 2.5f; // Altura máxima del salto / arco (H)

    private float minXLimit;
    private float maxXLimit;
    private bool isValidSetup = true;
    private bool isGrounded = true;

    private void Start()
    {
        // 1. Validar la referencia del Rigidbody
        if (rb == null)
        {
            Debug.LogError($"[PlayerController] ERROR: No se ha asignado el componente 'Rigidbody' en el Inspector. El juego no se ejecutará.");
            isValidSetup = false;
            enabled = false;
            return;
        }

        // 2. Asignar transformación inicial
        transform.position = initialPosition;
        transform.localScale = initialScale;

        // 3. Validar pistas y límites de carril
        CalculateTrackLimits();
    }

    private void CalculateTrackLimits()
    {
        GameObject[] tracks = GameObject.FindGameObjectsWithTag(trackTag);
        int trackCount = tracks.Length;

        // Validar si la cantidad de pistas es cero o PAR
        if (trackCount == 0 || trackCount % 2 == 0)
        {
            Debug.LogError($"[PlayerController] ERROR: Se requiere un número IMPAR de objetos con la etiqueta '{trackTag}'. " +
                           $"Actualmente hay {trackCount} pistas. El juego no se ejecutará.");
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

        // Descontar el tiempo de aire frame a frame
        if (airTimer > 0f)
        {
            airTimer -= Time.deltaTime;
        }

        // Solo permitir acciones de salto/desplazamiento si el personaje está en el suelo
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                JumpVertical();
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
        // Aplica el salto con desplazamiento cero en el eje X
        ApplyParabolicImpulse(0f, jumpHeight);
    }

    private void TryMoveLane(float directionStep)
    {
        float targetX = transform.position.x + directionStep;

        // Comprobar si el carril objetivo está dentro de los límites
        if (targetX < minXLimit - 0.1f || targetX > maxXLimit + 0.1f) return;

        // Executar salto parabólico lateral
        ApplyParabolicImpulse(directionStep, jumpHeight);
    }

    private void ApplyParabolicImpulse(float distanceX, float height)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        // 1. Velocidad inicial necesaria en Y para alcanzar la altura 'height'
        float vy = Mathf.Sqrt(2f * gravity * height);

        // 2. Tiempo total de vuelo (subida + bajada)
        float totalTime = 2f * vy / gravity;

        // 3. Velocidad horizontal necesaria en X
        float vx = distanceX / totalTime;

        // Aplicar la velocidad al Rigidbody asignado
        rb.linearVelocity = new Vector3(vx, vy, 0f);
        isGrounded = false;

        airTimer = totalTime; // Setear el tiempo exacto que durará el salto
        hasScoredInCurrentJump = false; // Permitimos 1 punto para este nuevo salto
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detectar contacto con la pista o superficie horizontal
        if (collision.gameObject.CompareTag(trackTag) || collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;

            // Detener la velocidad residual al colisionar con el suelo para fijar la posición
            rb.linearVelocity = Vector3.zero;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto con el que chocó tiene la etiqueta "Obstacle"
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log($"Objeto tocado: {other.gameObject.name}");
        }
        else
        {
            // Logica de puntuacion
            if (other.CompareTag("ScoreZone")) // O la etiqueta que le asignes al cubo blanco
            {
                // Solo puntúa si el tiempo de aire sigue activo Y no ha puntuado ya en este salto
                if (airTimer > 0f && !hasScoredInCurrentJump)
                {
                    puntuacionTotal++;
                    hasScoredInCurrentJump = true; // Bloquea más puntos hasta el próximo salto
                    Debug.Log($"¡Punto conseguido!\nTienes: {puntuacionTotal} puntos");
                    // ScoreManager.instance.AddPoint();
                }
            }
        }
        
    }
    
}