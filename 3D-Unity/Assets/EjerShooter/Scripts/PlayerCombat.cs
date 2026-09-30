using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    [SerializeField] private GameObject bulletPrefab; // Arrastra aquí el Prefab de la esfera
    [SerializeField] private Transform firePoint;     // Punto desde donde sale la bala (opcional, si no se asigna sale del centro de la cámara)
    [SerializeField] private float bulletForce = 40f;  // Fuerza de propulsión de la bala 

    [Header("Configuración de Apuntado (Zoom)")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float aimFOV = 40f;
    [SerializeField] private float aimSpeed = 8f;

    private PlayerControls playerControls;
    private Camera cam;
    private bool isAiming = false;
    private float targetFOV;

    private void Awake()
    {
        playerControls = new PlayerControls();
        cam = GetComponent<Camera>();
        targetFOV = normalFOV;
    }

    private void OnEnable()
    {
        playerControls.ShooterPlayerMovement.Enable();
        // Al presionar clic izquierdo (started), llamamos a Disparar
        playerControls.ShooterPlayerMovement.Shoot.started += ctx => Disparar();

        // Comportamiento de apuntado (mantener presionado clic derecho)
        playerControls.ShooterPlayerMovement.Aim.performed += ctx => isAiming = true;
        playerControls.ShooterPlayerMovement.Aim.canceled += ctx => isAiming = false;

    }

    private void OnDisable()
    {
        playerControls.ShooterPlayerMovement.Disable();
    }

    private void Update()
    {
        ManejarApuntado();
    }

    private void Disparar()
    {
        if (bulletPrefab == null) return;
        // Si no asignaste un firePoint, la bala nace en la posición de la cámara
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position + transform.forward * 0.5f;
        Quaternion spawnRotation = transform.rotation;

        // Crear la bala en el mundo físico
        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        // Obtener su Rigidbody y aplicarle fuerza instantánea hacia adelante
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(transform.forward * bulletForce, ForceMode.VelocityChange);
        }

        // Destruir la bala automáticamente a los 5 segundos para optimizar rendimiento
        Destroy(newBullet, 5f);

    }

    private void ManejarApuntado()
    {
        // Determinar el FOV objetivo
        targetFOV = isAiming ? aimFOV : normalFOV;
        // Transición suave (Lerp) para el efecto de zoom
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, aimSpeed * Time.deltaTime);

    }

}