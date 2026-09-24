using UnityEngine;

public class Spawner : MonoBehaviour {
    [Header("Configuración de Prefabs")]
    // Cambiamos 'GameObject' por un array 'GameObject[]' para aceptar múltiples prefabs
    [SerializeField] private GameObject[] prefabsToSpawn;
    [SerializeField] private string spawnerTag = "Spawner";

    [Header("Configuración del Tiempo")]
    [SerializeField] private float startDelay = 0f;     // Cuánto tarda en salir el primer objeto
    [SerializeField] private float spawnInterval = 3f;  // Cada cuántos segundos se repite

    private Transform spawnPoint;

    private void Start()
    {
        // Intentar encontrar el objeto Spawner en la escena por su Tag
        GameObject spawnerObject = GameObject.FindWithTag(spawnerTag);

        // Verificamos que se haya encontrado el spawner y que el array tenga al menos 1 prefab
        if (spawnerObject != null && prefabsToSpawn != null && prefabsToSpawn.Length > 0)
        {
            spawnPoint = spawnerObject.transform;

            // Llama a la función "SpawnObject" repetidamente
            InvokeRepeating(nameof(SpawnObject), startDelay, spawnInterval);
        }
        else
        {
            Debug.LogWarning("Falta asignar prefabs en la lista o no se encontró ningún objeto con el Tag: " + spawnerTag);
        }
    }
    [ContextMenu("Generar Barrera")]
    private void SpawnObject()
    {
        // Elegir un índice aleatorio entre 0 y el total de prefabs asignados
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        // Instanciar el prefab elegido al azar en la posición y rotación del spawner
        Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Objeto generado: " + selectedPrefab.name);
    }
}