using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Configuración de Prefabs")]
    [SerializeField] private GameObject[] prefabsToSpawn;

    [Header("Punto de Generación")]
    [Tooltip("Objeto vacío que servirá como punto de origen. Si se deja vacío, usará este mismo transform.")]
    [SerializeField] private GameObject spawnerObject;

    [Header("Configuración del Tiempo")]
    [SerializeField] private float startDelay = 0f;        // Tiempo inicial antes del primer spawn
    [SerializeField] private float spawnInterval = 3f;     // Tiempo base entre spawns
    [SerializeField] private float randomVariance = 1f;    // Margen de aleatoriedad (± segundos)

    private Transform spawnPoint;
    private float timer;
    private bool isReadyToSpawn;

    private void Start()
    {
        // Asignar el punto de generación
        spawnPoint = spawnerObject != null ? spawnerObject.transform : transform;

        // Comprobación de seguridad
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning($"[Spawner] Falta asignar prefabs en el objeto {gameObject.name}.");
            enabled = false; // Desactiva el Update si no hay prefabs
            return;
        }

        // El primer temporizador usa el retraso inicial (startDelay)
        timer = startDelay;
        isReadyToSpawn = true;
    }

    private void Update()
    {
        if (!isReadyToSpawn) return;

        // Restamos el tiempo transcurrido desde el último frame
        timer -= Time.deltaTime;

        // Cuando el temporizador llega a cero
        if (timer <= 0f)
        {
            SpawnObject();
            SetNextSpawnTime();
        }
    }

    private void SetNextSpawnTime()
    {
        // Calcula un tiempo aleatorio entre (intervalo - variación) y (intervalo + variación)
        float minTime = Mathf.Max(0.1f, spawnInterval - randomVariance);
        float maxTime = spawnInterval + randomVariance;

        timer = Random.Range(minTime, maxTime);
    }

    [ContextMenu("Generar Objeto")]
    private void SpawnObject()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0) return;

        Transform targetTransform = spawnPoint != null ? spawnPoint : transform;

        // Seleccionar prefab aleatorio
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // Instanciar
        Instantiate(selectedPrefab, targetTransform.position, targetTransform.rotation);
        Debug.Log("Objeto generado: " + selectedPrefab.name);
    }
}